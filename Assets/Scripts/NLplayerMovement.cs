using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NLplayerMovement : MonoBehaviour
{
    // initialize variables, objects and references
    private Vector3 NLvelocity;
    private Vector3 NLplayerMovementInput;
    private Vector2 NLplayerMouseInput;
    private float NLxRotation;

    [SerializeField] private Transform NLplayerCamera;
    [SerializeField] private CharacterController NLcontroller;
    [Space]
    [SerializeField] private float NLspeed;
    [SerializeField] private float NLsensitivity;

    // Update is called once per frame
    void Update()
    {
        // set movement input vectors
        NLplayerMovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        NLplayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // move player and camera
        NLmovePlayer();
        NLmovePlayerCamera();
    }

    private void NLmovePlayer()
    {
        // detect inputs and move player
        Vector3 NLmoveVector = transform.TransformDirection(NLplayerMovementInput);

        if (Input.GetKey(KeyCode.LeftShift)) // float up
        {
            NLvelocity.y = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // float down
        {
            NLvelocity.y = -1f;
        }

        NLcontroller.Move(NLmoveVector * NLspeed * Time.deltaTime);
        NLcontroller.Move(NLvelocity * NLspeed * Time.deltaTime);

        // reset velocity to 0 to avoid infinitely moving without input
        NLvelocity.y = 0f;
    }

    private void NLmovePlayerCamera()
    {
        // only rotate camera if right mouse button down
        if(Input.GetMouseButton(1))
        {
            // make camera rotate with player movement
            NLxRotation -= NLplayerMouseInput.y * NLsensitivity;
            transform.Rotate(0f, NLplayerMouseInput.x * NLsensitivity, 0f);
            NLplayerCamera.transform.localRotation = Quaternion.Euler(NLxRotation, 0f, 0f);
        }
    }
}
