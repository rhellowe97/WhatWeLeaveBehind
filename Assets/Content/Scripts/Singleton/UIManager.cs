using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.Linq;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private bool dialogueActive = false;

    [SerializeField]
    private Image fade;

    private Tween fadeTween;

    [BoxGroup( "Dialogue" )]
    [SerializeField]
    private RectTransform dialogueBox;

    [BoxGroup( "Dialogue" )]
    [SerializeField]
    private float dialogueBoxSpeed = 0.2f;

    [BoxGroup( "Dialogue" )]
    [SerializeField]
    private float dialogueBoxOpenSize = 800f;

    [BoxGroup( "Dialogue" )]
    [SerializeField]
    private float lastDialogueDelay = 4f;

    private float lastDialogueTimer = 0f;

    [BoxGroup( "Dialogue" )]
    [SerializeField]
    private TMP_Text dialogueText;

    [BoxGroup( "Dialogue" )]
    [SerializeField]
    private float letterDelay = 0.1f;

    private WaitForSeconds letterDelayWait;

    private Coroutine speechCo;

    private Coroutine dialogueLifetimeCo;

    private StringBuilder builder;

    private StringBuilder editBuilder;

    private char[] colorIgnoredCharacters = new char[] { '.', '!', '?', ',' };

    private bool skipToEnd = false;

    private bool nextLine = false;

    private bool inConvo = false;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );
        }

        Instance = this;

        DontDestroyOnLoad( gameObject );

        fade.color = Color.black;

        SceneManager.sceneLoaded += ForceFadeIn;

        letterDelayWait = new WaitForSeconds( letterDelay );

        builder = new StringBuilder();

        editBuilder = new StringBuilder();
    }

    [SerializeField]
    private void ForceFadeIn( Scene scene, LoadSceneMode mode )
    {
        fade.color = Color.black;

        ObscureScreen( false );
    }

    public void ObscureScreen( bool obscure, float fadeTime = 0.4f, UnityAction postFadeFunc = null )
    {
        if ( fadeTween != null )
            fadeTween.Kill();

        if ( obscure )
            fadeTween = fade.DOColor( Color.black, fadeTime ).SetEase( Ease.InOutSine ).OnComplete(
                () =>
                {
                    fadeTween = null;
                    if ( postFadeFunc != null )
                        postFadeFunc?.Invoke();
                } );
        else
            fadeTween = fade.DOColor( Color.clear, fadeTime ).SetEase( Ease.InOutSine ).OnComplete(
                 () =>
                 {
                     fadeTween = null;
                     if ( postFadeFunc != null )
                         postFadeFunc?.Invoke();
                 } );
    }

    public void ToggleDialogue( bool toggle )
    {
        dialogueActive = toggle;

        if ( dialogueActive )
            dialogueBox.DOScaleX( 1, dialogueBoxSpeed );
        else
        {
            dialogueBox.DOScaleX( 0, dialogueBoxSpeed );
            dialogueText.text = "";
        }

    }

    public void CharacterSpeak( Dialogue dialogue )
    {
        ToggleDialogue( true );

        if ( speechCo != null )
            StopCoroutine( speechCo );

        if ( dialogueLifetimeCo != null )
            lastDialogueTimer = 0f;

        speechCo = StartCoroutine( SpeakLine( dialogue ) );
    }

    public IEnumerator EngageConversation( List<Dialogue> conversation )
    {
        GameManager.Instance.GlobalControls.Global.Primary.performed += ConversationJump;

        int lineIndex = 0;

        inConvo = true;

        while ( lineIndex < conversation.Count )
        {
            nextLine = false;

            CharacterSpeak( conversation[lineIndex] );

            yield return new WaitUntil( () => nextLine ); //Wait for player input

            lineIndex++;
        }

        ToggleDialogue( false );

        inConvo = false;

        GameManager.Instance.GlobalControls.Global.Primary.performed -= ConversationJump;
    }

    private void ConversationJump( InputAction.CallbackContext context )
    {
        if ( !skipToEnd )
            skipToEnd = true;
        else if ( skipToEnd )
            nextLine = true;
    }

    private IEnumerator DialogueBoxOpen()
    {
        lastDialogueTimer = 0f;

        while ( lastDialogueTimer < lastDialogueDelay )
        {
            lastDialogueTimer += Time.deltaTime;

            yield return null;
        }

        dialogueLifetimeCo = null;

        ToggleDialogue( false );
    }

    private IEnumerator SpeakLine( Dialogue dialogue )
    {
        builder.Clear();

        if ( LocalizationManager.Instance == null )
        {
            Debug.LogError( "No Localization Manager... No Speaking!" );

            yield break;
        }

        string[] splitSentence = dialogue.Translations[Language.en].Split( ' ' );

        skipToEnd = false;

        builder.Append( $"[{dialogue.Character}]: " );

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
                dialogueText.text = builder.ToString();

                if ( !skipToEnd )
                    yield return letterDelayWait;
            }

            if ( containsPunctuation )
                builder.Append( extraPunc );

            builder.Append( " " );

            dialogueText.text = builder.ToString();
        }

        if ( !inConvo )
        {
            if ( dialogueLifetimeCo != null )
                lastDialogueTimer = 0f;
            else
                dialogueLifetimeCo = StartCoroutine( DialogueBoxOpen() );
        }

        speechCo = null;
    }
}
