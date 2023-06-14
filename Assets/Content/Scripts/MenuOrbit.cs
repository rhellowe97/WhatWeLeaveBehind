using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOrbit : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 60f;

    private void Update()
    {
        transform.Rotate( Vector3.up, rotationSpeed * Time.deltaTime );
    }
}
