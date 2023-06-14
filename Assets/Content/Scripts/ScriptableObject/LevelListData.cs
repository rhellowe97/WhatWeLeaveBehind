using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "LevelListData", menuName = "ScriptableObjects/LevelListData", order = 5 )]
public class LevelListData : ScriptableObject
{
    [SerializeField]
    protected List<LevelData> levels;
    public List<LevelData> Levels => levels;
}