using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button getFullHpButton;

    [Header("Upgrade System")] 
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private UpgradeButton[] upgradeButtons;
    [SerializeField] private List<UpgradeData> upgradeDataList;
    private readonly List<UpgradeData> _selectableUpgrades=new List<UpgradeData>();
    
    //Aktif edildiği zaman UI objesi, GameEvents'teki belli olaylara abone olur. Bu sayede bazı olaylar olduğunda void'ler çalıştırır.
    void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthUI;
        GameEvents.OnPlayerExperienceChanged += UpdateExperienceUI;
        GameEvents.OnPlayerLevelUp += OpenLevelUpPanel;
    }

    //Deaktif edildiğinde abonelikten çıkar. Bu sayede tekrar aktif edilirse birden fazla kez çalışmaz void'ler.
    void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthUI;
        GameEvents.OnPlayerExperienceChanged -= UpdateExperienceUI;
        GameEvents.OnPlayerLevelUp -= OpenLevelUpPanel;
    }

    //Oyunun başında düğmelere tıklanınca olacak olan olayların atanmasını sağlar. Bu sayede editörden uğraştırmadan sadece kod parçalarını değiştirerek atamalar sağlanır.
    private void Start()
    {
        for (int i=0;i<upgradeButtons.Length;i++)
        {
            var i1 = i;
            upgradeButtons[i].GetComponent<Button>().onClick.AddListener(delegate { Upgrade(i1); });
        }
        
        restartButton.onClick.AddListener(RestartScene);
        getFullHpButton.onClick.AddListener(OnGetFullHpButtonPressed);
    }

    //Health Bar'ın, oyuncunun canı ve maksimum canına göre değerler almasını sağlar
    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    //Experience Bar'ın, oyuncunun tecrübesi ve maksimum tecrübesine göre değerler almasını sağlar
    private void UpdateExperienceUI(float currentExperience, float maxExperience)
    {
        experienceSlider.maxValue = maxExperience;
        experienceSlider.value = currentExperience;
    }

    //Level atlama panalenin açılması sağlanır
    private void OpenLevelUpPanel()
    {
        if (upgradeDataList.Count == 0) return;
        
        PauseGame();
        upgradePanel.SetActive(true);
        GetUpgradeDataToButtons();
    }
    
    //Level alındıysa panalenin kapanması sağlanır
    private void CloseLevelUpPanel()
    {
        upgradePanel.SetActive(false);
        ResumeGame();
    }

    //Mevcut yükseltmelerden belli miktarda rastgele seçilmesini sağlar. Eğer ki düğme sayısından az var ise fazla düğmeler kapanır.
    private void GetUpgradeDataToButtons()
    {
        var selectedIDs = new List<int>();
        
        var upgradeCount = 3;
        
        if (upgradeDataList.Count < upgradeButtons.Length)
        {
            upgradeCount=upgradeDataList.Count;
            
            for (int i = upgradeButtons.Length-1; i > upgradeCount-1; i--)
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        for (var i = 0; i < upgradeCount;)
        {
            var upgradeId = Random.Range(0, upgradeDataList.Count);
            
            if (!selectedIDs.Contains(upgradeId))
            {
                upgradeButtons[i].HandleUpgradeData(upgradeDataList[upgradeId]);
                var i1 = i;
                selectedIDs.Add(upgradeId);
                _selectableUpgrades.Add(upgradeDataList[upgradeId]);
                i++;
            }
        }
    }

    //Düğmelere atanmış olan yükseltmelerin oyuncuya eklenmesi ve ardından yükseltmenin listeden kaldırıp onun yerine varsa bir sonraki yükseltmenin eklenmesi sağlanır.
    private void Upgrade(int upgradeId)
    {
        var upgradeData=_selectableUpgrades[upgradeId];
        GameEvents.PublishUpgradeSelected(upgradeData);
        if(upgradeData.upgradeToAddToList!=null) upgradeDataList.Add(upgradeData.upgradeToAddToList);
        upgradeDataList.Remove(upgradeData);
        _selectableUpgrades.Clear();
        CloseLevelUpPanel();
    }
    
    //Oyunu durdurur
    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    //Oyunu devam ettirir
    private void ResumeGame()
    {
        Time.timeScale = 1;
    }

    //Sahneyi baştan başlatır
    private void RestartScene()
    {
        SceneManager.LoadScene(0);
    }

    //Oyuncunun canının fullenmesi için düğmeye basıldığında GameEvents ile abone olan tüm kodlara haber gönderir
    private void OnGetFullHpButtonPressed()
    {
        GameEvents.PublishFullHpButtonPressed();
    }
}