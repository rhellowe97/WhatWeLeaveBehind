using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "BiomeData", menuName = "ScriptableObjects/BiomeData", order = 4 )]
public class BiomeData : ScriptableObject
{
    [System.Serializable]
    public class BiomeLevelDictionary : UnitySerializedDictionary<Biome, LocalLevelList> { }

    [DictionaryDrawerSettings( KeyLabel = "Biome", ValueLabel = "Level" )]
    [SerializeField]
    protected BiomeLevelDictionary biomeLevelMap = new BiomeLevelDictionary();
    public BiomeLevelDictionary BiomeLevelMap => biomeLevelMap;
}

public enum Biome
{
    TUTORIAL,
    HUB,
    MINES,
    SEWERS,
    LAB,
    SURFACE
}

[System.Serializable]
public class LocalLevelList
{
    public List<LevelData> LevelList;
}