using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    Rigidbody2D myRigidbody;

    private void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        myRigidbody.velocity = myRigidbody.transform.up * speed;
    }

}
