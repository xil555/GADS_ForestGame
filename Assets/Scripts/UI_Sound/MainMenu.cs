using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        //  SHOW cursor in menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void MainMenuScene()
    {
        //  Lock cursor when entering game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("MainMenu");
    }
    public void PlayGame()
    {
        //  Lock cursor when entering game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("Game Scene");
    }
    public void GameCredits()
    {
        //  Lock cursor when entering game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("Credits Scene");
    }

    public void PlayerGuide()
    {
        //  Lock cursor when entering game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("Player Guide");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
