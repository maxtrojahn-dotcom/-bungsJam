using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour {
    public void GoToScene(string PauseMenu) {
        SceneManager.LoadScene(PauseMenu);
    }
}