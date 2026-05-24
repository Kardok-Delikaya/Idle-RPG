using System;

//Bu kodun hiyerayşiye eklenmesine gerek yok, otomatik olarak çalışıyor
//Bu kod ile objeler arası bağlantı olmadan bi olay olduğunda void'lerin gerekli değerler ile çalışması sağlanıyor. Bu sayede eğer ki bi obje eksik olsa bile diğerindeki hata vermeden devam edebiliyor.

public static class GameEvents
{
    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action<float, float> OnPlayerExperienceChanged;
    public static event Action OnEnemyKilled;
    public static event Action OnPlayerLevelUp;
    public static event Action<UpgradeData> OnUpgradeSelected;
    public static event Action OnGetFullHpButtonPressed;

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
    
    public static void PublishFullHpButtonPressed()
    {
        OnGetFullHpButtonPressed?.Invoke();
    }
}