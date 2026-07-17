using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour 
{
    [SerializeField] private GameObject OptionsCanvas;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Back()
    {
        OptionsCanvas.SetActive(false);
    }

}
