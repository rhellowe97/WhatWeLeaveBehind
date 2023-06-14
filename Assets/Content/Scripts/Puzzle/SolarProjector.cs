using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarProjector : MonoBehaviour
{
    protected LineRenderer solarRay;

    protected RaycastHit rayHit;

    protected Collider currentContact;

    protected RefractorPort currentPort;

    protected SolarSwitch currentSwitch;

    protected Collider thisCollider;

    public bool ShowOffsetLog = false;

    protected float lightTravelSpeed = 20f;

    protected float currentLightDistance = 0f;

    protected float targetDistance = 0f;

    private void Awake()
    {
        solarRay = GetComponent<LineRenderer>();
        solarRay.positionCount = 2;

        thisCollider = GetComponent<Collider>();
    }

    public void PerformLightCast( Vector3 localOffset, float maxLightDistance )
    {
        targetDistance = maxLightDistance;

        Vector3 currentRayOrigin = transform.position + transform.TransformVector( localOffset );

        solarRay.SetPosition( 0, currentRayOrigin );

        if ( Physics.Raycast( currentRayOrigin, transform.forward, out rayHit, currentLightDistance, ~0, QueryTriggerInteraction.Ignore ) )
        {
            targetDistance = rayHit.distance + 0.15f;

            if ( currentContact != rayHit.collider )
            {
                if ( currentContact != null )
                {
                    if ( currentPort != null )
                    {
                        currentPort.TogglePort( false );
                        currentPort = null;
                    }
                    if ( currentSwitch != null )
                    {
                        currentSwitch.ChangeState( false );
                        currentSwitch = null;
                    }
                }

                currentContact = rayHit.collider;

                currentPort = currentContact.GetComponent<RefractorPort>();

                if ( currentPort != null )
                {
                    currentPort.TogglePort( true );

                    currentSwitch = null;

                    return;
                }

                currentSwitch = currentContact.GetComponent<SolarSwitch>();

                if ( currentSwitch != null )
                {
                    currentSwitch.ChangeState( true );
                    return;
                }
            }
            else
            {
                if ( currentPort != null )
                {
                    currentPort.UpdateOffset( currentPort.transform.InverseTransformPoint( rayHit.point ) );
                }
            }
        }
        else
        {
            if ( currentContact != null )
            {
                currentContact = null;
            }

            if ( currentPort != null )
            {
                currentPort.TogglePort( false );
                currentPort = null;
            }

            if ( currentSwitch != null )
            {
                currentSwitch.ChangeState( false );
                currentSwitch = null;
            }
        }

        currentLightDistance = Mathf.Min( currentLightDistance + lightTravelSpeed * Time.fixedDeltaTime, targetDistance );

        solarRay.SetPosition( 1, currentRayOrigin + transform.forward * currentLightDistance );

        Debug.Log( currentLightDistance + " " + targetDistance );
    }
}
