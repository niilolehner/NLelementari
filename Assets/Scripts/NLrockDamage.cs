using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLrockDamage : MonoBehaviour, NLiDamageMethod
{
    // initialize variables, objects and references
    public LayerMask NLenemiesLayer;
    [SerializeField] private ParticleSystem NLrockShootSystem;
    [SerializeField] private Transform NLtowerHead;

    private ParticleSystem.MainModule NLrockShootSystemMain;
    [HideInInspector] public float NLdamage;
    private float NLfirerate;
    private float NLdelay;

    public void NLinit(float NLdamage, float NLfirerate)
    {
        // initialize variables
        NLrockShootSystemMain = NLrockShootSystem.main;
        this.NLdamage = NLdamage;
        this.NLfirerate = NLfirerate;
        NLdelay = 1f / NLfirerate;
    }
    public void NLdamageUpdate(NLenemy NLtarget)
    {
        if (NLtarget)
        {
            // enact fire rate delay
            if (NLdelay > 0f)
            {
                NLdelay -= Time.deltaTime;
                return;
            }

            // set particle rotation
            NLrockShootSystemMain.startRotationX = NLtowerHead.forward.x;
            NLrockShootSystemMain.startRotationY = NLtowerHead.forward.x;
            NLrockShootSystemMain.startRotationZ = NLtowerHead.forward.x;

            // play rock shoot
            NLrockShootSystem.Play();
            NLdelay = 1f / NLfirerate;
        }
    }
}
