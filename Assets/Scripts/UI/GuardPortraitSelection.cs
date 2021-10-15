using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuardPortraitSelection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hoverSelectionIcon;
    public Vector3 offScreenPos = new Vector3(0, -100f, 0);

    private GuardPathfinding correspondingGuard;

    private void Start()
    {
        //Look through all guard panels to find the guard that corresponds to this portrait
        GuardPanel[] guardPanels = GuardController.Instance.guardPanels;

        for (int i = 0; i < guardPanels.Length; i++)
        {
            if (guardPanels[i].guardPortrait.gameObject == gameObject)
            {
                correspondingGuard = guardPanels[i].guard;
                return;
            }
        }
    }

    //Enter Hover over portrait
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(SecuritySelection.Instance.selectedObject != correspondingGuard.GetComponent<SecurityMeasure>())
            hoverSelectionIcon.transform.position = transform.position;
    }

    //Exit Hover over portrait
    public void OnPointerExit(PointerEventData eventData)
    {
        if (SecuritySelection.Instance.selectedObject != correspondingGuard.GetComponent<SecurityMeasure>())
            hoverSelectionIcon.transform.position = offScreenPos;
    }

    //Click on portrait - Select corresponding guard
    public void OnPointerClick(PointerEventData eventData)
    {
        SecuritySelection.Instance.SelectSecurityMeasure(correspondingGuard.transform.GetChild(0));
        hoverSelectionIcon.transform.position = offScreenPos;
    }
}
