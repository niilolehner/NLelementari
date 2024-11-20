using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLflameDamage : MonoBehaviour, NLiDamageMethod
{
    // initialize variables, objects and references
    [SerializeField] private ParticleSystem NLfireEffect;
    [SerializeField] private Collider NLfireTrigger;

    [HideInInspector] public float NLdamage;
    [HideInInspector] public float NLfirerate;

    public void NLinit(float NLdamage, float NLfirerate)
    {
        // initialize variables
        this.NLdamage = NLdamage;
        this.NLfirerate = NLfirerate;
    }
    public void NLdamageUpdate(NLenemy NLtarget)
    {
        // enable firetrigger when there is a target
        NLfireTrigger.enabled = NLtarget != null;

        // play flame effect if enemy target and if flame is not playing already
        if(NLtarget)
        {
            if(!NLfireEffect.isPlaying) NLfireEffect.Play();
            return;
        }

        // stop flame when no enemy target
        NLfireEffect.Stop();
    }
}
