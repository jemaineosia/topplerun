using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Call this function when Start Game button is pressed
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    // Call this function when Quit button is pressed
    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("Application.Quit() called. (Editor mode: window not closed)");
#else
        Application.Quit();
#endif
    }
}
