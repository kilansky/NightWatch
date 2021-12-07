using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UpgradePanel upgradePanel;

    [HideInInspector] public UpgradeButtonInfo upgradeButtonInfo;
    [HideInInspector] public int upgradeIndex = 0;

    public virtual void Start()
    {
        Button thisButton = GetComponent<Button>();

        int i = 0;
        foreach (UpgradeButtonInfo upgradeButton in upgradePanel.upgradeButtons)
        {
            if(upgradeButton.button == thisButton)
            {
                upgradeButtonInfo = upgradeButton;
                upgradeButtonInfo.costText.text = "$" + upgradeButtonInfo.cost.ToString();
                upgradeIndex = i;
                return;
            }
            i++;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        upgradePanel.descriptionText.text = upgradeButtonInfo.description;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {

    }

    public virtual void ButtonClicked()
    {
        MoneyManager.Instance.SubtractMoney(upgradeButtonInfo.cost);
        SecuritySelection.Instance.selectedObject.cost += upgradeButtonInfo.cost;
        SecuritySelection.Instance.selectedObject.timesUpgraded[upgradeIndex] += 1;
        int timesUpgraded = SecuritySelection.Instance.selectedObject.timesUpgraded[upgradeIndex];
        upgradeButtonInfo.indicatorImages[timesUpgraded - 1].color = upgradePanel.upgradedColor;

        //Check all buttons and disable them if max upgrades reached or money is too low
        upgradePanel.SetActiveButtons();

        if(SkillCheckManager.Instance)
            SkillCheckManager.Instance.UpgradePurchased();
    }

    public bool MaxUpgradeReached()
    {
        return SecuritySelection.Instance.selectedObject.timesUpgraded[upgradeIndex] >= upgradeButtonInfo.maxUpgrades;
    }
}
