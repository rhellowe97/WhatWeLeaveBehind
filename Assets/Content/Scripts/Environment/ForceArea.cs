using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceArea : MonoBehaviour
{
    [SerializeField]
    private bool activeOnAwake = false;

    [SerializeField]
    protected float externalAcceleration;
    public float ExternalAcceleration => externalAcceleration;

    [SerializeField]
    protected float falloffPerUnit = 0.95f;
    public float FallOffPerUnit => falloffPerUnit;

    [SerializeField]
    private bool overrideDirection = false;

    [ShowIf( "overrideDirection" )]
    [SerializeField]
    protected Vector3 direction = Vector3.zero;
    public Vector3 Direction => direction;

    [SerializeField]
    private ParticleSystem visualEffect;

    public bool Active { get; private set; }

    private void Awake()
    {
        Active = activeOnAwake;

        if ( !overrideDirection )
            direction = transform.forward;
    }

    public void ToggleActive( bool toggle )
    {
        Active = toggle;

        if ( visualEffect != null )
        {
            if ( Active )
                visualEffect.Play();
            else
                visualEffect.Stop();
        }

    }

}
