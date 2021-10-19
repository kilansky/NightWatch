using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuardRangeUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    private UpgradeManager upgradeInfo;

    public override void Start()
    {
        upgradeInfo = UpgradeManager.Instance;
        upgradeCost = upgradeInfo.guardRangeCost;
        upgradeDescription = upgradeInfo.guardRangeDescription;
        base.Start(); //Set cost text value
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); //Set description box text

        //Show guard range increase
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius += upgradeInfo.guardRangeIncreaseAmt;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        //Hide guard range increase
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius -= upgradeInfo.guardRangeIncreaseAmt;
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase guard range
        SecuritySelection.Instance.selectedObject.visionCone.viewRadius += upgradeInfo.guardRangeIncreaseAmt;
    }
}
