using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NightButtonTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Count the number of guards in the scene
        int numGuards = 0;
        for (int i = 0; i < FindObjectsOfType<GuardPathfinding>().Length; i++)
            numGuards++;

        //Show the tooltip to tell the player they must purchase at least one guard to continue
        if(numGuards == 0)
        {
            TooltipSystem.Instance.ShowTooltip(header, content);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.HideTooltip();
    }
}
