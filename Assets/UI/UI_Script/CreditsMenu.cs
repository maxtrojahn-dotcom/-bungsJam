using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenuButton : MonoBehaviour 
{
  
    public void LoadMain(string MainMenu) 
    {
        SceneManager.LoadScene(MainMenu);
    }
    
}
