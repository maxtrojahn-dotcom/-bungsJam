using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Saturation")]
    public int maxSaturation = 20;
    private int currentSaturation;

    [Header("UI")]
    public Slider saturationBar;
    public GameObject GameOverScreen;
   


    void Start()
    {
        currentSaturation = maxSaturation;

        saturationBar.maxValue = maxSaturation;
        saturationBar.value = currentSaturation;
    }

    public void TakeDamage(int damage)
    {
        currentSaturation -= damage;
        currentSaturation = Mathf.Max(currentSaturation, 0);

        UpdateSaturationBar();

        if (currentSaturation <= 0)
        {
            GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentSaturation += amount;
        currentSaturation = Mathf.Min(currentSaturation, maxSaturation);

        UpdateSaturationBar();
    }

    void UpdateSaturationBar()
    {
        saturationBar.value = currentSaturation;
    }

    void GameOver()
    {
        GameOverScreen.SetActive(true);
    }

    public int GetSaturation()
    {
        return currentSaturation;
    }
 
  
}

