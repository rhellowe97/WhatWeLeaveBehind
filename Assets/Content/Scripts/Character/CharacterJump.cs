using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterCore ) )]
public class CharacterJump : CharacterBehaviour
{
    public delegate void JumpSuccess();
    public event JumpSuccess OnJumpSuccess;

    [BoxGroup( "Jump" )]
    [SerializeField]
    private float jumpPower = 20f;

    [BoxGroup( "Jump" )]
    [SerializeField]
    private float jumpHoldPower = 5f;

    [SerializeField]
    private ParticleSystem jumpFX;

    private bool jump = false;

    private bool jumpHold = false;

    private CharacterCore character;

    private Vector3 velocity = Vector3.zero;

    private Vector2 moveInput = Vector2.zero;

    private void Awake()
    {
        character = GetComponent<CharacterCore>();
    }

    public override void SimulateBehaviour()
    {
        velocity = character.Rigidbody.velocity;

        if ( jump )
        {
            if ( character.Grounded )
            {
                if ( jumpFX != null )
                    jumpFX.Play();

                velocity.y = jumpPower;

                if ( character.Animator != null )
                    character.Animator.SetTrigger( "Jump" );

                OnJumpSuccess?.Invoke();
            }

            jump = false;
        }

        if ( jumpHold )
        {
            velocity.y += jumpHoldPower * Time.fixedDeltaTime;
        }

        character.UpdateVelocity( velocity );
    }

    public void SetJump( bool setHold = true )
    {
        jump = true;

        jumpHold = setHold;
    }

    public void JumpRelease()
    {
        jumpHold = false;
    }

    private void OnTriggerEnter( Collider other )
    {
        BouncePad bouncePad = other.GetComponent<BouncePad>();

        if ( bouncePad != null )
        {
            velocity = character.Rigidbody.velocity;

            velocity.y = bouncePad.BouncePower;

            character.UpdateVelocity( velocity );

            if ( character.Animator != null )
                character.Animator.SetTrigger( "Jump" );

            jump = false;
        }
    }
}
