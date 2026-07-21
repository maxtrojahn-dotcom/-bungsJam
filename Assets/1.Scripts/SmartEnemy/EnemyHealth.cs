using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;

    public Animator animator;
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";

    private void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(name + " took " + damage + " damage");

        if (animator != null)
            animator.SetTrigger(hitTrigger);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (animator != null)
            animator.SetTrigger(deathTrigger);

        Destroy(gameObject, 2f);
    }
}