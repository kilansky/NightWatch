using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{
    public GameObject Marker; //Marker to keep track of where the patrol points are at
    public LayerMask FloorMask;
    public Color redMarkerColor;
    public Color greenMarkerColor;

    [HideInInspector] public bool patrolPlacementMode;
    [HideInInspector] public List<Vector3> Points = new List<Vector3>(); //List of patrol points

    private Camera mainCamera;
    private GameObject heldMarker;
    private Vector3 offScreenPos = new Vector3(0, -10, 0);
    private PatrolMarker heldMarkerScript;
    private Color heldMarkerColor;
    private int currMarkerNum;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        heldMarker = Instantiate(Marker, offScreenPos, Quaternion.identity);
        heldMarkerScript = heldMarker.GetComponent<PatrolMarker>();
        heldMarkerColor = heldMarkerScript.markerImage.color;

        currMarkerNum = 1;
        heldMarkerScript.markerNum = currMarkerNum;
        heldMarkerScript.UpdateMarkerNum();
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPlacementMode) //If statement that prevents patrol points from being spawned if guard isn't selected
        {
            SpawnPatrolPoint();

            if (PlayerInputs.Instance.RightClickPressed)
            {
                heldMarker.transform.position = offScreenPos;
                patrolPlacementMode = false;
            }
        }
    }

    private void SpawnPatrolPoint() //Spawning function
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
        {
            //Set patrol point position at mouse cursor
            heldMarker.transform.position = hit.point;

            //Show patrol point as red/not placable
            heldMarkerScript.markerImage.color = redMarkerColor;

            //Check if hit point is a walkable area on the navmesh
            NavMeshHit NavIsHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask))
            {
                //Show patrol point as green/placable
                heldMarkerScript.markerImage.color = greenMarkerColor;

                //Place patrol point
                if (PlayerInputs.Instance.LeftClickPressed)
                {
                    Points.Add(hit.point);
                    GameObject newMarker = Instantiate(Marker, hit.point, Quaternion.identity);
                    newMarker.GetComponent<PatrolMarker>().markerNum = currMarkerNum;
                    newMarker.GetComponent<PatrolMarker>().UpdateMarkerNum();

                    currMarkerNum++;
                    heldMarkerScript.markerNum = currMarkerNum;
                    heldMarkerScript.UpdateMarkerNum();
                }
            }
        }  
    }
}
