using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlightScript : MonoBehaviour
{
    CharacterController characterController;
    public Rigidbody rb;
    float velocity;
    float gravity;
    private Vector3 moveDirection = Vector3.zero; //Shorthand for writing Vector3(0, 0, 0).
    bool isGrounded; 


    // Start is called before the first frame update
    void Start()
    {
         characterController = GetComponent<CharacterController>();
        /*  characterController.enableOverlapRecovery = true; */
    }

    void Update()
    {
    // zet de move elke frame op Zero zodat hij stopt wanneer er geen input is
        moveDirection.x = 0;
        moveDirection.z = 0;


        Debug.Log("hello");
        Debug.Log(characterController.isGrounded);
        isGrounded = characterController.isGrounded;

            if (Input.GetKey(KeyCode.Z))
            {
                moveDirection.z = 1f ;
            }
            else if (Input.GetKey(KeyCode.S))
            {
            moveDirection.z = -1f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                moveDirection.x = 1f;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                moveDirection.x = -1f;
            }

        Vector3 movement = moveDirection;
        Debug.Log(movement);
        movement.Normalize();
        Vector3 movementXZ = new Vector3(movement.x, 0, movement.z).normalized;
          
        //Look where to go
        if (movementXZ != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementXZ), 0.1f);
        } else {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.zero), 0.1f);
        }

        
        transform.Translate(new Vector3(0, velocity, 0) * Time.deltaTime);
        //Flying controlls -> if no flying then gravity
        if (Input.GetKey(KeyCode.F)){
           movement.y = 20f;
           Debug.Log(movement);
        } else if(Input.GetKey(KeyCode.C)){
            movement.y = -20f;
        } else if(Input.GetKeyDown(KeyCode.Space)){
            movement.y = 200f + (-9.8f * Time.deltaTime);
        } else{
            gravity = -9.8f * Time.deltaTime;
            movement.y = -9.8f;
            Debug.Log("gravity should be happening");
        }
          
        // actual moving    
                Vector3 movingVector = new Vector3(movementXZ.x, movement.y, movementXZ.z);
                Debug.Log(movingVector);
                characterController.Move(new Vector3(movementXZ.x, movement.y, movementXZ.z) * Time.deltaTime);
    }
    
}
