using UnityEngine;
using UnityEngine.UI;

public class PlayerCaughtLives : MonoBehaviour
{
    [Header("Herzaugen")]
    [SerializeField] private Image[] heartEyes;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;

    [Header("Schutz vor mehreren NPCs gleichzeitig")]
    [SerializeField] private float hitCooldown = 0.5f;

    private int currentHeartEyes;
    private float nextAllowedHitTime;

    private void Awake()
    {
        currentHeartEyes = heartEyes.Length;
        UpdateHeartEyes();

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    public void LoseHeartEye()
    {
        if (currentHeartEyes <= 0)
            return;

        if (Time.time < nextAllowedHitTime)
            return;

        nextAllowedHitTime = Time.time + hitCooldown;

        currentHeartEyes--;

        UpdateHeartEyes();

        if (currentHeartEyes == 0)
            GameOver();
    }

    private void UpdateHeartEyes()
    {
        for (int i = 0; i < heartEyes.Length; i++)
        {
            if (heartEyes[i] != null)
                heartEyes[i].enabled = i < currentHeartEyes;
        }
    }

    private void GameOver()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
    }
}
