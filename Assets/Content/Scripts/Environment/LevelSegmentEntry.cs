using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSegmentEntry : MonoBehaviour
{
    public delegate void EntryTriggered();
    public event EntryTriggered OnEntryTriggered;

    private void OnTriggerEnter( Collider other )
    {
        if ( other.GetComponent<CharacterController>() != null )
        {
            OnEntryTriggered?.Invoke();
        }
    }
}
