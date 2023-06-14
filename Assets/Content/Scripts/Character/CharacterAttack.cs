using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : CharacterBehaviour
{
    [BoxGroup( "Attack" )]
    [SerializeField]
    private float launchPower = 20f;

    private Rigidbody rb;

    private Vector3 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.GetComponent<Hitzone>() )
        {
            velocity = rb.velocity;

            velocity.y = launchPower;

            rb.velocity = velocity;
        }
    }
}
