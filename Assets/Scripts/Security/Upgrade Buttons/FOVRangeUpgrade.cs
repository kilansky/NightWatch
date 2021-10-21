using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FOVRangeUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    public override void Start()
    {
        base.Start(); //Set cost text value
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); //Set description box text

        //Show FOV range increase
        if ((!MaxUpgradeReached()))
            SecuritySelection.Instance.selectedObject.visionCone.viewRadius += upgradeButtonInfo.increaseAmt;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        //Hide FOV range increase
        if (!MaxUpgradeReached())
            SecuritySelection.Instance.selectedObject.visionCone.viewRadius -= upgradeButtonInfo.increaseAmt;
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase FOV range
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius += upgradeButtonInfo.increaseAmt;
    }
}
