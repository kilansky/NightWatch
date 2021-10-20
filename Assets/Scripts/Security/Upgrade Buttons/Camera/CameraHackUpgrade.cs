using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraHackUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
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
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase camera hack defense value
        SecuritySelection.Instance.selectedObject.GetComponent<HackedSecurityScript>().hackResistance += 1;

        //Disable upgrade button if max hack defense value reached
        if(SecuritySelection.Instance.selectedObject.GetComponent<HackedSecurityScript>().hackResistance == 3)
            GetComponent<Button>().interactable = false;
    }
}
