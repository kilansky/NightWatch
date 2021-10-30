using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinpointUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
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

        //Flip bool of laser script to send alerts at the position of the triggered thief
        SecuritySelection.Instance.selectedObject.laser.pinpointAlert = true;
    }
}
