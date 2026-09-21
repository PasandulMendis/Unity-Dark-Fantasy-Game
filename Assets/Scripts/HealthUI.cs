using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Player Reference")]
    public PlayerHealth playerHealth;

    void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged += UpdateHealthUI;

            UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
        }
    }

    private void UpdateHealthUI(int current, int max)
    {
        healthSlider.value = (float)current / (float)max;

        if (healthText != null)
        {
            healthText.text = current + " / " + max;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthUI;
        }
    }
}