using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatrolMarker : MonoBehaviour
{
    public int markerNum;
    public TextMeshProUGUI markerNumText;
    public Image markerImage;

    public void UpdateMarkerNum()
    {
        markerNumText.text = markerNum.ToString();
    }
}
