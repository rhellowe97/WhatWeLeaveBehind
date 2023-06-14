using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ChargePad : PuzzleSwitch
{
    [SerializeField]
    private Transform button;

    [SerializeField]
    private float buttonChangeSpeed = 1f;

    [SerializeField]
    private Renderer chargeRend;

    [SerializeField]
    private Transform keyHelper;

    [SerializeField]
    private float keyHoldForce;

    private Battery currentKey = null;

    private MaterialPropertyBlock mpb;

    private void Start()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( currentKey == null )
        {
            Battery possibleKey = other.GetComponent<Battery>();

            if ( possibleKey != null )
            {
                currentKey = possibleKey;

                ChangeState( true );

                StartCoroutine( UpdateCharge( true ) );
            }
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( currentKey != null )
        {
            Battery possibleKey = other.GetComponent<Battery>();

            if ( possibleKey != null && currentKey == possibleKey )
            {
                currentKey = null;

                ChangeState( false );

                StartCoroutine( UpdateCharge( false ) );
            }
        }
    }

    private void FixedUpdate()
    {
        if ( currentKey != null )
        {
            Vector3 keyDiff = ( keyHelper.position - currentKey.transform.position );

            if ( keyDiff.sqrMagnitude > 1 )
                keyDiff.Normalize();

            currentKey.Rigidbody.AddForce( keyDiff * keyHoldForce );
        }
    }

    private IEnumerator UpdateCharge( bool isOn )
    {
        float t = 0;

        while ( t < buttonChangeSpeed )
        {
            if ( isOn )
            {
                mpb.SetColor( "_BaseColor", Color.Lerp( colorData.StartBaseColor, colorData.ChargedBaseColor, t / buttonChangeSpeed ) );
                mpb.SetColor( "_EmissionColor", Color.Lerp( colorData.StartEmissionColor, colorData.ChargedEmissionColor, t / buttonChangeSpeed ) );
            }
            else
            {
                mpb.SetColor( "_BaseColor", Color.Lerp( colorData.ChargedBaseColor, colorData.StartBaseColor, t / buttonChangeSpeed ) );
                mpb.SetColor( "_EmissionColor", Color.Lerp( colorData.ChargedEmissionColor, colorData.StartEmissionColor, t / buttonChangeSpeed ) );
            }

            chargeRend.SetPropertyBlock( mpb );

            t += Time.deltaTime;

            yield return null;
        }
    }

}
