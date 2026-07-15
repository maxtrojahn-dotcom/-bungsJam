using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour 
{
  
    public void LoadMainMenu(string MainMenu) 
    {
        SceneManager.LoadScene(MainMenu);
    }
    
}
