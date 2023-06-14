using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent( typeof( CinemachineVirtualCamera ) )]
public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    private CinemachineVirtualCamera playerCamera;

    private CinemachineFramingTransposer transposer;

    private CinemachineBasicMultiChannelPerlin cameraNoise;

    private float startCameraDistance = 15f;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );

            return;
        }

        Instance = this;

        playerCamera = GetComponent<CinemachineVirtualCamera>();

        transposer = playerCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        startCameraDistance = transposer.m_CameraDistance;

        cameraNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        transform.SetParent( null );
    }

    public void OffsetScreenBias( float yInput )
    {
        transposer.m_CameraDistance = startCameraDistance - 5 * yInput;
    }

    public void ScreenShake( float intenstity, float duration )
    {
        StartCoroutine( ProcessShake( intenstity, duration ) );
    }

    private IEnumerator ProcessShake( float shakeIntensity = 5f, float duration = 0.5f )
    {
        float t = 0;

        while ( t < duration )
        {
            Noise( 1, Mathf.Lerp( shakeIntensity, 0, t / duration ) );

            t += Time.deltaTime;

            yield return null;
        }

        Noise( 0, 0 );
    }

    public void Noise( float amplitudeGain, float frequencyGain )
    {
        cameraNoise.m_AmplitudeGain = amplitudeGain;

        cameraNoise.m_FrequencyGain = frequencyGain;
    }
}
