using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class LevelSegment : MonoBehaviour
{
    [SerializeField]
    protected List<LevelSegmentEntry> levelSegmentEntries = new List<LevelSegmentEntry>();
    public List<LevelSegmentEntry> LevelSegmentEntries => levelSegmentEntries;

    [SerializeField]
    private CinemachineVirtualCamera segmentCamera;

    [SerializeField]
    private Transform cameraConfiner;

    [SerializeField]
    private bool ActiveOnAwake = false;

    private CinemachineFramingTransposer transposer;

    private float camStartDistance;

    private void Start()
    {
        Transform player = FindObjectOfType<CharacterController>().transform;

        segmentCamera.m_Follow = player;

        transposer = segmentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        camStartDistance = transposer.m_CameraDistance;

        if ( ActiveOnAwake )
            SegmentTriggered();

        foreach ( LevelSegmentEntry entry in levelSegmentEntries )
        {
            entry.OnEntryTriggered += SegmentTriggered;
        }
    }

    public void OffsetScreenBias( float yInput )
    {
        transposer.m_CameraDistance = camStartDistance - 5 * yInput;
    }

    private void SegmentTriggered()
    {
        if ( Level.Instance.ActiveSegment == this )
            return;

        UIManager.Instance.ObscureScreen( true, 0.4f, () =>
         {
             ToggleActive( true );

             if ( Level.Instance.ActiveSegment != null )
             {
                 Level.Instance.ActiveSegment.ToggleActive( false );
             }

             Level.Instance.ActiveSegment = this;

             UIManager.Instance.ObscureScreen( false );
         } );
    }

    public void ToggleActive( bool toggle )
    {
        OffsetScreenBias( 0 );

        segmentCamera.enabled = toggle;

        segmentCamera.OnTargetObjectWarped( segmentCamera.m_Follow, ( segmentCamera.m_Follow.position - transform.position ) );
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if ( cameraConfiner != null && ( Selection.activeObject == gameObject || Selection.activeObject == cameraConfiner.gameObject ) )
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube( transform.position + transform.right * cameraConfiner.localScale.x / 2, cameraConfiner.localScale );
        }
#endif
    }
}
