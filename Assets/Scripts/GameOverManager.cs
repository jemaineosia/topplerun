using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; set; }
    public GameObject gameOverUI;  // Reference to the Game Over UI Canvas

    void Awake()
    {
        // Set up singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Ensure the Game Over UI is hidden at start
        gameOverUI.SetActive(false);
    }

    // Method to trigger the Game Over event
    public void GameOver()
    {
        // Activate the Game Over UI
        gameOverUI.SetActive(true);

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
