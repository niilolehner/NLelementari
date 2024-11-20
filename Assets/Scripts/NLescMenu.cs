using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLescMenu : MonoBehaviour
{
    [SerializeField] private GameObject NLgameEscMenu;
    private NLgameOver NLgameOverM;

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables and objects
        NLgameOverM = FindObjectOfType<NLgameOver>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // bring up or close esc menu if game is running
        if (NLgameOverM.NLgameOverIsNow == false)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                NLgameEscMenu.SetActive(!NLgameEscMenu.activeSelf);
            }
        }
    }
}
