using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLrockCollisionManager : MonoBehaviour
{
    // initialize variables, objects and references
    [SerializeField] private NLrockDamage NLbaseClass;
    [SerializeField] private ParticleSystem NLrockExplosionSystem;
    [SerializeField] private ParticleSystem NLrockShootSystem;
    [SerializeField] private float NLrockExplosionRadius;
    private List<ParticleCollisionEvent> NLrockCollisions;

    // Start is called before the first frame update
    public void Start()
    {
        // initialize variables
        NLrockCollisions = new List<ParticleCollisionEvent>();
    }

    // fire OnParticleCollision
    void OnParticleCollision(GameObject NLother)
    {
        NLrockShootSystem.GetCollisionEvents(NLother, NLrockCollisions);

        for(int NLcollisionEvent = 0; NLcollisionEvent < NLrockCollisions.Count; NLcollisionEvent++)
        {
            // get explosion position and play explosion
            NLrockExplosionSystem.transform.position = NLrockCollisions[NLcollisionEvent].intersection;
            NLrockExplosionSystem.Play();

            Collider[] NLenemiesInRadius = Physics.OverlapSphere(NLrockCollisions[NLcollisionEvent].intersection, NLrockExplosionRadius, NLbaseClass.NLenemiesLayer);

            for(int i = 0; i < NLenemiesInRadius.Length; i++)
            {
                // check enemies in range of explosion
                NLenemy NLenemyToDamage = NLentitySummoner.NLenemyTransformPairs[NLenemiesInRadius[i].transform.parent];
                
                // send damage
                NLenemyDamageData NLdamageToApply = new NLenemyDamageData(NLenemyToDamage, NLbaseClass.NLdamage, NLenemyToDamage.NLdamageResistance);
                NLgameLoopManager.NLenqueueDamageData(NLdamageToApply);
            }
        }
    }
}
