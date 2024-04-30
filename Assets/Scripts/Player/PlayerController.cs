using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Input Variables")]
    [SerializeField] float movementSpeed = 4f;
    [SerializeField] float turningRate = 180f;

    PlayerControls playerControls;
    Vector2 movementInput;
    Rigidbody2D myRigidbody;
    Transform treadsTrans;


    private void Awake() {
        playerControls = new PlayerControls();
        treadsTrans = this.transform.GetChild(0);
        myRigidbody = GetComponent<Rigidbody2D>();
    }


    private void Update() {
        if(!IsOwner) {return;}
        ReadPlayerInput();
        
    }


    private void FixedUpdate() {
        if(!IsOwner) {return;}
        Move();
    }


    private void Move()
    {
        if(movementInput != Vector2.zero)
        {
            float rotationZ = movementInput.x * (-turningRate * Time.deltaTime);
            treadsTrans.Rotate(new Vector3(0f, 0f, rotationZ));
            myRigidbody.velocity = (Vector2) treadsTrans.up * (movementInput.y * movementSpeed);
        }
        else
        {
            myRigidbody.velocity = Vector2.zero;
        }
    }



    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) {return;}
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) {return;}
    }

    private void ReadPlayerInput()
    {
        movementInput = playerControls.Movement.Move.ReadValue<Vector2>();
    }

}
