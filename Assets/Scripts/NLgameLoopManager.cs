using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class NLgameLoopManager : MonoBehaviour
{
    // initialize variables, objects and references
    public static List<NLtowerBehaviour> NLtowersInGame; // reference to all placed towers
    public Transform NLnodeParent; // reference to parent node of movement nodes
    public static Vector3[] NLnodePositions; // reference of nodes to move to
    public static float[] NLnodeDistances; // reference of distances between nodes 

    private static Queue<NLapplyEffectData> NLeffectsQueue; // queue of effects to apply
    private static Queue<NLenemyDamageData> NLdamageData; // queue of damage to apply
    private static Queue<int> NLenemyIDsToSummon; // queue of enemies to summon
    private static Queue<NLenemy> NLenemiesToRemove; // queue of enemies to remove

    private NLplayerStats NLplayerStatistics;

    public bool NLloopShouldEnd;

    // Start is called before the first frame update
    private void Start()
    {
        // initialize variables and objects
        NLeffectsQueue = new Queue<NLapplyEffectData>();
        NLplayerStatistics = FindObjectOfType<NLplayerStats>();
        NLdamageData = new Queue<NLenemyDamageData>();
        NLtowersInGame = new List<NLtowerBehaviour>();
        NLenemyIDsToSummon = new Queue<int>();
        NLenemiesToRemove = new Queue<NLenemy>();
        NLentitySummoner.NLinit();
       
        NLnodePositions = new Vector3[NLnodeParent.childCount];
        // populate NLnodePositions
        for (int i = 0; i < NLnodePositions.Length; i++)
        {
            NLnodePositions[i] = NLnodeParent.GetChild(i).position;
        }

        // get distance between nodes
        NLnodeDistances = new float [NLnodePositions.Length - 1];
        for (int i = 0; i < NLnodeDistances.Length; i++)
        {
            NLnodeDistances[i] = Vector3.Distance(NLnodePositions[i], NLnodePositions[i + 1]);
        }

        // start game loop/coroutines
        StartCoroutine(NLgameLoop());
    }

    // main game loop
    IEnumerator NLgameLoop()
    {
        while(NLloopShouldEnd == false)
        {
            // SPAWN ENEMIES in queue
            if(NLenemyIDsToSummon.Count > 0)
            {
                for (int i = 0; i < NLenemyIDsToSummon.Count; i++)
                {
                    NLentitySummoner.NLsummonEnemy(NLenemyIDsToSummon.Dequeue());
                }
            }

            // SPAWN TOWERS
            // tower spawning handled in NLtowerPlacement

            // MOVE ENEMIES in queue
            // create native arrays and transform access array containing movement variables and assign multithreading jobs
            NativeArray<Vector3> NLnodesToUse = new NativeArray<Vector3>(NLnodePositions, Allocator.TempJob);
            NativeArray<float> NLenemySpeeds = new NativeArray<float>(NLentitySummoner.NLenemiesInGame.Count, Allocator.TempJob);
            NativeArray<int> NLnodeIndices = new NativeArray<int>(NLentitySummoner.NLenemiesInGame.Count, Allocator.TempJob);
            TransformAccessArray NLenemyAccess = new TransformAccessArray(NLentitySummoner.NLenemiesInGameTransform.ToArray(), 2);

            // populate the above native arrays
            for (int i = 0; i < NLentitySummoner.NLenemiesInGame.Count; i++)
            {
                NLenemySpeeds[i] = NLentitySummoner.NLenemiesInGame[i].NLspeed;
                NLnodeIndices[i] = NLentitySummoner.NLenemiesInGame[i].NLnodeIndex;
            }

            // initialize enemies move job
            NLmoveEnemiesJob NLmoveJob = new NLmoveEnemiesJob()
            {
                NLnodePositions = NLnodesToUse,
                NLenemySpeed = NLenemySpeeds,
                NLnodeIndex = NLnodeIndices,
                NLdeltaTime = Time.deltaTime
            };

            JobHandle NLmoveJobHandle = NLmoveJob.Schedule(NLenemyAccess);
            NLmoveJobHandle.Complete();

            // set indexes and data that job handle modified to enemy classes
            for(int i = 0; i < NLentitySummoner.NLenemiesInGame.Count; i++)
            {
                NLentitySummoner.NLenemiesInGame[i].NLnodeIndex = NLnodeIndices[i];

                if(NLentitySummoner.NLenemiesInGame[i].NLnodeIndex == NLnodePositions.Length)
                {
                    NLenqueueEnemyToRemove(NLentitySummoner.NLenemiesInGame[i]);
                }
            }

            // dispose of native arrays after job is done
            NLenemySpeeds.Dispose();
            NLnodeIndices.Dispose();
            NLenemyAccess.Dispose();
            NLnodesToUse.Dispose();

            // UPDATE TOWERS
            foreach(NLtowerBehaviour NLtower in NLtowersInGame)
            {
                NLtower.NLtarget = NLtowerTargeting.NLgetTarget(NLtower, NLtowerTargeting.NLtargetType.NLfirst);
                NLtower.NLupdate();
            }

            // APPLY EFFECTS in queue
            if (NLeffectsQueue.Count > 0)
            {
                for (int i = 0; i < NLeffectsQueue.Count; i++)
                {
                    NLapplyEffectData NLcurrentDamageData = NLeffectsQueue.Dequeue();
                    NLeffect NLeffectDuplicate = NLcurrentDamageData.NLenemyToAffect.NLactiveEffects.Find(x => x.NLeffectName == NLcurrentDamageData.NLeffectToApply.NLeffectName);
                    
                    // check for duplicate effects
                    if (NLeffectDuplicate == null)
                    {
                        NLcurrentDamageData.NLenemyToAffect.NLactiveEffects.Add(NLcurrentDamageData.NLeffectToApply);
                    }
                    else
                    {
                        NLeffectDuplicate.NLexpireTime = NLcurrentDamageData.NLeffectToApply.NLexpireTime;
                    }
                }
            }

            // UPDATE ENEMIES 
            foreach(NLenemy NLcurrentEnemy in NLentitySummoner.NLenemiesInGame)
            {
                NLcurrentEnemy.NLupdate();
            }

            // DAMAGE ENEMIES in queue
            if (NLdamageData.Count > 0)
            {
                for (int i = 0; i < NLdamageData.Count; i++)
                {
                    NLenemyDamageData NLcurrentDamageData = NLdamageData.Dequeue();

                    // apply damage and resistance
                    NLcurrentDamageData.NLtargetedEnemy.NLhealth -= NLcurrentDamageData.NLtotalDamage / NLcurrentDamageData.NLresistance;

                    // give mana to player based on damage done
                    NLplayerStatistics.NLaddMana((int)NLcurrentDamageData.NLtotalDamage);

                    // queue to remove if 0 health left
                    if (NLcurrentDamageData.NLtargetedEnemy.NLhealth <= 0f)
                    {
                        NLenqueueEnemyToRemove(NLcurrentDamageData.NLtargetedEnemy);
                    }
                }
            }

            // REMOVE ENEMIES in queue
            if (NLenemiesToRemove.Count > 0)
            {
                for (int i = 0; i < NLenemiesToRemove.Count; i++)
                {
                    NLentitySummoner.NLremoveEnemy(NLenemiesToRemove.Dequeue());
                }
            }

            yield return null;
        }
    }

    public static void NLenqueueEffectToApply(NLapplyEffectData NLefctData)
    {
        NLeffectsQueue.Enqueue(NLefctData);
    }

    public static void NLenqueueDamageData(NLenemyDamageData NLdmgData)
    {
        NLdamageData.Enqueue(NLdmgData);
    }

    // put enemy in line to be summoned
    public static void NLenqueueEnemyIDtoSummon(int NLid)
    {
        NLenemyIDsToSummon.Enqueue(NLid);
    }

    public static void NLenqueueEnemyToRemove(NLenemy NLenemyToRemove)
    {
        NLenemiesToRemove.Enqueue(NLenemyToRemove);
    }
}

// setting up effects
public class NLeffect
{
    public NLeffect(string NLefctName, float NLdmgRate, float NLdmg, float NLexprTime)
    {
        NLeffectName = NLefctName;
        NLdamageRate = NLdmgRate;
        NLdamage = NLdmg;
        NLexpireTime = NLexprTime;
    }

    public string NLeffectName;

    public float NLdamageDelay;
    public float NLdamageRate;
    public float NLdamage;

    public float NLexpireTime;
}

// enemy to Affect/affect to apply data
public struct NLapplyEffectData
{
    public NLapplyEffectData(NLenemy NLnmyToAffect, NLeffect NLefctToApply)
    {
        NLenemyToAffect = NLnmyToAffect;
        NLeffectToApply = NLefctToApply;
    }

    public NLenemy NLenemyToAffect;
    public NLeffect NLeffectToApply;
}

// assign NLenemyDamageData
public struct NLenemyDamageData
{
    public NLenemyDamageData(NLenemy NLtarget, float NLdamage, float NLdmgresistance)
    {
        NLtargetedEnemy = NLtarget;
        NLtotalDamage = NLdamage;
        NLresistance = NLdmgresistance;
    }

    public NLenemy NLtargetedEnemy;
    public float NLtotalDamage;
    public float NLresistance;
}

// job to move enemies
public struct NLmoveEnemiesJob : IJobParallelForTransform
{
    // initialize job native arrays and variables
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NLnodePositions;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> NLenemySpeed;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> NLnodeIndex;
    public float NLdeltaTime;
    public void Execute(int NLindex, TransformAccess NLtransform)
    {
        // check if there are any more node positions to move toward
        if (NLnodeIndex[NLindex] < NLnodePositions.Length)
        {
            // set target position
            Vector3 NLpositionToMoveTo = NLnodePositions[NLnodeIndex[NLindex]];

            // get enemy moving toward position
            NLtransform.position = Vector3.MoveTowards(NLtransform.position, NLpositionToMoveTo, NLenemySpeed[NLindex] * NLdeltaTime);

            // once there, go to next node in node index
            if (NLtransform.position == NLpositionToMoveTo)
            {
                NLnodeIndex[NLindex]++;
            }
        }
    }
}
