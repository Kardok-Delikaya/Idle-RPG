using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider experienceSlider;

    void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthUI;
        GameEvents.OnPlayerExperienceChanged += UpdateExperienceUI;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthUI;
        GameEvents.OnPlayerExperienceChanged -= UpdateExperienceUI;
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void UpdateExperienceUI(float currentExperience, float maxExperience)
    {
        experienceSlider.maxValue = maxExperience;
        experienceSlider.value = currentExperience;
    }
}