using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Refractor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private RefractorPort inPort;

    [SerializeField]
    private float maximumRefractionDistance = 10f;

    private List<RefractorPort> outPorts = new List<RefractorPort>();

    private void Awake()
    {
        outPorts = GetComponentsInChildren<RefractorPort>().Where( port => port.State == RefractorPort.PortState.OUT ).ToList();
    }

    private void FixedUpdate()
    {
        if ( inPort.Active )
        {
            foreach ( RefractorPort outPort in outPorts )
            {
                outPort.TogglePort( true );

                outPort.PerformLightCast( -inPort.ReceiverOffset, maximumRefractionDistance );
            }
        }
        else if ( outPorts.Count > 0 && outPorts[0].Active )
        {
            foreach ( RefractorPort outPort in outPorts )
            {
                outPort.TogglePort( false );
            }
        }
    }

    public void Interact()
    {
        transform.Rotate( Vector3.forward, 90f );
    }
}
