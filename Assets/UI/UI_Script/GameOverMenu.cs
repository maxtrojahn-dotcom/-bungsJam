using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject GameOverCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CatGame_01");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Application has quit.");
    }

}
