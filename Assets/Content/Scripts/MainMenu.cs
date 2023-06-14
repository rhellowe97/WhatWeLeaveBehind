using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;

    [SerializeField]
    private GameObject settingsPanel;

    public void StartLevel()
    {
        UIManager.Instance.ObscureScreen( true, 0.5f, () =>
        {
            SaveDataManager.Instance.LoadGame();

            GameManager.Instance.CurrentBiome = SaveDataManager.Instance.SaveData.LastBiomePlayed;

            GameManager.Instance.CurrentLevelIndex = SaveDataManager.Instance.SaveData.LastLevelPlayed;

            GameManager.Instance.LoadSetLevel();
        } );
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void CallClearSaveData()
    {
        SaveDataManager.Instance.ClearSaveGame();
    }

    public void BackToMain()
    {
        mainPanel.SetActive( true );

        settingsPanel.SetActive( false );
    }

    public void OpenSettings()
    {
        mainPanel.SetActive( false );

        settingsPanel.SetActive( true );
    }
}
