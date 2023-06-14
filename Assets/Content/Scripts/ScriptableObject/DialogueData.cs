using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData", order = 8 )]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueDictionary : UnitySerializedDictionary<string, Dialogue> { }

    [DictionaryDrawerSettings( DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Identifier", ValueLabel = "Dialogue" )]
    [SerializeField]
    protected DialogueDictionary dialogeLines = new DialogueDictionary();
    public DialogueDictionary DialogeLines => dialogeLines;
}

public enum Language
{
    en,
    fr
}

public enum SpeakingCharacter
{
    Jammo,
    Willy
}

[System.Serializable]
public class Dialogue
{
    public SpeakingCharacter Character;
    public TranslatedPhraseDictionary Translations;
}

[DictionaryDrawerSettings( KeyColumnWidth = 50, KeyLabel = "Lang", ValueLabel = "Line" )]
[System.Serializable]
public class TranslatedPhraseDictionary : UnitySerializedDictionary<Language, string> { }
