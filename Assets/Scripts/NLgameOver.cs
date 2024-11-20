using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NLgameOver : MonoBehaviour
{
    // initialize variables, objects and references
    private NLgameLoopManager NLgameLoop;
    [SerializeField] private TextMeshProUGUI NLlivesDisplayText;
    [SerializeField] private GameObject NLgameOverMenu;
    [SerializeField] private GameObject NLgameEscMenu;
    [HideInInspector] public bool NLgameOverIsNow;
    private int NLlives;

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables and objects
        NLlives = 10;
        NLlivesDisplayText.SetText($"Lives \n{NLlives}");
        NLgameOverIsNow = false;
        NLgameLoop = FindObjectOfType<NLgameLoopManager>();
    }

    // onTriggerEnter
    private void OnTriggerEnter(Collider NLother)
    {
        // check if entered is enemy
        if (NLother.gameObject.CompareTag("NLenemy"))
        {
            // subtract lives and update UI
            NLlives -= 1;
            NLlivesDisplayText.SetText($"Lives \n{NLlives}");

            // if lives are zero or below stop game loop and trigger game over
            if (NLlives <= 0)
            {
                NLgameOverIsNow = true;
                NLgameEscMenu.SetActive(false);
                NLgameOverMenu.SetActive(true);
                NLgameLoop.NLloopShouldEnd = true;
            }
        }
    }
}
