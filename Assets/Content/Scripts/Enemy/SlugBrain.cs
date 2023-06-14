using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugBrain : AIBrain
{
    private CharacterMove move;

    private Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
        move = GetComponent<CharacterMove>();
    }

    protected override void AIUpdate()
    {
        if ( playerAwarenessDistance * playerAwarenessDistance > sqrPlayerDistance )
        {
            moveDirection.x = Mathf.Sign( player.transform.position.x - transform.position.x );
        }
        else
        {
            moveDirection.x = 0;
        }

        move.SetMoveDirection( moveDirection );

        move.SimulateBehaviour();
    }
}
