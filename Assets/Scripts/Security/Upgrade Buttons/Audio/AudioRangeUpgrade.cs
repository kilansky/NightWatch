using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioRangeUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    private UpgradeManager upgradeInfo;

    public override void Start()
    {
        upgradeInfo = UpgradeManager.Instance;
        upgradeCost = upgradeInfo.audioRangeCost;
        upgradeDescription = upgradeInfo.audioRangeDescription;
        base.Start(); //Set cost text value
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); //Set description box text

        //Show audio range increase
        float sensorRange = SecuritySelection.Instance.selectedObject.audioDetection.detectionRange;
        SecuritySelection.Instance.selectedObject.audioDetection.SetSensorRange(sensorRange + upgradeInfo.audioRangeIncreaseAmt);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        //Hide audio range increase
        float sensorRange = SecuritySelection.Instance.selectedObject.audioDetection.detectionRange;
        SecuritySelection.Instance.selectedObject.audioDetection.SetSensorRange(sensorRange - upgradeInfo.audioRangeIncreaseAmt);
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase audio range
        float sensorRange = SecuritySelection.Instance.selectedObject.audioDetection.detectionRange;
        SecuritySelection.Instance.selectedObject.audioDetection.SetSensorRange(sensorRange + upgradeInfo.audioRangeIncreaseAmt);
    }
}
