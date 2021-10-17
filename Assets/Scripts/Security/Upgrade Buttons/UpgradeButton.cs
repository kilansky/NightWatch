using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int upgradeCost = 0;
    [HideInInspector] public string upgradeDescription;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI upgradeDescriptionText;

    public virtual void Start()
    {
        upgradeCostText.text = "$" + upgradeCost.ToString();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        upgradeDescriptionText.text = upgradeDescription;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {

    }

    public virtual void ButtonClicked()
    {
        MoneyManager.Instance.SubtractMoney(upgradeCost);
    }
}
