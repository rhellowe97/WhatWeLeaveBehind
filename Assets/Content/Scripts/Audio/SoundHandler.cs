using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField]
    private float soundCooldown;

    private float soundTimer = 0f;

    [SerializeField]
    private List<AudioClip> soundClips = new List<AudioClip>();

    private SoundInstance currentLoopInstance;

    public void TryPlaySoundClip( Vector3 desiredPosition )
    {
        if ( soundTimer >= soundCooldown && SoundManager.Instance != null && soundClips.Count > 0 )
        {
            SoundManager.Instance.PlaySoundInstance( soundClips[Random.Range( 0, soundClips.Count )], desiredPosition );

            soundTimer = 0f;
        }
    }

    public void TryGetLoopClip( Vector3 desiredPosition, Transform parent )
    {
        if ( soundTimer >= soundCooldown && SoundManager.Instance != null && soundClips.Count > 0 )
        {
            currentLoopInstance = SoundManager.Instance.PlayLoopedInstance( soundClips[0], desiredPosition, parent );

            soundTimer = 0f;
        }
    }

    public void StopLoopClip()
    {
        if ( currentLoopInstance != null )
        {
            currentLoopInstance.StopLooped();

            currentLoopInstance = null;
        }
    }

    private void Update()
    {
        if ( soundTimer < soundCooldown )
            soundTimer += Time.deltaTime;
    }
}
