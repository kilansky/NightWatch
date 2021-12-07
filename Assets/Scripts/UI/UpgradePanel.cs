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
    public Image[] indicatorImages;
}

public class UpgradePanel : MonoBehaviour
{
    public UpgradeButtonInfo[] upgradeButtons;
    public TextMeshProUGUI descriptionText;
    public Color nonUpgradedColor;
    public Color upgradedColor;

    private void Start()
    {
        SetIndicatorImageCount();
    }

    public void SetActiveButtons()
    {
        int i = 0;
        foreach (UpgradeButtonInfo upgradeButton in upgradeButtons)
        {
            if (upgradeButton.cost <= MoneyManager.Instance.money && SecuritySelection.Instance.selectedObject.timesUpgraded[i] < upgradeButton.maxUpgrades)
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

    public void SetIndicatorImageCount()
    {
        foreach (UpgradeButtonInfo upgradeButton in upgradeButtons)
        {
            for (int i = 0; i < upgradeButton.indicatorImages.Length; i++)
            {
                if (i < upgradeButton.maxUpgrades)
                {
                    upgradeButton.indicatorImages[i].gameObject.SetActive(true);
                    upgradeButton.indicatorImages[i].color = nonUpgradedColor;
                }
                else
                {
                    upgradeButton.indicatorImages[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
