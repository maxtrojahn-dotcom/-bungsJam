using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [SerializeField] private GameObject PauseCanvas;
    [SerializeField] private GameObject OptionsCanvas;

void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            PauseCanvas.SetActive(true);
        }
    }

    public void Resume()
    {
        PauseCanvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitApp() {
        Application.Quit();
        Debug.Log("Application has quit.");
    }
}