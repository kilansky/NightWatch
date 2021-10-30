using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeButtonInfo
{
    public Button button;
    [Range(0, 200)] public int cost = 25;
    public TextMeshProUGUI costText;
    public string description;
    public float increaseAmt = 2.5f;
    [Range(0, 5)] public int maxUpgrades = 3;
}

public class UpgradePanel : MonoBehaviour
{
    public UpgradeButtonInfo[] upgradeButtons;
    public TextMeshProUGUI descriptionText;

    public void SetActiveButtons()
    {
        int i = 0;
        foreach (UpgradeButtonInfo upgradeButton in upgradeButtons)
        {
            if (upgradeButton.cost <= MoneyManager.Instance.Money && SecuritySelection.Instance.selectedObject.timesUpgraded[i] < upgradeButton.maxUpgrades)
            {
                upgradeButton.button.interactable = true;
            }
            else
            {
                upgradeButton.button.interactable = false;
            }
            i++;
        }
    }
}
