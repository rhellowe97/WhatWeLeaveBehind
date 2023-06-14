using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSource : SolarProjector
{
    [SerializeField]
    private float sourceLightDistance = 10f;

    private HashSet<PhysicsObject> validObjects = new HashSet<PhysicsObject>();

    private void Start()
    {
        currentLightDistance = sourceLightDistance;

        PerformLightCast( Vector3.zero, sourceLightDistance );
    }

    private void FixedUpdate()
    {
        if ( validObjects.Count > 0 )
        {
            PerformLightCast( Vector3.zero, sourceLightDistance );
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        PhysicsObject physObject = other.attachedRigidbody.GetComponent<PhysicsObject>();

        if ( physObject != null && !validObjects.Contains( physObject ) )
        {
            validObjects.Add( physObject );
        }
    }

    private void OnTriggerExit( Collider other )
    {
        PhysicsObject physObject = other.attachedRigidbody.GetComponent<PhysicsObject>();

        if ( physObject != null && validObjects.Contains( physObject ) )
        {
            validObjects.Remove( physObject );

            if ( validObjects.Count == 0 )
            {
                PerformLightCast( Vector3.zero, sourceLightDistance );
            }
        }


    }
}
