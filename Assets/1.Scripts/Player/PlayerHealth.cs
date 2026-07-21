using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public GameObject deathScreen;
    public GameObject playermodel;

    public UiController uiController;

    void Start()
    {
        currentHealth = maxHealth;

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
    }

    void Die()
    {
       uiController.UnlockCursor();
        deathScreen.SetActive(true);
        playermodel.SetActive(false);
        // Disable player controls here (e.g., disable movement, shooting, etc.)
    }

    public int GetHealth()
    {
        return currentHealth;
    }
  // regen health over time (optional)
  
}

