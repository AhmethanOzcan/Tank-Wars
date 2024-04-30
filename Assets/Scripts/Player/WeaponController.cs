using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    PlayerControls playerControls;
    bool attackButtonDown = false;
    bool isAttacking = false;
    bool isOnCooldown = false;
    [SerializeField] float attackCooldown = .5f;

    private void Awake() {
        playerControls = new PlayerControls();
    }

    private void Start() {
        playerControls.Combat.PrimaryFire.started += _ => StartAttacking();
        playerControls.Combat.PrimaryFire.canceled += _ => StopAttacking();
    }

    private void FixedUpdate() {
        if(!IsOwner) {return;}
        FaceMouse();
        Fire();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void FaceMouse()
    {
        Vector2 mousePosition = playerControls.Combat.Aim.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = new Vector2(transform.position.x - mousePosition.x, transform.position.y - mousePosition.y);
        transform.up = -direction;
    }

    private void Fire()
    {
        if(attackButtonDown)
        {
            if(!isAttacking && !isOnCooldown)
            {
                isAttacking = true;
                Debug.Log("Fire!");
                isAttacking = false;
                StartCoroutine(AttackCooldownRoutine());
            }
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) {return;}
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) {return;}
    }
}
