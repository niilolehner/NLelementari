using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// set up standard damage interface
public interface NLiDamageMethod
{
    // initialize damage
    public void NLinit(float NLdamage, float NLfirerate);

    // update damage
    public void NLdamageUpdate(NLenemy NLtarget);
}

public class NLwaterDamage : MonoBehaviour, NLiDamageMethod
{
    // initialize variables, objects and references
    public LayerMask NLenemiesLayer;
    [SerializeField] private ParticleSystem NLwaterShootSystem;
    [SerializeField] private Transform NLtowerHead;

    private ParticleSystem.MainModule NLwaterShootSystemMain;
    [HideInInspector] public float NLdamage;
    private float NLfirerate;
    private float NLdelay;

    public void NLinit(float NLdamage, float NLfirerate)
    {
        // initialize variables
        NLwaterShootSystemMain = NLwaterShootSystem.main;
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
            NLwaterShootSystemMain.startRotationX = NLtowerHead.forward.x;
            NLwaterShootSystemMain.startRotationY = NLtowerHead.forward.x;
            NLwaterShootSystemMain.startRotationZ = NLtowerHead.forward.x;

            // play water shoot
            NLwaterShootSystem.Play();
            NLdelay = 1f / NLfirerate;
        }
    }
}