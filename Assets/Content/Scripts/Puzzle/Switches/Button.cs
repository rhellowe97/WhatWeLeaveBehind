using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : PuzzleSwitch, IInteractable
{
    [SerializeField]
    private float switchSpeed;

    [SerializeField]
    private bool timed = false;

    [ShowIf( "timed" )]
    [SerializeField]
    private float timeActive = 8f;

    [SerializeField]
    private bool StartActive = false;

    [SerializeField]
    private Renderer activeLightRend;

    private MaterialPropertyBlock mpb;

    private Coroutine switchCo;

    // Start is called before the first frame update
    void Start()
    {
        mpb = new MaterialPropertyBlock();

        if ( StartActive )
            Interact();
    }


    [Button]
    public void Interact()
    {
        ChangeState( !Activated );

        if ( switchCo != null )
        {
            StopCoroutine( switchCo );
        }

        switchCo = StartCoroutine( ToggleButton( Activated ) );
    }

    private IEnumerator ToggleButton( bool isOn )
    {
        float t = 0;

        while ( t < switchSpeed )
        {
            if ( isOn )
            {
                mpb.SetColor( "_BaseColor", Color.Lerp( colorData.StartBaseColor, colorData.ChargedBaseColor, t / switchSpeed ) );
                mpb.SetColor( "_EmissionColor", Color.Lerp( colorData.StartEmissionColor, colorData.ChargedEmissionColor, t / switchSpeed ) );
            }
            else
            {
                mpb.SetColor( "_BaseColor", Color.Lerp( colorData.ChargedBaseColor, colorData.StartBaseColor, t / switchSpeed ) );
                mpb.SetColor( "_EmissionColor", Color.Lerp( colorData.ChargedEmissionColor, colorData.StartEmissionColor, t / switchSpeed ) );
            }

            activeLightRend.SetPropertyBlock( mpb );

            t += Time.deltaTime;

            yield return null;
        }

        if ( isOn )
        {
            t = 0;

            while ( t < timeActive )
            {
                t += Time.deltaTime;

                yield return null;
            }

            ChangeState( false );

            switchCo = StartCoroutine( ToggleButton( false ) );
        }
        else
        {
            switchCo = null;
        }
    }
}
