using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterCore ) )]
public class AIBrain : MonoBehaviour
{
    [SerializeField]
    protected float playerAwarenessDistance;

    protected CharacterController player;

    protected CharacterCore character;

    protected float sqrPlayerDistance = 0;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();

        character = GetComponent<CharacterCore>();
    }

    protected virtual void AIUpdate() { }

    void FixedUpdate()
    {
        sqrPlayerDistance = ( player.transform.position - transform.position ).sqrMagnitude;

        AIUpdate();
    }
}
