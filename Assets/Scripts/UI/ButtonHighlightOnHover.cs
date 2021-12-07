using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonHighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI buttonText;
    public Color baseColor;
    public Color highlightColor;

    // Start is called before the first frame update
    void Start()
    {
        buttonText.color = baseColor;
    }

    void Update()
    {
        if(PlayerInputs.Instance.PauseKey)
            buttonText.color = baseColor;
    }

    //Enter Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = highlightColor;
    }

    //Exit Hover
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = baseColor;
    }

    //Click
    public void OnPointerClick(PointerEventData eventData)
    {
        buttonText.color = baseColor;
    }
}
