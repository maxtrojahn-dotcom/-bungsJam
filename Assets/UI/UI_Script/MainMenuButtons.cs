using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour 
{
  
    public void StartGame(string Game_Level01) 
    {
        SceneManager.LoadScene(Game_Level01);
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
