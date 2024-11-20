using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NLenemy : MonoBehaviour
{
    // initialize variables, objects and references
    public Transform NLrootPart;
    public float NLmaxHealth, NLhealth, NLspeed, NLdamageResistance = 1f;
    public int NLid, NLnodeIndex;

    public List<NLeffect> NLactiveEffects;

    // initialize variables etc
    public void NLinit()
    {
        // initialize active effects
        NLactiveEffects = new List<NLeffect>();

        // set NLhealth to max
        NLhealth = NLmaxHealth;
        // set transform to movement node 0 position
        transform.position = NLgameLoopManager.NLnodePositions[0];
        // set node index to beginning of path
        NLnodeIndex = 0;
    }

    // update effects / damages / effect time etc
    public void NLupdate()
    {
        for (int i = 0; i < NLactiveEffects.Count; i++)
        {
            if (NLactiveEffects[i].NLexpireTime > 0f)
            {
                if(NLactiveEffects[i].NLdamageDelay > 0f)
                {
                    NLactiveEffects[i].NLdamageDelay -= Time.deltaTime;
                }
                else
                {
                    NLgameLoopManager.NLenqueueDamageData(new NLenemyDamageData(this, NLactiveEffects[i].NLdamage, NLdamageResistance));
                    NLactiveEffects[i].NLdamageDelay = 1f / NLactiveEffects[i].NLdamageRate;
                }

                NLactiveEffects[i].NLexpireTime -= Time.deltaTime;
            }
        }

        // stop effect if time of effect over
        NLactiveEffects.RemoveAll(x => x.NLexpireTime <= 0f);
    }
}
