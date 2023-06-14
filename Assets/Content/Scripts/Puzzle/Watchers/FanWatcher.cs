using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanWatcher : PuzzleWatcher
{
    [SerializeField]
    private Propellor propBlade;

    [SerializeField]
    private ForceArea fanArea;

    protected override void OnSwitchUpdate()
    {
        propBlade.ToggleActive( AllSwitchesActive() );

        fanArea.ToggleActive( AllSwitchesActive() );
    }
}
