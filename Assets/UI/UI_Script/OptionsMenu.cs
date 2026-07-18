using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour 
{
    [SerializeField] private GameObject OptionsCanvas;
    [SerializeField] private Slider SoundSlider;
    [SerializeField] private AudioMixer MainMixer;
    
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

public void SetVolume()
    {
        float volume = SoundSlider.value;
        MainMixer.SetFloat("output", Mathf.Log10(volume)*20);
    }

}
