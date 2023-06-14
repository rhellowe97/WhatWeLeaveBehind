using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class PhysicsObject : MonoBehaviour, IResetable
{
    [SerializeField]
    private float gravityScale = 1f;

    [SerializeField]
    private bool useReset = true;

    private Vector3 startPosition;

    private Quaternion startRotation;

    private Rigidbody rb;

    private HashSet<ForceArea> forceAreas = new HashSet<ForceArea>();

    [BoxGroup( "Audio" )]
    [SerializeField]
    private bool handleImpact = true;

    [BoxGroup( "Audio" ), ShowIf( "handleImpact" )]
    [SerializeField]
    private float impactSoundThreshold = 4f;

    [BoxGroup( "Audio" ), ShowIf( "handleImpact" )]
    [SerializeField]
    private SoundHandler impactHandler;

    void Awake()
    {
        startPosition = transform.position;

        startRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        rb.AddForce( Vector3.down * rb.mass * 9.81f * gravityScale );

        foreach ( ForceArea forceArea in forceAreas )
        {
            if ( !forceArea.Active )
                continue;

            Vector3 forceVector = forceArea.Direction.normalized * ( forceArea.ExternalAcceleration - ( forceArea.FallOffPerUnit * ( transform.position - forceArea.transform.position ).magnitude ) ) * Time.fixedDeltaTime;

            if ( Mathf.Sign( forceVector.y ) != Mathf.Sign( rb.velocity.y ) )
            {
                forceVector.y *= 2f;
            }

            rb.velocity += forceVector;
        }
    }

    public void ResetObject()
    {
        if ( !useReset )
            return;

        transform.position = startPosition;

        transform.rotation = startRotation;

        rb.velocity = Vector3.zero;
    }

    public void ApplyExternalForce( Vector3 force, ForceMode mode = ForceMode.Force )
    {
        rb.AddForce( force, mode );
    }

    public Vector3 GetCOM()
    {
        return rb.worldCenterOfMass;
    }

    private void OnCollisionEnter( Collision collision )
    {
        if ( handleImpact && impactHandler != null && SoundManager.Instance != null && rb.velocity.sqrMagnitude > impactSoundThreshold * impactSoundThreshold )
        {
            impactHandler.TryPlaySoundClip( collision.contacts[0].point );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.GetComponent<FallDeath>() != null )
        {
            ResetObject();

            return;
        }

        ForceArea forceArea = other.GetComponent<ForceArea>();

        if ( forceArea != null && !forceAreas.Contains( forceArea ) )
        {
            forceAreas.Add( forceArea );
        }
    }


    private void OnTriggerExit( Collider other )
    {
        ForceArea forceArea = other.GetComponent<ForceArea>();

        if ( forceArea != null && forceAreas.Contains( forceArea ) )
        {
            forceAreas.Remove( forceArea );
        }
    }
}
