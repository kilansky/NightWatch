using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PatrolMarker : MonoBehaviour
{
    public int markerNum;
    public TextMeshProUGUI markerNumText;

    public void UpdateMarkerNum()
    {
        markerNumText.text = markerNum.ToString();
    }
}
