using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : PuzzleSwitch, IInteractable
{
    private Quaternion startState;

    private Quaternion activeState;

    [SerializeField]
    private float switchSpeed;

    [SerializeField]
    private Transform leverHead;

    [SerializeField]
    private bool StartActive = false;

    [SerializeField]
    private Renderer activeLightRend;

    private MaterialPropertyBlock mpb;

    private Coroutine switchCo;

    // Start is called before the first frame update
    void Start()
    {
        startState = leverHead.localRotation;

        Vector3 localEulerStart = leverHead.localEulerAngles;

        Vector3 localEulerActive = localEulerStart;

        if ( localEulerActive.x > 180 )
            localEulerActive.x -= 360;

        localEulerActive.x += ( 2 * Mathf.Abs( localEulerActive.x ) );

        activeState = Quaternion.Euler( localEulerActive );

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

        switchCo = StartCoroutine( ToggleLever( Activated ) );
    }

    private IEnumerator ToggleLever( bool isOn )
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

            if ( isOn )
                leverHead.localRotation = Quaternion.Lerp( startState, activeState, t / switchSpeed );
            else
                leverHead.localRotation = Quaternion.Lerp( activeState, startState, t / switchSpeed );

            t += Time.deltaTime;

            yield return null;
        }

        switchCo = null;
    }
}
