using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( AudioSource ) )]
public class SoundInstance : MonoBehaviour
{
    public delegate void ClipComplete( SoundInstance completedInstance );
    public event ClipComplete OnClipComplete;

    public AudioSource Source { get; private set; }

    private void Awake()
    {
        Source = GetComponent<AudioSource>();
    }

    public void PlaySingle()
    {
        StartCoroutine( PlayAudioClip() );
    }

    public void PlayLoop()
    {
        Source.Play();
    }

    public void StopLooped()
    {
        Source.Stop();

        OnClipComplete?.Invoke( this );
    }

    private IEnumerator PlayAudioClip()
    {
        Source.Play();

        yield return null;

        while ( Source.isPlaying )
            yield return null;

        OnClipComplete?.Invoke( this );
    }
}
