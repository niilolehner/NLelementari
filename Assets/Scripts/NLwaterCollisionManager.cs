using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLwaterCollisionManager : MonoBehaviour
{
    // initialize variables, objects and references
    [SerializeField] private NLwaterDamage NLbaseClass;
    [SerializeField] private ParticleSystem NLwaterExplosionSystem;
    [SerializeField] private ParticleSystem NLwaterShootSystem;
    [SerializeField] private float NLwaterExplosionRadius;
    private List<ParticleCollisionEvent> NLwaterCollisions;

    // Start is called before the first frame update
    public void Start()
    {
        // initialize variables
        NLwaterCollisions = new List<ParticleCollisionEvent>();
    }

    // fire OnParticleCollision
    void OnParticleCollision(GameObject NLother)
    {
        NLwaterShootSystem.GetCollisionEvents(NLother, NLwaterCollisions);

        for (int NLcollisionEvent = 0; NLcollisionEvent < NLwaterCollisions.Count; NLcollisionEvent++)
        {
            // get explosion position and play explosion
            NLwaterExplosionSystem.transform.position = NLwaterCollisions[NLcollisionEvent].intersection;
            NLwaterExplosionSystem.Play();

            Collider[] NLenemiesInRadius = Physics.OverlapSphere(NLwaterCollisions[NLcollisionEvent].intersection, NLwaterExplosionRadius, NLbaseClass.NLenemiesLayer);

            for (int i = 0; i < NLenemiesInRadius.Length; i++)
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
