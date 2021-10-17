using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SecurityMeasure : MonoBehaviour
{
    public enum SecurityType { camera, laser, guard, audio, patrolMarker};

    [Header("Security Measure Info")]
    public SecurityType securityType;
    public bool placedOnWalls = true; //false = place on floors - ie: guards
    public int cost;

    [Header("References")]
    public FieldOfView visionCone;
    public AudioSensor audioDetection;
}
