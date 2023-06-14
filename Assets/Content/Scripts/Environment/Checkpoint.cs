using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public delegate void CheckpointUpdate( Checkpoint point );
    public event CheckpointUpdate OnCheckpointPassed;

    [SerializeField]
    private Collider trigger;

    [SerializeField]
    private ParticleSystem checkpointActiveFX;

    [SerializeField]
    private LevelSegment segment;
    public LevelSegment Segment => segment;

    private void OnTriggerEnter( Collider other )
    {
        if ( other.GetComponent<CharacterController>() != null )
        {
            OnCheckpointPassed?.Invoke( this );

            if ( trigger != null )
            {
                trigger.enabled = false;
            }

            if ( checkpointActiveFX != null )
            {
                checkpointActiveFX.Play();
            }
        }
    }
}
