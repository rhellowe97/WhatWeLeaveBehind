using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "BatteryColorData", menuName = "ScriptableObjects/BatteryColorData", order = 1 )]
public class BatteryColorData : ScriptableObject
{
    [SerializeField]
    private Color startBaseColor;
    public Color StartBaseColor => startBaseColor;

    [SerializeField]
    private Color chargedBaseColor;
    public Color ChargedBaseColor => chargedBaseColor;

    [SerializeField, ColorUsage( true, true )]
    private Color startEmissionColor;
    public Color StartEmissionColor => startEmissionColor;

    [SerializeField, ColorUsage( true, true )]
    private Color chargedEmissionColor;
    public Color ChargedEmissionColor => chargedEmissionColor;
}
