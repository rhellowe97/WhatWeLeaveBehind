using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;

public class CharacterSpeech : MonoBehaviour
{
    [SerializeField]
    private TMP_Text speechBubble;

    [SerializeField]
    private float letterDelay = 0.1f;

    [SerializeField]
    private string sentenceToTest = "";

    private WaitForSeconds letterDelayWait;

    private Coroutine speechCo;

    private StringBuilder builder;

    private StringBuilder editBuilder;

    private char[] colorIgnoredCharacters = new char[] { '.', '!', '?', ',' };

    private void Awake()
    {
        letterDelayWait = new WaitForSeconds( letterDelay );

        builder = new StringBuilder();

        editBuilder = new StringBuilder();
    }

    [Button]
    private void TestSpeed()
    {
        if ( speechCo != null )
            StopCoroutine( speechCo );

        speechCo = StartCoroutine( SpeakLine( sentenceToTest ) );
    }

    public void CharacterSpeak( string dialogueLine )
    {
        if ( speechCo != null )
            StopCoroutine( speechCo );

        speechCo = StartCoroutine( SpeakLine( dialogueLine ) );
    }

    private IEnumerator SpeakLine( string sentence )
    {
        builder.Clear();

        if ( LocalizationManager.Instance == null )
        {
            Debug.LogError( "No Localization Manager... No Speaking!" );

            yield break;
        }

        string[] splitSentence = sentence.Split( ' ' );

        for ( int i = 0; i < splitSentence.Length; i++ )
        {
            bool containsPunctuation = false;

            char extraPunc = ' ';

            if ( colorIgnoredCharacters.Contains( splitSentence[i][splitSentence[i].Length - 1] ) )
            {
                editBuilder.Clear();

                editBuilder.Append( splitSentence[i] );

                extraPunc = splitSentence[i][splitSentence[i].Length - 1];

                editBuilder.Remove( splitSentence[i].Length - 1, 1 );

                splitSentence[i] = editBuilder.ToString();

                containsPunctuation = true;
            }

            for ( int j = 0; j < splitSentence[i].Length; j++ )
            {
                if ( LocalizationManager.Instance.CheckColorStatus( splitSentence[i] ) && !colorIgnoredCharacters.Contains( splitSentence[i][j] ) )
                {
                    builder.Append( $"<color=#{ColorUtility.ToHtmlStringRGBA( LocalizationManager.Instance.GetTermColor( splitSentence[i] ) )}>{splitSentence[i][j]}</color>" );
                }
                else
                {
                    builder.Append( splitSentence[i][j] );
                }
                speechBubble.text = builder.ToString();

                yield return letterDelayWait;
            }

            if ( containsPunctuation )
                builder.Append( extraPunc );

            builder.Append( " " );

            speechBubble.text = builder.ToString();
        }
    }
}
