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
    [HideInInspector] public bool patrolMovementMode;
    [HideInInspector] public List<GameObject> PatrolPoints = new List<GameObject>(); //List of patrol points

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
        heldMarker.transform.GetChild(0).gameObject.SetActive(false);//disable overlap detection
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
                SecuritySelection.Instance.canSelect = true;
            }
        }

        if (patrolMovementMode)
            MovePatrolPoint();
    }

    //Allows player to place patrol points on the ground
    private void SpawnPatrolPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
        {
            //Set patrol point position at mouse cursor
            heldMarker.transform.position = hit.point;

            //Show patrol point as red/not placable
            heldMarkerScript.markerImage.color = redMarkerColor;

            //Check if hit point is a walkable area on the navmesh, and is not overlapping with other patrol points
            NavMeshHit NavIsHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask) && !heldMarker.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping)
            {
                //Show patrol point as green/placable
                heldMarkerScript.markerImage.color = greenMarkerColor;

                //Place patrol point
                if (PlayerInputs.Instance.LeftClickPressed)
                {
                    GameObject newMarker = Instantiate(Marker, hit.point, Quaternion.identity);
                    PatrolPoints.Add(newMarker);

                    PatrolMarker markerScript = newMarker.GetComponent<PatrolMarker>();
                    markerScript.markerNum = currMarkerNum;
                    markerScript.UpdateMarkerNum();
                    markerScript.connectedGuard = this;

                    currMarkerNum++;
                    heldMarkerScript.markerNum = currMarkerNum;
                    heldMarkerScript.UpdateMarkerNum();
                }
            }
        }  
    }

    public void BeginMovingPatrolPoint()
    {

    }

    //Allows player to move patrol points on the ground
    private void MovePatrolPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
        {
            //Set patrol point position at mouse cursor
            heldMarker.transform.position = hit.point;

            //Show patrol point as red/not placable
            heldMarkerScript.markerImage.color = redMarkerColor;

            //Check if hit point is a walkable area on the navmesh, and is not overlapping with other patrol points
            NavMeshHit NavIsHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask) && !heldMarker.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping)
            {
                //Show patrol point as green/placable
                heldMarkerScript.markerImage.color = greenMarkerColor;

                //Place patrol point
                if (PlayerInputs.Instance.LeftClickPressed)
                {

                }
            }
        }
    }
}
