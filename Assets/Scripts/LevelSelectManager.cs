using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LevelSelectManager : MonoBehaviour
{
    public TMP_Text welcomeText;
    public Button back_Btn;
    public Button[] levelButtons; // Drag your 5 buttons here
    public Image[] tickSigns;     // Drag the 5 tick images here

    private SaveData myData;
    private PlayerProfile activeProfile;

    void Start()
    {
        string playerName = PlayerPrefs.GetString("ActivePlayer", "Guest");
        welcomeText.text = "Welcome " + playerName + ",\nPlease choose a level to continue";

        LoadPlayerData(playerName);
        SetupButtons();
    }

    void LoadPlayerData(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            myData = JsonUtility.FromJson<SaveData>(json);

            // Find the profile that matches the name we clicked
            activeProfile = myData.allPlayers.Find(p => p.playerName == name);
        }
    }

    void SetupButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Level 1 (index 0) is always playable. 
            // Others only playable if you are doing Level 1. 
            // (For now, we lock 2-5 as you requested)
            if (i == 0)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }

            // Set Tick Color
            if (activeProfile != null && activeProfile.levelsCompleted[i])
            {
                tickSigns[i].color = Color.green; // Completed
            }
            else
            {
                tickSigns[i].color = Color.gray;  // Not completed
            }

            // Add Click Listener
            int levelNum = i + 1;
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelNum));
        }
        if (back_Btn != null)
        {
            back_Btn.onClick.RemoveAllListeners(); // Clean up old connections
            back_Btn.onClick.AddListener(GoBack);  // Assign the method
        }
    }

    void LoadLevel(int levelIndex)
    {
        if (levelIndex == 1)
        {
            SceneManager.LoadScene("Level_1"); // Make sure your maze scene is named Level1
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene("_Mainmenu"); 
    }
}