using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent( typeof( CharacterCore ) )]
public class CharacterMove : CharacterBehaviour
{
    [BoxGroup( "Movement" )]
    [SerializeField]
    private float acceleration = 5f;

    [BoxGroup( "Movement" )]
    [SerializeField]
    private float maxSpeed = 5f;

    [BoxGroup( "Movement" )]
    [SerializeField]
    private float drag = 1f;

    [BoxGroup( "Movement" )]
    [SerializeField]
    private float maxWheelSpeed = 720;

    [SerializeField]
    private Transform wheelHelper;

    private CharacterCore character;

    private Vector3 velocity = Vector2.zero;

    private Vector2 moveInput = Vector2.zero;

    private void Awake()
    {
        character = GetComponent<CharacterCore>();
    }

    public override void SimulateBehaviour()
    {
        velocity = character.Rigidbody.velocity;

        velocity.x = Mathf.Clamp( velocity.x + ( moveInput.x * acceleration * Time.fixedDeltaTime ) - Mathf.Clamp( Mathf.Sign( velocity.x ) * drag * Time.fixedDeltaTime, -Mathf.Abs( velocity.x ), Mathf.Abs( velocity.x ) ), -maxSpeed, maxSpeed );

        wheelHelper.Rotate( Vector3.right, ( velocity.x / maxSpeed ) * maxWheelSpeed * Time.fixedDeltaTime, Space.Self );

        character.UpdateVelocity( velocity );

        if ( character.Animator == null )
            return;

        character.Animator.SetFloat( "MoveX", Mathf.Abs( velocity.x ) / maxSpeed, 0.1f, Time.deltaTime );

        character.Animator.SetFloat( "MoveY", velocity.y );
    }

    public void SetMoveDirection( Vector2 currentMoveInput )
    {
        moveInput = currentMoveInput;
    }
}
