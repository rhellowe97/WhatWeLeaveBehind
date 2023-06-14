using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechArea : MonoBehaviour
{
    [SerializeField]
    private DialogueData dialogueData;

    [ValueDropdown( "GetDialogueTags", DropdownTitle = "Select Dialogue Tag" )]
    [SerializeField]
    private string selectedTag;

    [SerializeField]
    private float timeUntilSpeech = 10f;

    private Coroutine waitCo;

    private WaitForSeconds waitDelay;

    private void Awake()
    {
        waitDelay = new WaitForSeconds( timeUntilSpeech );
    }

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
                waitCo = StartCoroutine( SpeechWaitRoutine() );
            }
        }
    }

    private void OnTriggerExit( Collider other )
    {
        CharacterController character = other.GetComponent<CharacterController>();

        if ( character != null )
        {
            if ( waitCo != null )
                StopCoroutine( waitCo );
        }
    }

    private IEnumerator SpeechWaitRoutine()
    {
        yield return waitDelay;

        UIManager.Instance.CharacterSpeak( dialogueData.DialogeLines[selectedTag] );

        GetComponent<Collider>().enabled = false;
    }
}
