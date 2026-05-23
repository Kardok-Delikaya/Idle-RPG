using System;

public static class GameEvents
{
    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action<float, float> OnPlayerExperienceChanged;
    public static event Action OnEnemyKilled;
    public static event Action OnPlayerLevelUp;
    public static event Action<UpgradeData> OnUpgradeSelected;

    public static void PublishPlayerHealthChanged(float currentHealth, float maxHealth)
    {
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public static void PublishPlayerExperienceChanged(float currentExperience, float maxExperience)
    {
        OnPlayerExperienceChanged?.Invoke(currentExperience, maxExperience);
    }
    
    public static void PublishEnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }
    
    public static void PublishPlayerLevelUp()
    {
        OnPlayerLevelUp?.Invoke();
    }
    
    public static void PublishUpgradeSelected(UpgradeData upgradeData)
    {
        OnUpgradeSelected?.Invoke(upgradeData);
    }
}