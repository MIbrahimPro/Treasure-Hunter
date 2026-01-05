using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class MazeLogic : MonoBehaviour
{
    public GameObject successPanel;
    public TMP_Text hintText;

    private bool hasKey = false;
    private string activePlayer;

    void Start()
    {
        activePlayer = PlayerPrefs.GetString("ActivePlayer", "Guest");
        successPanel.SetActive(false);
        hintText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        // PICK UP KEY
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            hintText.text = "Key Collected!";
            Destroy(other.gameObject); // Remove the key from the maze
            Invoke("ClearHint", 2f);
        }

        // TOUCH CHEST
        if (other.CompareTag("Chest"))
        {
            if (hasKey)
            {
                Win();
            }
            else
            {
                hintText.text = "The chest is locked. Find the key!";
                Invoke("ClearHint", 2f);
            }
        }
    }

    void Win()
    {
        successPanel.SetActive(true);
        SaveGame();
        // Stops time so player can't move after winning
        Time.timeScale = 0f;
    }

    void SaveGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Find our player in the list and mark level 1 as true
            PlayerProfile p = data.allPlayers.Find(x => x.playerName == activePlayer);
            if (p != null)
            {
                p.levelsCompleted[0] = true; // Level 1 is index 0
                File.WriteAllText(path, JsonUtility.ToJson(data));
            }
        }
    }

    void ClearHint() { hintText.text = ""; }

    // This is for the Continue button
    public void ContinueToLevelSelect()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene("_LevelSelect");
    }
}