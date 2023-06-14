using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationWatcher : PuzzleWatcher
{
    [SerializeField]
    private float rotateSpeed = 90f;

    [SerializeField]
    private float rotationAcceleration = 2f;

    [SerializeField]
    private Vector3 rotationAxis = Vector3.zero;

    private float currentRotateSpeed;

    [SerializeField]
    private bool Active = false;

    private void FixedUpdate()
    {
        currentRotateSpeed = Mathf.Lerp( currentRotateSpeed, ( Active ) ? rotateSpeed : 0, rotationAcceleration * Time.fixedDeltaTime );

        transform.Rotate( rotationAxis * currentRotateSpeed * Time.fixedDeltaTime, Space.Self );
    }

    protected override void OnSwitchUpdate()
    {
        Active = AllSwitchesActive();
    }
}
