using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;

    [HideInInspector] public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //Set position of tooltip at the mouse position, and modify the pivot so it doesn't go off screen
        Vector2 mousePos = PlayerInputs.Instance.MousePosition;

        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);

        transform.position = mousePos;



        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        //Enable the layout element only when the header or content exceeds the characterWrapLimit
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    public void SetText(string content, string header = "")
    {
        //Disable the header if the value is not set
        if(string.IsNullOrEmpty(header))
            headerField.gameObject.SetActive(false);
        //Enable and set the header text
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content; //Set the content text
        UpdateElementSize(); //Set the size of the tooltip UI
    }

    //Set the maximum tooltip width to the characterWrapLimit
    private void UpdateElementSize()
    {

    }
}
