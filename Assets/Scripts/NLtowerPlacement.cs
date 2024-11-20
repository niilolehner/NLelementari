using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLtowerPlacement : MonoBehaviour
{
    // initialize variables, objects and references
    [SerializeField] private LayerMask NLplacementCheckMask;
    [SerializeField] private LayerMask NLplacementCollideMask;
    [SerializeField] private NLplayerStats NLplayerStatistics;
    [SerializeField] private Camera NLplayerCamera;

    private GameObject NLcurrentPlacingTower;

    // Update is called once per frame
    void Update()
    {
        // change position of NLcurrentPlacingTower to mouse raycast hit position
        if( NLcurrentPlacingTower != null )
        {

            Ray NLcamray = NLplayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit NLhitInfo;
            if(Physics.Raycast(NLcamray, out NLhitInfo, 100f, NLplacementCollideMask))
            {
                NLcurrentPlacingTower.transform.position = NLhitInfo.point;
            }

            // cancel tower placement
            if(Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(NLcurrentPlacingTower);
                NLcurrentPlacingTower = null;
                return;
            }

            if(Input.GetMouseButtonDown(0) && NLhitInfo.collider.gameObject != null)
            {
                // check if can place tower in clicked location
                if (!NLhitInfo.collider.gameObject.CompareTag("NLcantPlace"))
                {
                    BoxCollider NLtowerCollider = NLcurrentPlacingTower.GetComponent<BoxCollider>();
                    NLtowerCollider.isTrigger = true;
                    
                    Vector3 NLboxCenter = NLcurrentPlacingTower.transform.position + NLtowerCollider.center;
                    Vector3 NLhalfExtents = NLtowerCollider.size / 2;
                    if(!Physics.CheckBox(NLboxCenter, NLhalfExtents, Quaternion.identity, NLplacementCheckMask, QueryTriggerInteraction.Ignore))
                    {
                        // getting tower behaviour
                        NLtowerBehaviour NLcurrentTowerBehaviour = NLcurrentPlacingTower.GetComponent<NLtowerBehaviour>();

                        // add tower to NLtowersInGame
                        NLgameLoopManager.NLtowersInGame.Add(NLcurrentTowerBehaviour);

                        // remove mana once placed
                        NLplayerStatistics.NLaddMana(-NLcurrentTowerBehaviour.NLsummonCost);

                        NLtowerCollider.isTrigger = false;
                        NLcurrentPlacingTower = null;
                    }
                }
            }
        }
    }
    // place tower!
    public void NLsetTowerToPlace(GameObject NLtower)
    {
        int NLtowerSummonCost = NLtower.GetComponent<NLtowerBehaviour>().NLsummonCost;

        if(NLplayerStatistics.NLgetMana() >= NLtowerSummonCost)
        {
            NLcurrentPlacingTower = Instantiate(NLtower, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("Need more mana to summon a " + NLtower.name + "!");
        }
    }
}
