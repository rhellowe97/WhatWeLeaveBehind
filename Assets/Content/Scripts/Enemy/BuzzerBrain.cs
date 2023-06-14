using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzerBrain : AIBrain
{
    [SerializeField]
    protected Transform swoopTarget;

    [SerializeField]
    protected float swoopSpeed = 1f;

    [SerializeField]
    private float swoopCooldown = 5f;

    [SerializeField]
    private float swoopRange = 7f;

    [SerializeField]
    private float heightCorrectSpeed = 7f;

    [SerializeField]
    private float heightCorrectAcceleration = 10f;

    [SerializeField]
    private float stayAbovePlayerHeight = 4f;

    private float swoopTimer = 0f;

    private Coroutine swoopCo = null;

    private CharacterMove characterMove;

    private Vector3 moveDirection = Vector3.zero;

    private WaitForFixedUpdate fixedDelay;

    private void Start()
    {
        characterMove = GetComponent<CharacterMove>();

        swoopTimer = swoopCooldown;

        fixedDelay = new WaitForFixedUpdate();
    }

    protected override void AIUpdate()
    {
        if ( playerAwarenessDistance * playerAwarenessDistance > sqrPlayerDistance && swoopCo == null )
        {
            moveDirection.x = Mathf.Sign( player.transform.position.x - transform.position.x );
        }
        else
        {
            moveDirection.x = 0;
        }

        characterMove.SetMoveDirection( moveDirection );

        characterMove.SimulateBehaviour();

        if ( swoopTimer < swoopCooldown )
        {
            swoopTimer += Time.fixedDeltaTime;
        }
        else
        {
            Vector3 targetDiff = swoopTarget.transform.position - transform.position;

            if ( Mathf.Abs( targetDiff.x ) <= swoopRange && swoopTarget.position.y < transform.position.y )
                ActivateSwoop();
        }

        if ( swoopCo == null )
        {
            Vector3 velocity = character.Rigidbody.velocity;

            float heightDiff = ( player.transform.position.y + stayAbovePlayerHeight ) - transform.position.y;

            velocity.y = Mathf.Lerp( velocity.y, Mathf.Clamp( heightDiff / 0.25f, -heightCorrectSpeed, heightCorrectSpeed ), heightCorrectAcceleration * Time.fixedDeltaTime );

            character.UpdateVelocity( velocity );
        }
    }

    private void ActivateSwoop()
    {
        if ( swoopCo == null )
        {
            swoopTimer = 0f;
            swoopCo = StartCoroutine( SwoopRoutine() );
        }
    }

    private IEnumerator SwoopRoutine()
    {
        Vector3 startPos = transform.position;

        Vector3 targetStartPos = swoopTarget.position;

        Vector3 endPos = startPos;

        Debug.Log( startPos + " " + targetStartPos + " " + endPos );

        endPos.x = swoopTarget.position.x - ( startPos.x - swoopTarget.position.x );

        float t = 0;

        while ( t < swoopSpeed )
        {
            transform.position = GetPoint( startPos, targetStartPos, endPos, t / swoopSpeed );

            t += Time.fixedDeltaTime;

            yield return fixedDelay;
        }

        swoopCo = null;
    }

    private Vector3 GetPoint( Vector3 startPoint, Vector3 midPoint, Vector3 endPoint, float t )
    {
        return Vector3.Lerp( Vector3.Lerp( startPoint, midPoint, t ), Vector3.Lerp( midPoint, endPoint, t ), t );
    }
}
