using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperBrain : AIBrain
{
    private CharacterJump characterJump;

    private CharacterMove characterMove;

    private Vector3 moveDirection;

    private void Start()
    {
        characterMove = GetComponent<CharacterMove>();

        characterJump = GetComponent<CharacterJump>();
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

        if ( character.Grounded )
            characterJump.SetJump( false );

        characterMove.SetMoveDirection( moveDirection );

        characterMove.SimulateBehaviour();
        characterJump.SimulateBehaviour();
    }
}
