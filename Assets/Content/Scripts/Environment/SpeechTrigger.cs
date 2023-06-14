using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechTrigger : MonoBehaviour
{
    [SerializeField]
    private DialogueData dialogueData;

    [ValueDropdown( "GetDialogueTags", DropdownTitle = "Select Dialogue Tag" )]
    [SerializeField]
    private string selectedTag;

    private IEnumerable GetDialogueTags()
    {
        if ( dialogueData != null )
        {
            foreach ( string dialogueTag in dialogueData.DialogeLines.Keys )
            {
                yield return dialogueTag;
            }
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        CharacterController character = other.GetComponent<CharacterController>();

        if ( character != null && selectedTag != null )
        {
            if ( !SaveDataManager.Instance.SaveData.SpokenDialogueTags.Contains( selectedTag ) )
            {
                UIManager.Instance.CharacterSpeak( dialogueData.DialogeLines[selectedTag] );

                GameManager.Instance.AddSpokenLine( selectedTag );
            }

            GetComponent<Collider>().enabled = false;
        }
    }
}
