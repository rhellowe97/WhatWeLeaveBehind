using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    [SerializeField]
    protected BatteryColorData colorData;

    public delegate void StateChange();
    public event StateChange OnStateChanged;

    public bool Activated { get; private set; }

    public virtual void ChangeState( bool state )
    {
        Activated = state;

        OnStateChanged?.Invoke();
    }
}
