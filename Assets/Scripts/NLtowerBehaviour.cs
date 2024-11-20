using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLtowerBehaviour : MonoBehaviour
{
    // initialize variables, objects and references
    public LayerMask NLenemiesLayer;
    public NLenemy NLtarget;
    public Transform NLtowerPivot;

    public float NLdamage;
    public float NLfirerate;
    public float NLrange;
    private float NLdelay;
    public int NLsummonCost = 100;

    private NLiDamageMethod NLcurrentDamageMethodClass;

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables and objects
        NLcurrentDamageMethodClass = GetComponent<NLiDamageMethod>();

        // check if no damage method class
        if(NLcurrentDamageMethodClass == null )
        {
            Debug.LogError("TOWERS: NO DAMAGE CLASS ATTACHED TO GIVEN TOWER!");
        }
        else
        {
            NLcurrentDamageMethodClass.NLinit(NLdamage, NLfirerate);
        }

        NLdelay = 1 / NLfirerate;
    }

    // update function
    public void NLupdate()
    {
        NLcurrentDamageMethodClass.NLdamageUpdate(NLtarget);

        if(NLtarget != null)
        {
            // orient tower head toward NLtarget
            NLtowerPivot.transform.rotation = Quaternion.LookRotation(NLtarget.transform.position - transform.position);
        }
    }
}
