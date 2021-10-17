using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRangeUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Show camera range increase
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //Hide camera range increase
    }

    public override void ButtonClicked()
    {
        MoneyManager.Instance.SubtractMoney(upgradeCost);
    }
}
