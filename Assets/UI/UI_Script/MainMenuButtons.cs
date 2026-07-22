using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
  
    public void StartGame(string CatGame_01) 
    {
        SceneManager.LoadScene(CatGame_01);
        Debug.Log("Aktuelle Szene." + SceneManager.GetActiveScene().name);
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
