using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameplayManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject deathPanel;
    public GameObject winPanel;
    public TMP_Text statusMessage;

    private bool hasKey = false;
    private string activePlayerName;

    void Start()
    {
        // Get the name of who is playing
        activePlayerName = PlayerPrefs.GetString("ActivePlayer", "Guest");
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
    }

    // This handles all the "touching" logic
    private void OnTriggerEnter(Collider other)
    {
        // 1. KILL ZONE
        if (other.CompareTag("KillZone"))
        {
            ShowDeathScreen();
        }

        // 2. THE KEY
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            statusMessage.text = "Key Collected!";
            Destroy(other.gameObject); // Remove the key from the maze
            Invoke("ClearMessage", 2f); // Clear text after 2 seconds
        }

        // 3. THE CHEST
        if (other.CompareTag("Chest"))
        {
            if (hasKey)
            {
                WinLevel();
            }
            else
            {
                statusMessage.text = "Please find a key!";
                Invoke("ClearMessage", 2f);
            }
        }
    }

    void ShowDeathScreen()
    {
        deathPanel.SetActive(true);
        Time.timeScale = 0f; // Freezes the game
    }

    void WinLevel()
    {
        SaveProgress();
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void SaveProgress()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Find the current player and mark Level 1 as complete
            PlayerProfile profile = data.allPlayers.Find(p => p.playerName == activePlayerName);
            if (profile != null)
            {
                profile.levelsCompleted[0] = true; // 0 is Level 1
                File.WriteAllText(path, JsonUtility.ToJson(data));
            }
        }
    }

    void ClearMessage() { statusMessage.text = ""; }

    // Button Functions
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }
}