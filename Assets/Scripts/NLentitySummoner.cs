using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLentitySummoner : MonoBehaviour
{
    // initialize variables, objects and references
    public static List<NLenemy> NLenemiesInGame; // references all enemies currently alive in the game
    public static List<Transform> NLenemiesInGameTransform; // references to all transforms of enemies in the game

    public static Dictionary<Transform, NLenemy> NLenemyTransformPairs; // references transforms to NLenemy
    public static Dictionary<int, GameObject> NLenemyPrefabs; // references to NLenemyID, NLenemyPrefab
    public static Dictionary<int, Queue<NLenemy>> NLenemyObjectPools; // make a queue of enemies, references to NLenemy

    public static void NLinit()
    {
            // initialize dictionaries and lists
            NLenemyTransformPairs = new Dictionary<Transform, NLenemy>();
            NLenemyPrefabs = new Dictionary<int, GameObject>();
            NLenemyObjectPools = new Dictionary<int, Queue<NLenemy>>();
            NLenemiesInGame = new List<NLenemy>();
            NLenemiesInGameTransform = new List<Transform>();


            // get array of all NLenemies in resources folder
            NLenemySummonData[] NLenemies = Resources.LoadAll<NLenemySummonData>("Enemies");

            // populate NLenemyPrefabs and NLenemyObjectPools with enemy
            foreach (NLenemySummonData NLnmy in NLenemies)
            {
                NLenemyPrefabs.Add(NLnmy.NLenemyID, NLnmy.NLenemyPrefab);
                NLenemyObjectPools.Add(NLnmy.NLenemyID, new Queue<NLenemy>());
            }
    }

    // spawn enemies
    public static NLenemy NLsummonEnemy(int NLenemyID)
    {
        NLenemy NLsummonedEnemy = null;

        // check if NLenemy actually exists in NLenemyPrefabs
        if(NLenemyPrefabs.ContainsKey(NLenemyID))
        {
            Queue<NLenemy> NLreferencedQueue = NLenemyObjectPools[NLenemyID];

            if(NLreferencedQueue.Count > 0)
            {
                // dequeue enemy and initialize
                NLsummonedEnemy = NLreferencedQueue.Dequeue();
                NLsummonedEnemy.NLinit();

                // re-enable enemy after dequeue
                NLsummonedEnemy.gameObject.SetActive(true);
            }
            else
            {
                // instantiate new enemy and initialize, spawn at movement node 0
                GameObject NLnewEnemy = Instantiate(NLenemyPrefabs[NLenemyID], NLgameLoopManager.NLnodePositions[0], Quaternion.identity);
                NLsummonedEnemy = NLnewEnemy.GetComponent<NLenemy>();
                NLsummonedEnemy.NLinit();
            }
        }
        else
        {
            Debug.Log($"ENTITYSUMMONER: ENEMY WITH ID OF {NLenemyID} DOES NOT EXIST!");
            return null;
        }

        // add spawned enemy transform to NLenemiesInGameTransform (check if duplicate already present)
        if(!NLenemiesInGameTransform.Contains(NLsummonedEnemy.transform)) NLenemiesInGameTransform.Add(NLsummonedEnemy.transform);
        // add spawned enemy to NLenemiesInGame (check if duplicate already present)
        if (!NLenemiesInGame.Contains(NLsummonedEnemy)) NLenemiesInGame.Add(NLsummonedEnemy);
        // add transforms to NLenemyTransformPairs (check if duplicate already present)
        if (!NLenemyTransformPairs.ContainsKey(NLsummonedEnemy.transform)) NLenemyTransformPairs.Add(NLsummonedEnemy.transform, NLsummonedEnemy);
        // assign NLid
        NLsummonedEnemy.NLid = NLenemyID;
        return NLsummonedEnemy;
    }

    // remove enemies
    public static void NLremoveEnemy(NLenemy NLenemyToRemove)
    {
        // put enemy back into the cue, idle and ready to be used
        NLenemyObjectPools[NLenemyToRemove.NLid].Enqueue(NLenemyToRemove);
        // deactivate the enemy
        NLenemyToRemove.gameObject.SetActive(false);

        // remove enemy from NLenemyTransformPairs
        NLenemyTransformPairs.Remove(NLenemyToRemove.transform);
        // remove enemy from NLenemiesInGameTransform
        NLenemiesInGameTransform.Remove(NLenemyToRemove.transform);
        // remove enemy from NLenemiesInGame
        NLenemiesInGame.Remove(NLenemyToRemove);
    }
}
