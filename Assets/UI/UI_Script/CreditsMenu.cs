using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenuButton : MonoBehaviour 
{
    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void LoadMain(string MainMenu) 
    {
        SceneManager.LoadScene(MainMenu);
    }
    
}
