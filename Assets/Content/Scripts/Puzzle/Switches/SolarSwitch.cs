using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSwitch : PuzzleSwitch
{
    [SerializeField]
    private float changeSpeed = 1f;

    [SerializeField]
    private Renderer chargeRend;

    private MaterialPropertyBlock mpb;

    private Coroutine glowChangeCo;

    private void Start()
    {
        mpb = new MaterialPropertyBlock();

        glowChangeCo = StartCoroutine( UpdateCharge( Activated ) );
    }

    public override void ChangeState( bool state )
    {
        base.ChangeState( state );

        if ( glowChangeCo != null )
            StopCoroutine( glowChangeCo );

        glowChangeCo = StartCoroutine( UpdateCharge( state ) );
    }

    private IEnumerator UpdateCharge( bool isOn )
    {
        float t = 0;

        while ( t < changeSpeed )
        {
            if ( isOn )
            {
                mpb.SetColor( "_BaseColor", Color.Lerp( colorData.StartBaseColor, colorData.ChargedBaseColor, t / changeSpeed ) );
                mpb.SetColor( "_EmissionColor", Color.Lerp( colorData.StartEmissionColor, colorData.ChargedEmissionColor, t / changeSpeed ) );
            }
            else
            {
                mpb.SetColor( "_BaseColor", Color.Lerp( colorData.ChargedBaseColor, colorData.StartBaseColor, t / changeSpeed ) );
                mpb.SetColor( "_EmissionColor", Color.Lerp( colorData.ChargedEmissionColor, colorData.StartEmissionColor, t / changeSpeed ) );
            }

            chargeRend.SetPropertyBlock( mpb );

            t += Time.deltaTime;

            yield return null;
        }

        glowChangeCo = null;
    }
}
