using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerMenu : MonoBehaviour
{
    [SerializeField] private GameObject WinnerCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }

}
