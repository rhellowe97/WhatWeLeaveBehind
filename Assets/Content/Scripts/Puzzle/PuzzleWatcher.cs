using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWatcher : MonoBehaviour, IResetable
{
    [SerializeField]
    protected List<PuzzleSwitch> linkedSwitches;

    [SerializeField]
    private bool invertActiveState = false;

    [SerializeField]
    private bool TryStartActive = false;

    private void Awake()
    {
        foreach ( PuzzleSwitch linkedSwitch in linkedSwitches )
        {
            linkedSwitch.OnStateChanged += OnSwitchUpdate;
        }

        if ( TryStartActive )
            OnSwitchUpdate();
    }

    protected virtual void OnSwitchUpdate() { }

    public virtual void ResetObject() { }

    protected bool AllSwitchesActive()
    {
        if ( !invertActiveState )
        {
            foreach ( PuzzleSwitch linkedSwitch in linkedSwitches )
            {
                if ( !linkedSwitch.Activated )
                    return false;
            }

            return true;
        }
        else
        {
            foreach ( PuzzleSwitch linkedSwitch in linkedSwitches )
            {
                if ( linkedSwitch.Activated )
                    return false;
            }

            return true;
        }
    }
}
