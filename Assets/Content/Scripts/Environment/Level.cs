using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level Instance;

    [SerializeField]
    protected List<Checkpoint> checkpoints;
    public List<Checkpoint> Checkpoints => checkpoints;

    [SerializeField]
    protected CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera PlayerCamera => playerCamera;

    private int checkpointIndex = 0;

    public LevelSegment ActiveSegment;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );

            return;
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        foreach ( Checkpoint point in checkpoints )
        {
            point.OnCheckpointPassed += CheckpointPassed;
        }
    }

    private void OnDisable()
    {
        foreach ( Checkpoint point in checkpoints )
        {
            point.OnCheckpointPassed -= CheckpointPassed;
        }
    }

    public Vector3 GetCurrentCheckpoint()
    {
        return ActiveSegment.LevelSegmentEntries[0].transform.position;
    }

    private void CheckpointPassed( Checkpoint point )
    {
        for ( int i = 0; i < checkpoints.Count; i++ )
        {
            if ( checkpoints[i] == point )
            {
                checkpointIndex = i;

                break;
            }
        }
    }

    [Button]
    private void FindAllCheckpoints()
    {
        checkpoints = FindObjectsOfType<Checkpoint>().ToList();
    }
}
