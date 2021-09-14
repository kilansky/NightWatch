using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapDetection : MonoBehaviour
{
    [HideInInspector] public bool isOverlapping;

    private void OnTriggerEnter()
    {
        //Debug.Log("OnTriggerEnter fired");
        isOverlapping = true;
    }

    private void OnTriggerStay()
    {
        //Debug.Log("OnTriggerStay fired");
        isOverlapping = true;
    }

    private void OnTriggerExit()
    {
        //Debug.Log("OnTriggerExit fired");
        isOverlapping = false;
    }
}
