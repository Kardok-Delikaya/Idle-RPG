using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private TMP_Text upgradeDescriptionText;

    //Yükseltme düğmelerindeki yazıları düzenlemek için çağırılan kod parçası
    public void HandleUpgradeData(UpgradeData upgradeData)
    {
        upgradeNameText.text = upgradeData.upgradeName;
        upgradeDescriptionText.text = upgradeData.upgradeDescription;
    }
}
