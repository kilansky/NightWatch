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

    //Enables or disables interactability with each upgrade button based on its upgrade count and the player's money
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

        SetIndicatorImages();
    }

    //Sets the quantity and color of the indicators for how many upgrades can and have been purchased
    private void SetIndicatorImages()
    {
        int i = 0;
        foreach (UpgradeButtonInfo upgradeButton in upgradeButtons)
        {
            int timesUpgraded = SecuritySelection.Instance.selectedObject.timesUpgraded[i];

            for (int j = 0; j < upgradeButton.indicatorImages.Length; j++)
            {
                if (j < upgradeButton.maxUpgrades)
                {
                    upgradeButton.indicatorImages[j].gameObject.SetActive(true);
                }
                else
                {
                    upgradeButton.indicatorImages[j].gameObject.SetActive(false);
                }

                if(j < timesUpgraded)
                {
                    upgradeButton.indicatorImages[j].color = upgradedColor;
                }
                else
                {
                    upgradeButton.indicatorImages[j].color = nonUpgradedColor;
                }
            }
            i++;
        }
    }
}
