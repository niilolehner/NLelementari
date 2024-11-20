using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLwindDamage : MonoBehaviour, NLiDamageMethod
{
    // initialize variables, objects and references
    [SerializeField] private Transform NLbeamPivot;
    [SerializeField] private LineRenderer NLbeamRenderer;

    private float NLdamage;
    private float NLfirerate;
    private float NLdelay;

    public void NLinit(float NLdamage, float NLfirerate)
    {
        // initialize variables
        this.NLdamage = NLdamage;
        this.NLfirerate = NLfirerate;
        NLdelay = 1f / NLfirerate;
    }
    public void NLdamageUpdate(NLenemy NLtarget)
    {
        // detect if enemy exists
        if (NLtarget)
        {
            // beam renderer settings
            NLbeamRenderer.enabled = true;
            NLbeamRenderer.SetPosition(0, NLbeamPivot.position);
            NLbeamRenderer.SetPosition(1, NLtarget.NLrootPart.position);

            // enact fire rate delay
            if (NLdelay > 0f)
            {
                NLdelay -= Time.deltaTime;
                return;
            }

            // send damage
            NLgameLoopManager.NLenqueueDamageData(new NLenemyDamageData(NLtarget, NLdamage, NLtarget.NLdamageResistance));
            NLdelay = 1f / NLfirerate;
            return;
        }

        // switch off beam renderer when no enemy
        NLbeamRenderer.enabled = false;
    }
}
