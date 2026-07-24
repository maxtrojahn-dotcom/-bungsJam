using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame(string comic_intro) 
    {
        SceneManager.LoadScene(comic_intro);
    }
    
    public void QuitApp() 
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }

    public void GoToCredits(string CreditsMenu)
    {
        SceneManager.LoadScene(CreditsMenu);
    }

}
