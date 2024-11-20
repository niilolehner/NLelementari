using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NLplayerStats : MonoBehaviour
{
    // initialize variables, objects and references
    [SerializeField] private TextMeshProUGUI NLmanaDisplayText;
    [SerializeField] private int NLstartingMana;
    private int NLcurrentMana;

    // Start is called before the first frame update
    void Start()
    {
        NLcurrentMana = NLstartingMana;
        NLmanaDisplayText.SetText($"Mana \n{NLstartingMana}");
    }

    // manipulate player mana
    public void NLaddMana(int NLmanaToAdd)
    {
        NLcurrentMana += NLmanaToAdd;
        NLmanaDisplayText.SetText($"Mana \n{NLcurrentMana}");
    }

    public int NLgetMana()
    {
        return NLcurrentMana;
    }
}
