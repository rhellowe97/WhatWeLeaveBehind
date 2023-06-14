using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( LineRenderer ) )]
public class Rope : MonoBehaviour
{
    [SerializeField]
    private float ropeLength = 8f;

    [SerializeField]
    private float jointDensity = 0.5f;

    [SerializeField]
    private Rigidbody grappleNode;

    [SerializeField]
    private GameObject ropeJointPrefab;

    [SerializeField]
    private List<Rigidbody> joints = new List<Rigidbody>();

    private LineRenderer rope;

    private void Awake()
    {
        rope = GetComponent<LineRenderer>();

        rope.positionCount = joints.Count + 1;
    }

    private void FixedUpdate()
    {
        rope.SetPosition( 0, grappleNode.transform.position );

        for ( int i = 0; i < joints.Count; i++ )
        {
            rope.SetPosition( i + 1, joints[i].position );
        }
    }

    [Button]
    public void GenerateRope()
    {
        int totalJointCount = Mathf.RoundToInt( ropeLength / jointDensity );

        foreach ( Rigidbody joint in joints )
        {
            Destroy( joint.gameObject );
        }

        joints.Clear();

        for ( int i = 0; i < totalJointCount; i++ )
        {
            HingeJoint newJoint = Instantiate( ropeJointPrefab, grappleNode.transform.position + Vector3.down * i * jointDensity, Quaternion.identity ).GetComponent<HingeJoint>();

            if ( i == 0 )
            {
                newJoint.connectedBody = grappleNode;
            }
            else
            {
                newJoint.connectedBody = joints[joints.Count - 1];
            }

            joints.Add( newJoint.GetComponent<Rigidbody>() );
        }

        rope = GetComponent<LineRenderer>();

        rope.positionCount = joints.Count + 1;

        rope.SetPosition( 0, grappleNode.transform.position );

        for ( int i = 0; i < joints.Count; i++ )
        {
            rope.SetPosition( i + 1, joints[i].position );
        }
    }
}
