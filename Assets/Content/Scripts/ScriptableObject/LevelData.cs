using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu( fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 6 )]
public class LevelData : ScriptableObject
{
    [SerializeField]
    protected string levelName = "";
    public string LevelName => levelName;

    [SerializeField]
    public string sceneName = "";
    public string SceneName => sceneName;
}

