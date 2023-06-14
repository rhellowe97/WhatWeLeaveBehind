using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public class BiomeProgressDictionary : UnitySerializedDictionary<Biome, int> { }

    public BiomeProgressDictionary BiomeNextLevelMap = new BiomeProgressDictionary();

    public Biome LastBiomePlayed = Biome.HUB;

    public int LastLevelPlayed = 0;

    public List<string> SpokenDialogueTags;

    public SaveData()
    {
        BiomeNextLevelMap = new BiomeProgressDictionary();

        foreach ( Biome biomeType in System.Enum.GetValues( typeof( Biome ) ) )
        {
            BiomeNextLevelMap.Add( biomeType, 0 );
        }

        LastBiomePlayed = Biome.TUTORIAL;

        LastLevelPlayed = 0;

        SpokenDialogueTags = new List<string>();
    }
}
