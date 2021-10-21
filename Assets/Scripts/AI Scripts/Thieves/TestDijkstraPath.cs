using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDijkstraPath : MonoBehaviour
{
    public List<Vector3> unexploredPaths = new List<Vector3>();
    public Transform[] waypoints;
    public float[] distance;
    public Transform startPoint;
    public Transform endPoint;
    // Start is called before the first frame update
    void Start()
    {

        print("Fastest Path goes ");
    }

    private void FindShortestPath()
    {
        IDictionary<Vector3, int> distances = new Dictionary<Vector3, int>();

        for(int i = 0; i < waypoints.Length; i++)
        {
            distances.Add(new KeyValuePair<Vector3, int>(waypoints[i].position, int.MaxValue));
            unexploredPaths.Add(waypoints[i].position);
            if (waypoints[i] == startPoint)
            {
                
            }
            else
            {
                
            }

        }

        while(unexploredPaths.Count > 0)
        {
            
        }
    }
    
}
