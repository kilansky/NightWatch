using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{
    public GameObject Marker; //Marker to keep track of where the patrol points are at
    public LayerMask FloorMask;
    public Color redMarkerColor;
    public Color greenMarkerColor;
    public Image patrolColorIndicator;
    public bool startingGuard;

    [HideInInspector] public bool patrolPlacementMode;
    [HideInInspector] public bool patrolMovementMode;
    [HideInInspector] public List<GameObject> PatrolPoints = new List<GameObject>(); //List of patrol points
    [HideInInspector] public Color patrolMarkerColor;

    private Camera mainCamera;
    private GameObject heldMarker;
    private GameObject moveMarker;
    private PatrolMarker heldMarkerScript;
    private PatrolMarker moveMarkerScript;
    private Vector3 offScreenPos = new Vector3(0, -10, 0);
    private Vector3 storedMoveMarkerPos;
    private int currMarkerNum;

    void Awake()
    {
        mainCamera = Camera.main;
        heldMarker = Instantiate(Marker, offScreenPos, Quaternion.identity);
        heldMarkerScript = heldMarker.GetComponent<PatrolMarker>();

        currMarkerNum = 1;
        heldMarkerScript.markerNum = currMarkerNum;
        heldMarkerScript.UpdateMarkerNum();

        if (startingGuard)
            SetGuardPatrolColor();
    }

    public void SetGuardPatrolColor()
    {
        patrolMarkerColor = PatrolColors.Instance.SetGuardRouteColor();
        patrolColorIndicator.color = patrolMarkerColor;
        heldMarkerScript.markerImage.color = patrolMarkerColor;
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
                    newMarker.GetComponent<PatrolMarker>().markerImage.color = patrolMarkerColor;

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

    //Begins the process of moving a placed patrol point
    public void BeginMovingPatrolPoint(GameObject patrolPoint)
    {
        patrolMovementMode = true;
        storedMoveMarkerPos = patrolPoint.transform.position;
        moveMarker = patrolPoint;
        moveMarkerScript = patrolPoint.GetComponent<PatrolMarker>();
        SecuritySelection.Instance.canSelect = false;
        SecuritySelection.Instance.CloseSelection();
    }

    //Allows player to move patrol points on the ground
    private void MovePatrolPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
        {
            //Set patrol point position at mouse cursor
            moveMarker.transform.position = hit.point;

            //Show patrol point as red/not placable
            moveMarkerScript.markerImage.color = redMarkerColor;

            //Check if hit point is a walkable area on the navmesh, and is not overlapping with other patrol points
            NavMeshHit NavIsHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask) && !moveMarker.transform.GetChild(0).GetComponent<OverlapDetection>().isOverlapping)
            {
                //Show patrol point as green/placable
                moveMarkerScript.markerImage.color = greenMarkerColor;

                //Place patrol point
                if (PlayerInputs.Instance.LeftClickPressed)
                {
                    patrolMovementMode = false;
                    moveMarkerScript.markerImage.color = patrolMarkerColor;
                    SecuritySelection.Instance.canSelect = true;
                }
            }
        }

        //Cancel moving patrol point
        if (PlayerInputs.Instance.RightClickPressed)
        {
            moveMarker.transform.position = storedMoveMarkerPos;
            moveMarkerScript.markerImage.color = patrolMarkerColor;

            patrolMovementMode = false;
            SecuritySelection.Instance.canSelect = true;
        }
    }

    public void RemovePatrolPoint(GameObject patrolPoint)
    {
        PatrolPoints.Remove(patrolPoint);
        Destroy(patrolPoint);

        currMarkerNum--;
        heldMarkerScript.markerNum = currMarkerNum;
        heldMarkerScript.UpdateMarkerNum();

        int i = 1;
        foreach (GameObject marker in PatrolPoints)
        {
            marker.GetComponent<PatrolMarker>().markerNum = i;
            marker.GetComponent<PatrolMarker>().UpdateMarkerNum();
            i++;
        }
    }
}
