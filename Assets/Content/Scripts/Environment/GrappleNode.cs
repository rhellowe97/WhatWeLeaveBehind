using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrappleNode : MonoBehaviour
{
    private ConfigurableJoint grappleJoint;

    private void Awake()
    {
        grappleJoint = GetComponent<ConfigurableJoint>();
    }

    public void SetConnectedBody( Rigidbody body )
    {
        if ( grappleJoint != null )
            grappleJoint.connectedBody = body;
    }
}
