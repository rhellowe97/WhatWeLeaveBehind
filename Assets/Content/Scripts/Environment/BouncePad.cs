using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField]
    protected float bouncePower = 25f;
    public float BouncePower => bouncePower;

    [SerializeField]
    protected Animator anim;

    private void OnTriggerEnter( Collider other )
    {
        if ( other.attachedRigidbody != null && other.attachedRigidbody.GetComponent<CharacterJump>() != null && anim != null )
        {
            anim.SetTrigger( "Bounce" );
        }
    }
}
 