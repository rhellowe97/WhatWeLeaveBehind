using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( CharacterCore ) )]
public class CharacterBoost : CharacterBehaviour
{
    [SerializeField]
    protected Vector3 scaleAdjustment = Vector3.one;

    [SerializeField]
    protected float transformationTime = 2f;

    [SerializeField]
    protected float maxBoost = 100f;

    [SerializeField]
    protected float boostBurnRate = 5f;

    [SerializeField]
    protected float groundShakeAirTime = 1f;

    [SerializeField]
    protected Color boostedColor = Color.green;

    [BoxGroup( "FX" )]
    [SerializeField]
    protected ParticleSystem landSystem;

    [BoxGroup( "FX" )]
    [SerializeField]
    protected List<ParticleSystem> boostSystems;

    private float airborneTimer = 0f;

    private float currentBoost = 0f;

    private bool boostActive;

    private bool transforming = false;

    private CharacterCore characterCore;

    private CharacterController characterController;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        characterCore = GetComponent<CharacterCore>();

        characterController = GetComponent<CharacterController>();

        mpb = new MaterialPropertyBlock();

        currentBoost = 100f;
    }

    private void Update()
    {
        if ( !characterCore.Grounded && airborneTimer < groundShakeAirTime )
        {
            airborneTimer += Time.deltaTime;
        }
        else if ( characterCore.Grounded && boostActive )
        {
            if ( airborneTimer >= groundShakeAirTime )
            {
                PlayerCamera.Instance.ScreenShake( 5, 0.5f );
            }

            airborneTimer = 0f;
        }

        if ( boostActive )
        {
            currentBoost -= boostBurnRate * Time.deltaTime;

            if ( currentBoost <= 0 )
            {
                currentBoost = 0;

                ToggleBoost( false );
            }
        }
    }

    public void UpdateBoost( float boostDelta )
    {
        currentBoost += boostDelta;
    }

    public void ToggleBoost( bool toggle = true )
    {
        if ( !transforming )
        {
            if ( !boostActive && currentBoost <= 0 )
                return;

            boostActive = !boostActive;

            if ( !toggle )
                boostActive = false;

            StartCoroutine( ToggleBoostRoutine( boostActive ) );
        }
    }

    private IEnumerator ToggleBoostRoutine( bool boostUp )
    {
        transforming = true;

        float t = 0;

        Color c = Color.white;

        if ( boostUp )
        {
            foreach ( ParticleSystem boost in boostSystems )
                boost.Play();

            while ( t < transformationTime )
            {
                transform.localScale = Vector3.Lerp( Vector3.one, Vector3.one + scaleAdjustment, t / transformationTime );

                c = Color.Lerp( Color.white, boostedColor, t / transformationTime );

                mpb.SetColor( "_BaseColor", c );

                characterCore.PlayerRenderer.SetPropertyBlock( mpb );

                t += Time.deltaTime;

                yield return null;
            }
        }
        else
        {
            foreach ( ParticleSystem boost in boostSystems )
                boost.Stop();

            while ( t < transformationTime )
            {
                transform.localScale = Vector3.Lerp( Vector3.one + scaleAdjustment, Vector3.one, t / transformationTime );

                c = Color.Lerp( boostedColor, Color.white, t / transformationTime );

                mpb.SetColor( "_BaseColor", c );

                characterCore.PlayerRenderer.SetPropertyBlock( mpb );

                t += Time.deltaTime;

                yield return null;
            }
        }

        transforming = false;

        boostActive = boostUp;
    }
}
