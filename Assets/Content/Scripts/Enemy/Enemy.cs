using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float deathRotateSpeed = 120f;

    [SerializeField]
    private float deathTime = 1f;

    private Coroutine deathCo;

    public void Death()
    {
        if ( deathCo == null )
        {
            deathCo = StartCoroutine( DeathRoutine() );
        }
    }

    private IEnumerator DeathRoutine()
    {
        float t = 0;

        while ( t < deathTime )
        {
            transform.localScale = Vector3.Lerp( Vector3.one, Vector3.zero, t / deathTime );

            transform.Rotate( Vector3.up, deathRotateSpeed * Time.deltaTime );

            t += Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive( false );
    }
}
