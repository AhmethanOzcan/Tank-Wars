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
    float muzzleFlashTimer = 0f;

    [Header("Variables")]
    [SerializeField] float attackCooldown = .5f;
    [SerializeField] float muzzleFlashDuration = .1f;
    
    [Header("References")]
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject clientProjectilePrefab;
    [SerializeField] Transform projectileSpawnTransform;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Collider2D playerCollider;
    
    private void Awake() {
        playerControls = new PlayerControls();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) {return;}
        playerControls.Combat.PrimaryFire.started += _ => StartAttacking();
        playerControls.Combat.PrimaryFire.canceled += _ => StopAttacking();
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) {return;}
        playerControls.Combat.PrimaryFire.started -= _ => StartAttacking();
        playerControls.Combat.PrimaryFire.canceled -= _ => StopAttacking();
    }
    private void FixedUpdate()
    {
        MuzzleFlashHandler();

        if (!IsOwner) { return; }
        FaceMouse();
        Fire();
    }

    private void MuzzleFlashHandler()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlash.SetActive(true);
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0)
            {
                muzzleFlash.SetActive(false);
            }
        }
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
                isOnCooldown = true;
                muzzleFlashTimer = muzzleFlashDuration;
                SpawnServerRpc(projectileSpawnTransform.position, this.transform.up);
                SpawnDummyProjectile(projectileSpawnTransform.position, this.transform.up);
                isAttacking = false;
                StartCoroutine(AttackCooldownRoutine());
            }
        }
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(
                            clientProjectilePrefab,
                            spawnPos,
                            Quaternion.identity
                            );
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
    }

    [ServerRpc]
    private void SpawnServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(
                            serverProjectilePrefab,
                            spawnPos,
                            Quaternion.identity
                            );
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        
        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }
        
        SpawnClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if(IsOwner) return;

        SpawnDummyProjectile(spawnPos, direction);
    }

    private IEnumerator AttackCooldownRoutine()
    {
        
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

}
