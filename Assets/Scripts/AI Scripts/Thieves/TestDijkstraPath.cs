using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDijkstraPath : MonoBehaviour
{
    public List<Transform> unexploredPaths = new List<Transform>(); //List of waypoints that haven't been explored yet
    public List<int> previousPath = new List<int>(); //List of waypoints that lead into the current one
    public List<Transform> ShortestPath = new List<Transform>(); //The shortest path of waypoints between the startpoint and the endpoint
    public float[] pointDistance;
    public Transform[] waypoints;
    public Transform startPoint;
    public Transform endPoint;

    private float currentPathValue;
    private int currentPathNum;
    private int resetNum;
    // Start is called before the first frame update
    void Update()
    {
        if (GameManager.Instance.nightWatchPhase && waypoints[(waypoints.Length - 1)].GetComponent<BoxCollider>().enabled == false)
        {
            print("Waypoint colliders one");
            for(int w = 0; w < waypoints.Length; w++)
            {
                waypoints[w].GetComponent<BoxCollider>().enabled = true;
            }
        }
    }
    public void FindShortestPath(GameObject thief)
    {

        previousPath.Clear();
        print("Start Finding ShortestPath");
        for(int i = 0; i < waypoints.Length; i++)
        {
            pointDistance[i] = float.MaxValue;
            unexploredPaths.Add(waypoints[i]);
            previousPath.Add(0);
            if(waypoints[i].position == startPoint.position)
            {
                pointDistance[i] = 0;
            }
        }
        while (unexploredPaths.Count > 0)
        {
            int curr = 0;
            currentPathValue = float.MaxValue;
            for (int i = 0; i < unexploredPaths.Count; i++)
            {
                if(currentPathValue > pointDistance[unexploredPaths[i].GetComponent<Waypoints>().NumberReference])
                {
                    currentPathValue = pointDistance[unexploredPaths[i].GetComponent<Waypoints>().NumberReference];
                    currentPathNum = unexploredPaths[i].GetComponent<Waypoints>().NumberReference;
                    curr = i;
                }
            }
            unexploredPaths.Remove(unexploredPaths[curr]);
            for (int n = 0; n < waypoints[currentPathNum].GetComponent<Waypoints>().ConnectedPoints.Length; n++)
            {
                int neighbor = waypoints[currentPathNum].GetComponent<Waypoints>().ConnectedPoints[n].GetComponent<Waypoints>().NumberReference;
                if(pointDistance[neighbor] > (currentPathValue + Vector3.Distance(waypoints[currentPathNum].position, waypoints[neighbor].position)))
                {
                    pointDistance[neighbor] = (currentPathValue + Vector3.Distance(waypoints[currentPathNum].position, waypoints[neighbor].position)) + thief.GetComponent<ThiefPathfinding>().waypointWeights[currentPathNum];
                    previousPath[neighbor] = currentPathNum;
                }
            }
        }
        resetNum = endPoint.GetComponent<Waypoints>().NumberReference;
        while (resetNum != startPoint.GetComponent<Waypoints>().NumberReference)
        {
            thief.GetComponent<ThiefPathfinding>().ShortestPath.Add(waypoints[resetNum]);
            resetNum = previousPath[resetNum];
        }
        thief.GetComponent<ThiefPathfinding>().ShortestPath.Add(waypoints[resetNum]);
        unexploredPaths.Clear();
        print("Finish Finding ShortestPath");
    }
    
}
