using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class CharacterCore : MonoBehaviour
{
    [SerializeField]
    protected Animator anim;
    public Animator Animator => anim;

    [SerializeField]
    protected SkinnedMeshRenderer playerRenderer;
    public SkinnedMeshRenderer PlayerRenderer => playerRenderer;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private ParticleSystem dustSystem;

    public Rigidbody Rigidbody { get; private set; }

    public bool Grounded => GroundedCheck();

    private HashSet<Collider> contacts = new HashSet<Collider>();

    private bool lastGrounded = false;

    private bool rayContact = false;

    [SerializeField]
    private SoundHandler landingSoundHandler;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void UpdateVelocity( Vector3 newVel )
    {
        Rigidbody.velocity = newVel;
    }

    private void FixedUpdate()
    {
        rayContact = Physics.Raycast( transform.position + Vector3.up * 0.2f, Vector3.down, 0.5f, groundLayer, QueryTriggerInteraction.Ignore );

        if ( Animator != null )
        {
            Animator.SetBool( "Grounded", Grounded );
        }

        if ( dustSystem != null )
        {
            if ( Grounded && !dustSystem.isPlaying )
            {
                dustSystem.Play();
            }
            else if ( !Grounded && dustSystem.isPlaying )
            {
                dustSystem.Stop();
            }
        }

        if ( Grounded && !lastGrounded && landingSoundHandler != null )
        {
            landingSoundHandler.TryPlaySoundClip( transform.position );
        }

        lastGrounded = Grounded;
    }

    private void OnCollisionEnter( Collision collision )
    {
        if ( ( collision.contacts[0].point.y < transform.position.y + 0.3f ) && !contacts.Contains( collision.collider ) )
        {
            contacts.Add( collision.collider );
        }
    }

    private void OnCollisionExit( Collision collision )
    {
        if ( contacts.Contains( collision.collider ) )
        {
            contacts.Remove( collision.collider );
        }
    }

    private bool GroundedCheck()
    {
        foreach ( Collider col in contacts )
        {
            if ( !col.gameObject.activeInHierarchy )
            {
                contacts.Remove( col );
            }
        }
        return contacts.Count > 0 || rayContact;
    }

    public void ZeroCharacter()
    {
        Rigidbody.velocity = Vector3.zero;
    }
}
