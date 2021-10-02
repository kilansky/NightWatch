using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SecurityMeasure : MonoBehaviour
{
    public enum SecurityType { camera, laser, guard, audio, patrolMarker};

    public SecurityType securityType;
    public bool placedOnWalls = true; //false = place on floors - ie: guards
    public int cost;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
