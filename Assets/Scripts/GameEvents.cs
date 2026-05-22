using System;

public static class GameEvents
{
    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action<float, float> OnPlayerExperienceChanged;

    public static void PublishPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public static void PublishPlayerExperienceChanged(float currentExperience, float maxExperience)
    {
        OnPlayerHealthChanged?.Invoke(currentExperience, maxExperience);
    }
}