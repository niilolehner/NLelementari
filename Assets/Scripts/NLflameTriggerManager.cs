using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLflameTriggerManager : MonoBehaviour
{
    // initialize variables, objects and references
    [SerializeField] private NLflameDamage NLbaseClass;

    // fire onTriggerenter
    private void OnTriggerEnter(Collider NLother)
    {
        // check if entered is enemy
        if (NLother.gameObject.CompareTag("NLenemy"))
        {
            // set up burning effect
            NLeffect NLflameEffect = new NLeffect("Burning", NLbaseClass.NLfirerate, NLbaseClass.NLdamage, 6f);
            NLapplyEffectData NLeffectData = new NLapplyEffectData(NLentitySummoner.NLenemyTransformPairs[NLother.transform.parent], NLflameEffect);
            NLgameLoopManager.NLenqueueEffectToApply(NLeffectData);
        }
    }
}
