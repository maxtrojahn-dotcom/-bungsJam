using UnityEngine;
using UnityEngine.SceneManagement;


public class UiController : MonoBehaviour
{
    [Header("StartUi")]
    public GameObject startPanel;
    public GameObject tutorialScreen;
    public GameObject pauseMenuUI;
    public GameObject tutorial;

    [Header("time")]
     bool isPaused = false;

    public void Start()
    {
        
       
        tutorialScreen.SetActive(true);
        tutorial.SetActive(false);
        UnlockCursor();
    }
    public void Startgame()
    {
        startPanel.SetActive(false);
        ResumeGame();
       
        LockCursor();
    }

    public void OnDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        isPaused = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        startPanel.SetActive(false);
        isPaused = false;
    }
    public void ShowTutorial()
    {
        Time.timeScale = 0f;
        tutorialScreen.SetActive(true);
        isPaused = true;
    }
    public void OnTutorialWeiter()
    {
        tutorialScreen.SetActive(false);
        tutorial.SetActive(true);
    }
    public void LeavePause()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        startPanel.SetActive(true);
        isPaused = false;
    }
    public void OpenTutorial()
    {
        Time.timeScale = 0f;
        tutorialScreen.SetActive(true);
        isPaused = true;
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}