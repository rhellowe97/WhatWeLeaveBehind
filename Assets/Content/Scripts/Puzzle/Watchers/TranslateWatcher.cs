using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateWatcher : PuzzleWatcher
{
    [SerializeField]
    protected Vector3 openVector = Vector3.zero;

    [SerializeField]
    protected float translateSpeed = 1f;

    [SerializeField]
    protected Ease translateEase = Ease.InOutSine;

    protected Tween toggleTween;

    protected Vector3 startPosition;

    protected virtual void Start()
    {
        startPosition = transform.position;
    }

    protected override void OnSwitchUpdate()
    {
        if ( toggleTween != null )
        {
            toggleTween.Kill();
        }

        if ( AllSwitchesActive() )
        {
            toggleTween = transform.DOMove( startPosition + openVector, translateSpeed ).SetEase( translateEase ).OnComplete( () => toggleTween = null ).SetUpdate( UpdateType.Fixed );
        }
        else
        {
            toggleTween = transform.DOMove( startPosition, translateSpeed ).SetEase( translateEase ).OnComplete( () => toggleTween = null ).SetUpdate( UpdateType.Fixed );
        }
    }
}
