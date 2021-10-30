using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioRangeUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
{
    public override void Start()
    {
        base.Start(); //Set cost text value
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData); //Set description box text

        //Show audio range increase
        if ((!MaxUpgradeReached()))
        {
            float sensorRange = SecuritySelection.Instance.selectedObject.audioDetection.detectionRange;
            SecuritySelection.Instance.selectedObject.audioDetection.SetSensorRange(sensorRange + upgradeButtonInfo.increaseAmt);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        //Hide audio range increase
        if ((!MaxUpgradeReached()))
        {
            float sensorRange = SecuritySelection.Instance.selectedObject.audioDetection.detectionRange;
            SecuritySelection.Instance.selectedObject.audioDetection.SetSensorRange(sensorRange - upgradeButtonInfo.increaseAmt);
        }
    }

    public override void ButtonClicked()
    {
        base.ButtonClicked();

        //Increase audio range
        float sensorRange = SecuritySelection.Instance.selectedObject.audioDetection.detectionRange;
        SecuritySelection.Instance.selectedObject.audioDetection.SetSensorRange(sensorRange + upgradeButtonInfo.increaseAmt);
    }
}
