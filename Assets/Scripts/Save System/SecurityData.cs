using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SecurityData
{
    public List<GameObject> securityObjects = new List<GameObject>();
    public List<Transform> securityPlacements = new List<Transform>();
    public int objNum;

    public SecurityData(LevelManager data)
    {
        objNum = data.securityObjects.Count;
        for(int i = 0; i < objNum; i++)
        {
            securityObjects.Add(data.securityObjects[i]);
            securityPlacements.Add(data.securityPlacements[i]);
        }
    }
}
