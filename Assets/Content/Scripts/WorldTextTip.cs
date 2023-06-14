using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldTextTip : MonoBehaviour
{
    [SerializeField]
    private float fadeTime = 1f;

    private Coroutine fadeCo;

    private List<TextMesh> subTexts = new List<TextMesh>();

    private void Awake()
    {
        subTexts = GetComponentsInChildren<TextMesh>().ToList();

        Color c;

        foreach ( TextMesh mesh in subTexts )
        {
            c = mesh.color;

            c.a = 0;

            mesh.color = c;
        }
    }

    private IEnumerator ToggleFade( bool fadeIn )
    {
        float t = 0;

        float currentAlpha = subTexts[0].color.a;

        if ( fadeIn )
            t = ( currentAlpha / 1 ) * fadeTime;
        else
            t = ( 1 - currentAlpha / 1 ) * fadeTime;

        Color c;

        while ( t < fadeTime )
        {
            foreach ( TextMesh mesh in subTexts )
            {
                c = mesh.color;
                if ( fadeIn )
                    c.a = Mathf.Lerp( 0, 1, t / fadeTime );
                else
                    c.a = Mathf.Lerp( 1, 0, t / fadeTime );

                mesh.color = c;
            }

            t += Time.deltaTime;

            yield return null;
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.GetComponent<CharacterController>() != null )
        {
            if ( fadeCo != null )
            {
                StopCoroutine( fadeCo );
            }

            fadeCo = StartCoroutine( ToggleFade( true ) );
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( other.GetComponent<CharacterController>() != null )
        {
            if ( fadeCo != null )
            {
                StopCoroutine( fadeCo );
            }

            fadeCo = StartCoroutine( ToggleFade( false ) );
        }
    }
}
