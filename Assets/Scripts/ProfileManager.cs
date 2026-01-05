using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// --- DATA MODELS (Keep these at the top or bottom, outside the main class) ---

[Serializable]
public class PlayerProfile
{
    public string playerName;
    public bool[] levelsCompleted = new bool[5]; // Stores 5 levels
}

[Serializable]
public class SaveData
{
    public List<PlayerProfile> allPlayers = new List<PlayerProfile>();
}

// --- MAIN MANAGER CLASS ---

public class ProfileManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput;
    public Transform contentArea;
    public GameObject nameButtonPrefab;

    private SaveData myData = new SaveData();
    private string savePath;

    void Start()
    {
        // Sets the save path for Android/PC
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        LoadAllData();
        RefreshList();
    }

    public void AddNewPlayer()
    {
        string newName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("Please enter a name.");
            return;
        }

        // IMPORTANT: Reload data right before adding to make sure we have the latest list
        LoadAllData();

        // Check if the player already exists so we don't duplicate
        if (myData.allPlayers.Exists(p => p.playerName == newName))
        {
            Debug.Log("Player already exists! Just selecting them.");
            SelectPlayer(newName);
            return;
        }

        // Create new profile and add to the EXISTING list
        PlayerProfile newProfile = new PlayerProfile { playerName = newName };
        myData.allPlayers.Add(newProfile);

        SaveAllData();
        RefreshList();

        nameInput.text = "";
        SelectPlayer(newName);
    }

    
    void RefreshList()
    {
        // Clear existing buttons
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // Create a button for every player in save data
        foreach (PlayerProfile p in myData.allPlayers)
        {
            GameObject btnObj = Instantiate(nameButtonPrefab, contentArea);
            btnObj.GetComponentInChildren<TMP_Text>().text = p.playerName;

            // Add click event to the button
            string tempName = p.playerName; // Store name for the lambda
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectPlayer(tempName));
        }
    }

    public void SelectPlayer(string name)
    {
        // Save the active player name so the next scene knows who it is
        PlayerPrefs.SetString("ActivePlayer", name);
        Debug.Log("Player Selected: " + name);

        // Once you create the "LevelSelect" scene, uncomment the line below:
         SceneManager.LoadScene("_LevelSelect");
    }

    // --- SAVE/LOAD LOGIC ---

    void SaveAllData()
    {
        string json = JsonUtility.ToJson(myData);
        File.WriteAllText(savePath, json);
    }

    void LoadAllData()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                // This is the fix: Overwrite the myData object with the file contents
                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);
                if (loadedData != null && loadedData.allPlayers != null)
                {
                    myData = loadedData;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load save: " + e.Message);
            }
        }
        else
        {
            // If no file exists, make sure we have a clean list to start with
            myData = new SaveData();
        }
    }

}