using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRangeUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    private UpgradeManager upgradeInfo;

    public override void Start()
    {
        upgradeInfo = UpgradeManager.Instance;
        upgradeCost = upgradeInfo.cameraRangeCost;
        upgradeDescription = upgradeInfo.cameraRangeDescription;
        base.Start(); //Set cost text value
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); //Set description box text

        //Show camera range increase
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius += upgradeInfo.cameraRangeIncreaseAmt;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        //Hide camera range increase
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius -= upgradeInfo.cameraRangeIncreaseAmt;
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase camera range
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius += upgradeInfo.cameraRangeIncreaseAmt;
    }
}
