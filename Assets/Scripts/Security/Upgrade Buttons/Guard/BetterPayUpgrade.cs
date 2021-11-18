using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class BetterPayUpgrade : UpgradeButton, IPointerEnterHandler, IPointerExitHandler
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

        //Begin auto rotating the camera 
        SecuritySelection.Instance.selectedObject.GetComponent<NavMeshAgent>().speed += upgradeButtonInfo.increaseAmt;
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().doorOpenSpeedMod += 0.6f;
    }
}
