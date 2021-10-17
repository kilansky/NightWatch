using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int upgradeCost;
    public TextMeshProUGUI upgradeCostText;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {

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
