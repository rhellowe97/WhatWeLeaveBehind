using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BiomeData biomeData;

    [SerializeField]
    private LevelListData levelList;

    public static GameManager Instance;

    private CharacterController player;

    private Level currentLevel;

    private List<IResetable> resetableObjects = new List<IResetable>();

    public Biome CurrentBiome = Biome.HUB;

    public int CurrentLevelIndex = 0;

    private List<string> cachedSpokenLines = new List<string>();

    public PlayerControls GlobalControls;

    [SerializeField]
    protected string sceneToLoad;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );
        }

        Instance = this;

        DontDestroyOnLoad( gameObject );

        GlobalControls = new PlayerControls();

        GlobalControls.Enable();

        SceneManager.sceneLoaded += FindLevelReferences;

        SceneManager.LoadScene( sceneToLoad );
    }

    private void FindLevelReferences( Scene Scene, LoadSceneMode mode )
    {
        currentLevel = FindObjectOfType<Level>();

        player = FindObjectOfType<CharacterController>();

        resetableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IResetable>().ToList();
    }

    public void Respawn()
    {
        if ( UIManager.Instance != null )
        {
            currentLevel.PlayerCamera.enabled = false;

            UIManager.Instance.ObscureScreen( true, 0.8f,
                () =>
                {
                    Vector3 playerPreWarp = player.transform.position;

                    Vector3 currentCheckpoint = currentLevel.GetCurrentCheckpoint();

                    player.transform.position = currentCheckpoint;

                    player.Respawned();

                    currentLevel.ActiveSegment.ToggleActive( true );

                    foreach ( IResetable resetable in resetableObjects )
                        resetable.ResetObject();

                    UIManager.Instance.ObscureScreen( false );
                } );
        }
    }

    public void AddSpokenLine( string line )
    {
        cachedSpokenLines.Add( line );
    }

    public void InitializeLevelLoad()
    {
        UIManager.Instance.ObscureScreen( true, 0.4f, () =>
        {
            if ( CurrentLevelIndex + 1 >= biomeData.BiomeLevelMap[CurrentBiome].LevelList.Count )
            {
                SaveDataManager.Instance.SaveData.LastBiomePlayed = CurrentBiome = Biome.HUB;
                SaveDataManager.Instance.SaveData.LastLevelPlayed = CurrentLevelIndex = 0;
                SaveDataManager.Instance.SaveData.SpokenDialogueTags.AddRange( cachedSpokenLines );
            }
            else
            {
                SaveDataManager.Instance.SaveData.LastBiomePlayed = CurrentBiome;
                SaveDataManager.Instance.SaveData.LastLevelPlayed = CurrentLevelIndex++;
                SaveDataManager.Instance.SaveData.BiomeNextLevelMap[CurrentBiome] = CurrentLevelIndex;
                SaveDataManager.Instance.SaveData.SpokenDialogueTags.AddRange( cachedSpokenLines );
            }

            cachedSpokenLines.Clear();

            SaveDataManager.Instance.SaveGame();

            LoadSetLevel();
        } );
    }

    public void DirectLevelLoad( Biome biome, int levelIndex )
    {
        UIManager.Instance.ObscureScreen( true, 0.4f, () =>
        {
            SaveDataManager.Instance.SaveData.LastBiomePlayed = CurrentBiome = biome;
            SaveDataManager.Instance.SaveData.LastLevelPlayed = CurrentLevelIndex = levelIndex;

            cachedSpokenLines.Clear();

            SaveDataManager.Instance.SaveGame();

            LoadSetLevel();
        } );
    }

    public void ReturnToHub()
    {
        DirectLevelLoad( Biome.HUB, 0 );
    }

    public void LoadSetLevel()
    {
        if ( biomeData == null )
        {
            Debug.LogError( "Missing Biome Level Data..." );
            return;
        }

        Debug.Log( $"Attempting Level Load: Biome = {CurrentBiome}, LevelIndex = {CurrentLevelIndex}, Resulting Scene = { biomeData.BiomeLevelMap[CurrentBiome].LevelList[SaveDataManager.Instance.SaveData.BiomeNextLevelMap[CurrentBiome]].sceneName}" );


        SceneManager.LoadScene( biomeData.BiomeLevelMap[CurrentBiome].LevelList[SaveDataManager.Instance.SaveData.BiomeNextLevelMap[CurrentBiome]].sceneName );

    }
}
