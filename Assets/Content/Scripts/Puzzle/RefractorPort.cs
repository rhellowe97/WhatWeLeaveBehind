using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractorPort : SolarProjector
{
    [SerializeField]
    protected PortState state;
    public PortState State => state;

    public Vector3 ReceiverOffset = Vector3.zero;

    public bool Active { get; private set; }

    public void TogglePort( bool toggle )
    {
        Active = toggle;

        if ( Active )
        {
            solarRay.enabled = true;
        }
        else
        {
            solarRay.enabled = false;

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

            currentLightDistance = 0f;
        }
    }

    public void UpdateOffset( Vector3 newLocalOffset )
    {
        ReceiverOffset = newLocalOffset;
    }

    public enum PortState
    {
        IN,
        OUT,
        CLOSED
    }
}
