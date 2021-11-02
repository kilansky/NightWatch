using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public List<GameObject> security = new List<GameObject>();
    public Transform[] ConnectedPoints;
    public int NumberReference;
    public float weightModifier;
    public float weight;
    private Transform ThiefPoint;
    private Transform TargetPoint;
    public bool marked;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {

    }


}
