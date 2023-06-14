using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;

    [SerializeField]
    private bool ignoreSaveData = false;

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );
        }

        Instance = this;

        DontDestroyOnLoad( gameObject );

        Debug.Log( Application.persistentDataPath );

        // Update the path once the persistent path exists.
        saveFile = Application.persistentDataPath + "/savedata.json";
    }

    // Create a field for the save file.
    string saveFile;

    public SaveData SaveData = new SaveData();

    // FileStream used for reading and writing files.
    FileStream dataStream;

    // Key for reading and writing encrypted data.
    // (This is a "hardcoded" secret key. )
    byte[] savedKey = { 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15 };

    [Button]
    public void ClearSaveGame()
    {
        SaveData = new SaveData();

        SaveGame();
    }

    [Button]
    public void LoadGame()
    {
        if ( ignoreSaveData )
        {
            SaveData = new SaveData();

            return;
        }

        // Does the file exist?
        if ( File.Exists( saveFile ) )
        {
            // Create FileStream for opening files.
            dataStream = new FileStream( saveFile, FileMode.Open );

            // Create new AES instance.
            Aes oAes = Aes.Create();

            // Create an array of correct size based on AES IV.
            byte[] outputIV = new byte[oAes.IV.Length];

            // Read the IV from the file.
            dataStream.Read( outputIV, 0, outputIV.Length );

            // Create CryptoStream, wrapping FileStream
            CryptoStream oStream = new CryptoStream(
                   dataStream,
                   oAes.CreateDecryptor( savedKey, outputIV ),
                   CryptoStreamMode.Read );

            // Create a StreamReader, wrapping CryptoStream
            StreamReader reader = new StreamReader( oStream );

            // Read the entire file into a String value.
            string text = reader.ReadToEnd();

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            SaveData = JsonUtility.FromJson<SaveData>( text );

            dataStream.Close();

            Debug.Log( "Game File Found!" );
        }
        else
        {
            SaveData = new SaveData();

            Debug.Log( "No File found, new data created." );
        }
    }

    [Button]
    public void SaveGame()
    {
        if ( ignoreSaveData )
            return;

        // Create new AES instance.
        Aes iAes = Aes.Create();

        // Create a FileStream for creating files.
        dataStream = new FileStream( saveFile, FileMode.Create );

        // Save the new generated IV.
        byte[] inputIV = iAes.IV;

        // Write the IV to the FileStream unencrypted.
        dataStream.Write( inputIV, 0, inputIV.Length );

        // Create CryptoStream, wrapping FileStream.
        CryptoStream iStream = new CryptoStream(
                dataStream,
                iAes.CreateEncryptor( savedKey, iAes.IV ),
                CryptoStreamMode.Write );

        // Create StreamWriter, wrapping CryptoStream.
        StreamWriter sWriter = new StreamWriter( iStream );

        // Serialize the object into JSON and save string.
        string jsonString = JsonUtility.ToJson( SaveData );

        // Write to the innermost stream (which will encrypt).
        sWriter.Write( jsonString );

        // Close StreamWriter.
        sWriter.Close();

        // Close CryptoStream.
        iStream.Close();

        // Close FileStream.
        dataStream.Close();

        Debug.Log( "Game Saved!" );
    }
}
