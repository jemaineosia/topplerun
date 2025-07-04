using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; set; }
    public GameObject gameOverUI;
    public TextMeshProUGUI finalScoreText;

    [Header("Audio Settings")]
    public AudioSource bgMusicSource;
    public AudioSource sfxSource;   // Assign the background music AudioSource in Inspector
    public AudioClip gameOverClip;

    void Awake()
    {
        gameOverUI.SetActive(false);
        
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Method to trigger the Game Over event
    public void GameOver()
    {
        if (bgMusicSource != null)
            bgMusicSource.Stop();

        // Play game over sound
        if (sfxSource != null && gameOverClip != null)
            sfxSource.PlayOneShot(gameOverClip);

        Debug.Log("Game Over triggered!");
        // Activate the Game Over UI
        gameOverUI.SetActive(true);

        finalScoreText.text = "Time Survived: " + ScoreManager.Instance.GetScore() + " sec";

        // Pause the game by setting Time.timeScale to 0
        Time.timeScale = 0f;  // Freeze the game
    }

    // Method to restart the game (or reload the current scene)
    public void RestartGame()
    {
        // Reset the time scale in case the game was paused
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
