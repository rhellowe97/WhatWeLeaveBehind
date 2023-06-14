using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitzone : MonoBehaviour
{
    private Enemy enemy;

    private Collider col;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();

        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter( Collider other )
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<CharacterController>())
        {
            enemy.Death();

            col.enabled = false;
        }
    }
}
