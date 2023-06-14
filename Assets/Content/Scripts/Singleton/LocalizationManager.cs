using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );
        }

        Instance = this;

        DontDestroyOnLoad( gameObject );

    }

    [SerializeField]
    private TermColorData TermData;

    public bool CheckColorStatus( string term )
    {
        return TermData.TermDefinitions.ContainsKey( term );
    }

    public Color GetTermColor( string term )
    {
        return TermData.DefinitionColors[TermData.TermDefinitions[term]];
    }
}
