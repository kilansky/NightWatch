using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int upgradeCost;
    public string upgradeDescription;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI upgradeDescriptionText;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        upgradeDescriptionText.text = upgradeDescription;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {

    }

    public virtual void ButtonClicked()
    {
        Debug.Log("Upgrade Added");
        MoneyManager.Instance.SubtractMoney(upgradeCost);
    }
}
