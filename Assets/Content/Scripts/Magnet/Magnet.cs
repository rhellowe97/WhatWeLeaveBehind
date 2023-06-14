using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Magnet : MonoBehaviour
{
    [SerializeField]
    private Magnet connectedMagnet;

    [SerializeField]
    private float magnetFalloffRadius = 0.25f;

    [SerializeField]
    private float magnetStrength = 10f;

    [SerializeField]
    private float maxBodyVelocity = 10f;

    [SerializeField]
    private float fireSpeed = 5f;

    [SerializeField]
    private float pairSpeed = 0.5f;

    [SerializeField]
    private LayerMask hittableLayerMask;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private Image magnetUI;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private LineRenderer pairBeam;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private TrailRenderer fireTrail;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private List<Renderer> magnetRenderers;

    [BoxGroup( "Visuals" )]
    [SerializeField, ColorUsage( true, true )]
    private Color pullColor;

    [BoxGroup( "Visuals" )]
    [SerializeField, ColorUsage( true, true )]
    private Color pushColor;

    [BoxGroup( "Visuals" )]
    [SerializeField, ColorUsage( true, true )]
    private Color freezeColor;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private Transform effectParent;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private ParticleSystem pullEffect;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private ParticleSystem pushEffect;

    [BoxGroup( "Visuals" )]
    [SerializeField]
    private ParticleSystem freezeEffect;

    private bool beamOrigin = false;

    private MaterialPropertyBlock mpb;

    public Rigidbody CurrentReferenceBody { get; private set; }

    private Rigidbody rb;

    private bool hasParentBody = false;

    private Coroutine colorUpdateRoutine;

    private bool isFiring = false;

    private Vector3 fireDirection;

    private Vector3 connectionDirection;

    public bool Paired { get; private set; } = false;

    private Coroutine pairCo;


    private void Awake()
    {
        transform.SetParent( null );
        transform.rotation = Quaternion.identity;

        mpb = new MaterialPropertyBlock();

        rb = GetComponent<Rigidbody>();
    }

    public void TogglePolarState( PolarState state, bool isActive )
    {
        if ( colorUpdateRoutine != null )
            StopCoroutine( colorUpdateRoutine );

        if ( state == PolarState.Pull )
        {
            if ( gameObject.activeInHierarchy )
                colorUpdateRoutine = StartCoroutine( UpdateColor( pullColor ) );
            pushEffect.Stop();

            if ( isActive )
                pullEffect.Play();
        }
        else
        {
            if ( gameObject.activeInHierarchy )
                colorUpdateRoutine = StartCoroutine( UpdateColor( pushColor ) );
            pullEffect.Stop();

            if ( isActive )
                pushEffect.Play();
        }
    }

    public void ToggleEffect( PolarState state, bool down )
    {
        if ( state == PolarState.Pull )
        {
            if ( down )
                pullEffect.Play();
            else
                pullEffect.Stop();
        }
        else
        {
            if ( down )
                pushEffect.Play();
            else
                pushEffect.Stop();
        }
    }

    public void ApplyPolarForce( PolarState state )
    {
        if ( hasParentBody && Paired )
        {
            float currentMagnetStrength = magnetStrength;

            if ( ( connectedMagnet.transform.position - transform.position ).sqrMagnitude < magnetFalloffRadius )
            {
                currentMagnetStrength = ( ( connectedMagnet.transform.position - transform.position ).sqrMagnitude / magnetFalloffRadius ) * magnetStrength;
            }

            CurrentReferenceBody.AddForceAtPosition( connectionDirection.normalized * currentMagnetStrength * ( state == PolarState.Pull ? 1 : -1 ), transform.position );

            CurrentReferenceBody.velocity = Vector3.ClampMagnitude( CurrentReferenceBody.velocity, maxBodyVelocity );
        }
    }

    public void TogglePair( PolarState state, bool turnOn, bool frozen )
    {
        if ( colorUpdateRoutine != null )
            StopCoroutine( colorUpdateRoutine );

        if ( frozen )
        {
            colorUpdateRoutine = StartCoroutine( UpdateColor( freezeColor, turnOn ) );
        }
        else if ( state == PolarState.Pull )
        {
            colorUpdateRoutine = StartCoroutine( UpdateColor( pullColor, turnOn ) );
        }
        else if ( state == PolarState.Push )
        {
            colorUpdateRoutine = StartCoroutine( UpdateColor( pushColor, turnOn ) );
        }

    }

    private IEnumerator UpdateColor( Color toColor, bool on = true )
    {
        float t = 0;

        Color fromColor = mpb.GetColor( "_EmissionColor" );

        Color UIColor = toColor;

        if ( !on )
            toColor *= 0.4f;

        while ( t < 0.5f )
        {
            mpb.SetColor( "_BaseColor", Color.Lerp( fromColor, toColor, t / 0.5f ) );
            mpb.SetColor( "_EmissionColor", Color.Lerp( fromColor, toColor, t / 0.5f ) );

            magnetUI.color = Color.Lerp( fromColor, UIColor, t / 0.5f );

            foreach ( Renderer rend in magnetRenderers )
                rend.SetPropertyBlock( mpb );

            t += Time.deltaTime;

            yield return null;
        }
    }

    public void UIIndicatorColor( Color toColor )
    {
        magnetUI.DOColor( toColor, 0.5f );
    }

    public void FireMagnet( Vector3 normalizedDirection, bool isBeamOrigin = false )
    {
        if ( pairCo != null )
        {
            StopCoroutine( pairCo );
        }

        Paired = false;

        pairBeam.enabled = false;

        fireTrail.enabled = true;

        isFiring = true;

        fireDirection = normalizedDirection;

        beamOrigin = isBeamOrigin;
    }

    public void FreezeMagnet( bool toggle, PolarState state )
    {
        if ( colorUpdateRoutine != null )
            StopCoroutine( colorUpdateRoutine );

        if ( toggle )
        {
            colorUpdateRoutine = StartCoroutine( UpdateColor( freezeColor ) );

            freezeEffect.Play();
            pullEffect.Stop();
            pushEffect.Stop();
        }
        else
        {
            freezeEffect.Stop();

            if ( state == PolarState.Pull )
            {
                colorUpdateRoutine = StartCoroutine( UpdateColor( pullColor ) );
            }
            else
            {
                colorUpdateRoutine = StartCoroutine( UpdateColor( pushColor ) );
            }
        }
    }

    public Color GetCurrentColor( PolarState state, bool frozen )
    {
        if ( frozen )
            return freezeColor;

        if ( state == PolarState.Pull )
            return pullColor;
        else
            return pushColor;
    }

    public void PairComplete()
    {
        Paired = true;
    }

    private void FixedUpdate()
    {
        if ( isFiring )
        {
            if ( Physics.Raycast( transform.position, fireDirection, out RaycastHit hit, 0.5f, hittableLayerMask, QueryTriggerInteraction.Ignore ) )
            {
                transform.SetParent( hit.collider.transform, true );

                transform.position = hit.point;

                Rigidbody parentBody = transform.parent.GetComponentInParent<Rigidbody>();

                if ( parentBody != null )
                {
                    CurrentReferenceBody = parentBody;

                    hasParentBody = true;
                }
                else
                {
                    CurrentReferenceBody = rb;

                    hasParentBody = false;
                }

                isFiring = false;

                if ( beamOrigin )
                {
                    pairBeam.enabled = true;
                    pairCo = StartCoroutine( ConnectMagnets() );
                }

                fireTrail.enabled = false;
            }

            rb.position += fireDirection * fireSpeed * Time.fixedDeltaTime;
        }
        else if ( beamOrigin && Paired )
        {

            pairBeam.SetPosition( 0, transform.position );
            pairBeam.SetPosition( 1, connectedMagnet.transform.position );
        }

        connectionDirection = ( connectedMagnet.transform.position - transform.position );

        effectParent.transform.rotation = Quaternion.LookRotation( connectionDirection, Vector3.up );
    }

    private IEnumerator ConnectMagnets()
    {
        float t = 0;

        while ( t < pairSpeed )
        {
            pairBeam.SetPosition( 0, transform.position );
            pairBeam.SetPosition( 1, Vector3.Lerp( transform.position, connectedMagnet.transform.position, t / pairSpeed ) );

            t += Time.deltaTime;

            yield return null;
        }

        Paired = true;

        connectedMagnet.PairComplete();

        pairCo = null;
    }
}
