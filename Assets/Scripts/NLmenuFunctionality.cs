using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NLmenuFunctionality : MonoBehaviour
{
    // initialize variables, objects and references

    // Start is called before the first frame update
    void Start()
    {
        // initialize variables and objects
    }

    // load level
    public void NLloadLevel()
    {
        SceneManager.LoadScene(1);
    }

    // load menu
    public void NLloadMenu()
    {
        SceneManager.LoadScene(0);
    }

    // exit game
    public void NLexitGame()
    {
        // exit the game, depending if in editor or live app, change method
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }
}
