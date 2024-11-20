using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using System;

public class NLtowerTargeting
{
    // initialize variables, objects and references
    public enum NLtargetType
    {
        NLfirst,
        NLlast,
        NLclose
    }

    // targeting system framework
    public static NLenemy NLgetTarget(NLtowerBehaviour NLcurrentTower, NLtargetType NLtargetMethod)
    {
        // get enemies overlapping sphere collider
        Collider[] NLenemiesInRange = Physics.OverlapSphere(NLcurrentTower.transform.position, NLcurrentTower.NLrange, NLcurrentTower.NLenemiesLayer);
        
        // set up native arrays and assign multithreading jobs
        NativeArray<NLenemyData> NLenemiesToCalculate = new NativeArray<NLenemyData>(NLenemiesInRange.Length, Allocator.TempJob);
        NativeArray<Vector3> NLnodePositions = new NativeArray<Vector3>(NLgameLoopManager.NLnodePositions, Allocator.TempJob);
        NativeArray<float> NLnodeDistances = new NativeArray<float> (NLgameLoopManager.NLnodeDistances, Allocator.TempJob);
        NativeArray<int> NLenemyToIndex = new NativeArray<int>(new int[] { -1 }, Allocator.TempJob);
        int NLenemyIndexToReturn = -1;

        // dispose of Native Arrays
        void NLdispose()
        {
            NLenemiesToCalculate.Dispose();
            NLnodePositions.Dispose();
            NLnodeDistances.Dispose();
            NLenemyToIndex.Dispose();
        }

        // assign enemy data
        for (int i = 0; i < NLenemiesToCalculate.Length; i++)
        {
            NLenemy NLcurrentEnemy = NLenemiesInRange[i].transform.parent.GetComponent<NLenemy>();

            int NLenemyIndexInList = NLentitySummoner.NLenemiesInGame.FindIndex(x => x == NLcurrentEnemy);

            NLenemiesToCalculate[i] = new NLenemyData(NLcurrentEnemy.transform.position, NLcurrentEnemy.NLnodeIndex, NLcurrentEnemy.NLhealth, NLenemyIndexInList); 
        }

        // initialize search for enemies job
        NLsearchForEnemy NLenemySearchJob = new NLsearchForEnemy
        {
            _NLenemiesToCalculate = NLenemiesToCalculate,
            _NLnodeDistances = NLnodeDistances,
            _NLnodePositions = NLnodePositions,
            _NLenemyToIndex = NLenemyToIndex,
            NLtargetingType = (int)NLtargetMethod,
            NLtowerPosition = NLcurrentTower.transform.position
        };

        // initialize NLcompareValue
        switch((int) NLtargetMethod)
        {
            case 0: // NLfirst
                NLenemySearchJob.NLcompareValue = Mathf.Infinity;

                break;

            case 1: // NLlast
                NLenemySearchJob.NLcompareValue = Mathf.NegativeInfinity;

                break;

            case 2: // NLclose

                goto case 0;

            case 3: // NLstrong

                goto case 1;

            case 4: // NLweak

                goto case 0;
        }

        // create JobHandle
        JobHandle NLdependency = new JobHandle();
        JobHandle NLSearchJobHandle = NLenemySearchJob.Schedule(NLenemiesToCalculate.Length, NLdependency);
        NLSearchJobHandle.Complete();

        // return null if no enemies in range at all
        if (NLenemyToIndex[0] == -1)
        {
            // dispose of Native Arrays before returning null
            NLdispose();

            return null;
        }
        NLenemyIndexToReturn =  NLenemiesToCalculate[NLenemyToIndex[0]].NLenemyIndex;

        // dispose of Native Arrays after job done
        NLdispose();

        return NLentitySummoner.NLenemiesInGame[NLenemyIndexToReturn];
    }

    struct NLenemyData
    {
        // get enemy data
        public NLenemyData(Vector3 NLnmyPosition, int NLndeIndex, float NLhp, int NLnmyIndex)
        {
            NLenemyPosition = NLnmyPosition;
            NLenemyIndex = NLnmyIndex;
            NLnodeIndex = NLndeIndex;
            NLhealth = NLhp;
        }

        public Vector3 NLenemyPosition;
        public int NLenemyIndex;
        public int NLnodeIndex;
        public float NLhealth;
    }

    // search for the enemies job
    struct NLsearchForEnemy : IJobFor
    {
        // initialize job native arrays and variables
        [ReadOnly] public NativeArray<NLenemyData> _NLenemiesToCalculate;
        [ReadOnly] public NativeArray<Vector3> _NLnodePositions;
        [ReadOnly] public NativeArray<float> _NLnodeDistances;
        [NativeDisableParallelForRestriction] public NativeArray<int> _NLenemyToIndex;
        public Vector3 NLtowerPosition;
        public float NLcompareValue;
        public int NLtargetingType;

        public void Execute(int NLindex)
        {
            // distance from enemy cases
            float NLcurrentEnemyDistanceToEnd = 0;
            float NLdistanceToEnemy = 0;
            switch (NLtargetingType)
            {
                case 0: // NLfirst

                    NLcurrentEnemyDistanceToEnd = NLgetDistanceToEnd(_NLenemiesToCalculate[NLindex]);
                    if (NLcurrentEnemyDistanceToEnd < NLcompareValue)
                    {
                        _NLenemyToIndex[0] = NLindex;
                        NLcompareValue = NLcurrentEnemyDistanceToEnd;
                    }

                    break;

                case 1: // NLlast

                    NLcurrentEnemyDistanceToEnd = NLgetDistanceToEnd(_NLenemiesToCalculate[NLindex]);
                    if (NLcurrentEnemyDistanceToEnd > NLcompareValue)
                    {
                        _NLenemyToIndex[0] = NLindex;
                        NLcompareValue = NLcurrentEnemyDistanceToEnd;
                    }

                    break;

                case 2: // NLclose

                    NLdistanceToEnemy = Vector3.Distance(NLtowerPosition, _NLenemiesToCalculate[NLindex].NLenemyPosition);
                    if (NLdistanceToEnemy < NLcompareValue)
                    {
                        _NLenemyToIndex[0] = NLindex;
                        NLcompareValue = NLdistanceToEnemy;
                    }

                    break;

                case 3: // NLstrong

                    
                    if (_NLenemiesToCalculate[NLindex].NLhealth > NLcompareValue)
                    {
                        _NLenemyToIndex[0] = NLindex;
                        NLcompareValue = _NLenemiesToCalculate[NLindex].NLhealth;
                    }

                    break;

                case 4: // NLweak

                    if (_NLenemiesToCalculate[NLindex].NLhealth < NLcompareValue)
                    {
                        _NLenemyToIndex[0] = NLindex;
                        NLcompareValue = _NLenemiesToCalculate[NLindex].NLhealth;
                    }

                    break;


            }
        }

        // calculate distance from enemy to path end
        private float NLgetDistanceToEnd(NLenemyData NLenemyToEvaluate)
        {
            float NLfinalDistance = Vector3.Distance(NLenemyToEvaluate.NLenemyPosition, _NLnodePositions[NLenemyToEvaluate.NLnodeIndex]);

            for (int i = NLenemyToEvaluate.NLnodeIndex; i < _NLnodeDistances.Length; i++)
            {
                NLfinalDistance += _NLnodeDistances[i];
            }

            return NLfinalDistance;
        }
    }
}