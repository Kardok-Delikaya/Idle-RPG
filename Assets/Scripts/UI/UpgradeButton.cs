using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private TMP_Text upgradeDescriptionText;

    public void HandleUpgradeData(UpgradeData upgradeData)
    {
        upgradeNameText.text = upgradeData.upgradeName;
        upgradeDescriptionText.text = upgradeData.upgradeDescription;
    }
}
