using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private void OnCollisionEnter( Collision collision )
    {
        Debug.Log( "MOVING COLLISION DETECTED" );
        if ( collision.collider.attachedRigidbody != null )
        {
            PhysicsObject physObj = collision.collider.attachedRigidbody.GetComponent<PhysicsObject>();

            if ( physObj != null )
                physObj.transform.SetParent( transform );
        }
    }

    private void OnCollisionExit( Collision collision )
    {
        if ( collision.collider.attachedRigidbody != null )
        {
            PhysicsObject physObj = collision.collider.attachedRigidbody.GetComponent<PhysicsObject>();

            if ( physObj != null )
                physObj.transform.SetParent( null );
        }
    }
}
