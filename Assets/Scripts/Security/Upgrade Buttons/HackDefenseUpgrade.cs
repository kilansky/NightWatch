using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HackDefenseUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    public override void Start()
    {
        base.Start(); //Set cost text value
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); //Set description box text
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase hack defense value
        SecuritySelection.Instance.selectedObject.GetComponent<HackedSecurityScript>().hackResistance += (int)upgradeButtonInfo.increaseAmt;
    }
}
