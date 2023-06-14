using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "TermColorData", menuName = "ScriptableObjects/TermColorData", order = 2 )]
public class TermColorData : ScriptableObject
{
    [System.Serializable]
    public class TermColorDictionary : UnitySerializedDictionary<TermDefinition, Color> { }

    [SerializeField]
    public TermColorDictionary DefinitionColors = new TermColorDictionary();

    [System.Serializable]
    public class TermDefinitionDictionary : UnitySerializedDictionary<string, TermDefinition> { }

    [SerializeField]
    public TermDefinitionDictionary TermDefinitions = new TermDefinitionDictionary();

    public enum TermDefinition
    {
        Character,
        Location,
        ObjectOfInterest
    }
}
