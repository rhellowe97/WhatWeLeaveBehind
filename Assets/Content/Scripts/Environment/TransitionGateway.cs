using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionGateway : MonoBehaviour, IInteractable
{
    [SerializeField]
    private bool biomeGate;

    [SerializeField, ShowIf( "biomeGate" )]
    private Biome biome;


    public void Interact()
    {
        if ( biomeGate )
        {
            if ( biome == Biome.HUB )
            {
                GameManager.Instance.ReturnToHub();
                return;
            }

            GameManager.Instance.DirectLevelLoad( biome, SaveDataManager.Instance.SaveData.BiomeNextLevelMap[biome] );
        }
        else
        {
            GameManager.Instance.InitializeLevelLoad();
        }
    }
}
