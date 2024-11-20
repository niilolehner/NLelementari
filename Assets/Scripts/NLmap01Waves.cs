using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NLmap01Waves : MonoBehaviour
{
    // initialize variables, objects and references
    [SerializeField] private TextMeshProUGUI NLwavesDisplayText;
    [SerializeField] private GameObject NLgameWinMenu;
    [SerializeField] private GameObject NLgameEscMenu;
    private NLgameLoopManager NLgameLoop;
    private NLplayerStats NLplayerStatistics;
    private NLgameOver NLgameOverM;
    private int NLwaves;

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables and objects
        NLgameOverM = FindObjectOfType<NLgameOver>();
        NLwaves = 5;
        NLwavesDisplayText.SetText($"Waves \n{NLwaves}");

        NLplayerStatistics = FindObjectOfType<NLplayerStats>();
        NLgameLoop = FindObjectOfType<NLgameLoopManager>();
        StartCoroutine(NLwaveSpawner());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // waves
    IEnumerator NLwaveSpawner()
    {
        // start

        //WAVE 1
        // intro water

        yield return new WaitForSeconds(2);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        yield return new WaitForSeconds(1);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        yield return new WaitForSeconds(1);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        yield return new WaitForSeconds(1);

        CancelInvoke();

        yield return new WaitUntil(() => NLentitySummoner.NLenemiesInGame.Count == 0);

        NLplayerStatistics.NLaddMana(100);

        NLwaves -= 1;
        NLwavesDisplayText.SetText($"Waves \n{NLwaves}");

        //WAVE 2
        // water, intro flame

        yield return new WaitForSeconds(2);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        yield return new WaitForSeconds(1);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        yield return new WaitForSeconds(1);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        yield return new WaitForSeconds(1);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonFlame", 0f, 1f);

        yield return new WaitForSeconds(5);

        CancelInvoke();

        yield return new WaitUntil(() => NLentitySummoner.NLenemiesInGame.Count == 0);

        NLplayerStatistics.NLaddMana(100);

        NLwaves -= 1;
        NLwavesDisplayText.SetText($"Waves \n{NLwaves}");


        //WAVE 3
        // water and flame, intro rock

        yield return new WaitForSeconds(2);

        InvokeRepeating("NLsummonRock", 0f, 1f);

        yield return new WaitForSeconds(11);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        InvokeRepeating("NLsummonFlame", 0f, 1f);

        yield return new WaitForSeconds(23);

        CancelInvoke();

        yield return new WaitUntil(() => NLentitySummoner.NLenemiesInGame.Count == 0);

        NLplayerStatistics.NLaddMana(100);

        NLwaves -= 1;
        NLwavesDisplayText.SetText($"Waves \n{NLwaves}");


        //WAVE 4
        // rock, water and flame, intro wind

        yield return new WaitForSeconds(2);

        InvokeRepeating("NLsummonWater", 0f, 1f);

        InvokeRepeating("NLsummonRock", 0f, 1f);

        yield return new WaitForSeconds(3);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonFlame", 0f, 1f);

        InvokeRepeating("NLsummonWind", 0f, 0.25f);

        yield return new WaitForSeconds(15);

        CancelInvoke();

        yield return new WaitUntil(() => NLentitySummoner.NLenemiesInGame.Count == 0);

        NLplayerStatistics.NLaddMana(100);

        NLwaves -= 1;
        NLwavesDisplayText.SetText($"Waves \n{NLwaves}");


        //WAVE 5
        // ramp up spawn frequency (AOE necessary)

        yield return new WaitForSeconds(2);

        InvokeRepeating("NLsummonWater", 0f, 0.5f);

        InvokeRepeating("NLsummonRock", 0f, 0.75f);

        yield return new WaitForSeconds(15);

        CancelInvoke();

        yield return new WaitForSeconds(3);

        InvokeRepeating("NLsummonFlame", 0f, 0.25f);

        InvokeRepeating("NLsummonWind", 0f, 0.25f);

        yield return new WaitForSeconds(19);

        CancelInvoke();

        yield return new WaitUntil(() => NLentitySummoner.NLenemiesInGame.Count == 0);

        NLplayerStatistics.NLaddMana(100);

        NLwaves -= 1;
        NLwavesDisplayText.SetText($"Waves \n{NLwaves}");

        // end

        yield return new WaitForSeconds(2);

        if (NLgameOverM.NLgameOverIsNow == false)
        {
            NLgameOverM.NLgameOverIsNow = true;
            NLgameEscMenu.SetActive(false);
            NLgameWinMenu.SetActive(true);
            NLgameLoop.NLloopShouldEnd = true;
        }
    }

    // put elemental spirits of type into spawn queue
    void NLsummonWater()
    {
        NLgameLoopManager.NLenqueueEnemyIDtoSummon(1);
    }
    void NLsummonFlame()
    {
        NLgameLoopManager.NLenqueueEnemyIDtoSummon(2);
    }
    void NLsummonRock()
    {
        NLgameLoopManager.NLenqueueEnemyIDtoSummon(3);
    }
    void NLsummonWind()
    {
        NLgameLoopManager.NLenqueueEnemyIDtoSummon(4);
    }
}
