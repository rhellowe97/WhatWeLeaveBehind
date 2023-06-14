using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 360f;

    [SerializeField]
    private float speedChangeDamping;

    private float currentSpeed = 0;

    private bool active = false;

    private Vector3 rotateDirection;

    private FanWatcher fanParent;

    private void Awake()
    {
        fanParent = GetComponentInParent<FanWatcher>();

        rotateDirection = fanParent.transform.up;
    }

    void FixedUpdate()
    {
        transform.Rotate( rotateDirection * currentSpeed * Time.fixedDeltaTime, Space.World );

        if ( active )
            currentSpeed = Mathf.Lerp( currentSpeed, rotationSpeed, speedChangeDamping * Time.fixedDeltaTime );
        else
            currentSpeed = Mathf.Lerp( currentSpeed, 0, speedChangeDamping * Time.fixedDeltaTime );
    }

    public void ToggleActive( bool toggle )
    {
        active = toggle;
    }
}
