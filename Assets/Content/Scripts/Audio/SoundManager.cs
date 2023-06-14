using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private Queue<SoundInstance> availableInstances = new Queue<SoundInstance>();

    [SerializeField]
    private int prewarmCount = 10;

    [SerializeField]
    private GameObject soundInstancePrefab;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );

            return;
        }

        Instance = this;

        DontDestroyOnLoad( gameObject );

        for ( int i = 0; i < prewarmCount; i++ )
        {
            SoundInstance newInstance = Instantiate( soundInstancePrefab ).GetComponent<SoundInstance>();

            newInstance.OnClipComplete += ReturnToPool;

            newInstance.transform.SetParent( transform );
        }
    }

    public void PlaySoundInstance( AudioClip clipToPlay, Vector3 desiredPosition )
    {
        SoundInstance newInstance;

        if ( availableInstances.Count == 0 )
        {
            newInstance = Instantiate( soundInstancePrefab ).GetComponent<SoundInstance>();

            newInstance.OnClipComplete += ReturnToPool;
        }
        else
        {
            newInstance = availableInstances.Dequeue();
        }

        newInstance.Source.clip = clipToPlay;

        newInstance.transform.position = desiredPosition;

        newInstance.PlaySingle();
    }

    public SoundInstance PlayLoopedInstance( AudioClip clipToPlay, Vector3 desiredPosition, Transform parent )
    {
        SoundInstance newInstance;

        if ( availableInstances.Count == 0 )
        {
            newInstance = Instantiate( soundInstancePrefab ).GetComponent<SoundInstance>();

            newInstance.OnClipComplete += ReturnToPool;
        }
        else
        {
            newInstance = availableInstances.Dequeue();
        }

        newInstance.Source.clip = clipToPlay;

        newInstance.transform.position = desiredPosition;

        if ( parent != null )
            newInstance.transform.SetParent( parent );

        newInstance.PlayLoop();

        return newInstance;
    }

    private void ReturnToPool( SoundInstance completedInstance )
    {
        completedInstance.transform.SetParent( transform );

        availableInstances.Enqueue( completedInstance );
    }
}
