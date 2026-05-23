using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider experienceSlider;

    [Header("Upgrade System")] 
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private UpgradeButton[] upgradeButtons;
    [SerializeField] private List<UpgradeData> upgradeDataList;
    private List<UpgradeData> _selectableUpgrades=new List<UpgradeData>();
    
    void OnEnable()
    {
        GameEvents.OnPlayerHealthChanged += UpdateHealthUI;
        GameEvents.OnPlayerExperienceChanged += UpdateExperienceUI;
        GameEvents.OnPlayerLevelUp += OpenLevelUpPanel;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerHealthChanged -= UpdateHealthUI;
        GameEvents.OnPlayerExperienceChanged -= UpdateExperienceUI;
        GameEvents.OnPlayerLevelUp -= OpenLevelUpPanel;
    }

    private void Start()
    {
        for (int i=0;i<upgradeButtons.Length;i++)
        {
            var i1 = i;
            upgradeButtons[i].GetComponent<Button>().onClick.AddListener(delegate { Upgrade(i1); });
        }
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

    private void OpenLevelUpPanel()
    {
        if (upgradeDataList.Count == 0) return;
        
        PauseGame();
        upgradePanel.SetActive(true);
        GetUpgradeDataToButtons();
    }

    private void CloseLevelUpPanel()
    {
        upgradePanel.SetActive(false);
        ResumeGame();
    }

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

    private void Upgrade(int upgradeId)
    {
        var upgradeData=_selectableUpgrades[upgradeId];
        GameEvents.PublishUpgradeSelected(upgradeData);
        upgradeDataList.Remove(upgradeData);
        if(upgradeData.upgradeToAddToList!=null) upgradeDataList.Add(upgradeData.upgradeToAddToList);
        _selectableUpgrades.Clear();
        CloseLevelUpPanel();
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
    }
}