using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float deathTimer = 2f;
    void Start()
    {
        Destroy(gameObject, deathTimer);
    }
}
