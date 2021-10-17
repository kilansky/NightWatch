using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : SingletonPattern<UpgradeManager>
{
    [Header("Camera Upgrades")]
    public int cameraRangeCost = 25;
    public float cameraRangeIncreaseAmt = 2.5f;
    public string cameraRangeDescription;

    [Header("Laser Upgrades")]
    public int laserPinpointCost = 50;
    public string laserPinpointDescription;

    [Header("Guard Upgrades")]
    public int guardRangeCost = 25;
    public float guardRangeIncreaseAmt = 2.5f;
    public string guardRangeDescription;

    [Header("Audio Upgrades")]
    public int audioRangeCost = 25;
    public float audioRangeIncreaseAmt = 2.5f;
    public string audioRangeDescription;
}
