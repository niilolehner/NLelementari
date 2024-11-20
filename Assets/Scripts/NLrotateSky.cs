using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NLrotateSky : MonoBehaviour
{
    // initialize variables, objects and references
    public float NLrotateSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        // rotate skybox
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * NLrotateSpeed);
    }
}
