using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateWatcher : TranslateWatcher
{
    [SerializeField]
    private float delay = 0f;

    protected override void Start()
    {
        base.Start();

        StartCoroutine( Oscillate() );
    }

    protected override void OnSwitchUpdate() { }

    private IEnumerator Oscillate()
    {
        bool up = true;

        WaitForSeconds tweenDelay = new WaitForSeconds( delay );

        while ( true )
        {
            while ( !AllSwitchesActive() )
                yield return null;

            toggleTween = transform.DOMove( startPosition + ( up ? openVector : Vector3.zero ), translateSpeed ).SetEase( translateEase ).OnComplete( () => toggleTween = null ).SetUpdate( UpdateType.Fixed );

            yield return toggleTween.WaitForCompletion();

            yield return tweenDelay;

            up = !up;
        }
    }

}
