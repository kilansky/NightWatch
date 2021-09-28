using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{
    public GameObject Marker; //Marker to keep track of where the patrol points are at
    public LayerMask FloorMask;
    public bool patrolPlacementMode;//REPLACE

    [HideInInspector] public List<Vector3> Points = new List<Vector3>(); //List of patrol points

    private Camera mainCamera;
    private GameObject heldMarker;
    private PatrolMarker heldMarkerScript;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        heldMarker = Instantiate(Marker, Vector3.zero - new Vector3(0, -10f, 0), Quaternion.identity);
        heldMarkerScript = heldMarker.GetComponent<PatrolMarker>();
        heldMarkerScript.markerNum = 1;
        heldMarkerScript.UpdateMarkerNum();
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolPlacementMode) //If statement that prevents patrol points from being spawned if guard isn't selected
        {
            SpawnPatrolPoint();

            if (PlayerInputs.Instance.RightClickPressed)
                patrolPlacementMode = false;
        }
    }

    private void SpawnPatrolPoint() //Spawning function
    {       
        if (PlayerInputs.Instance.LeftClickPressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
            {
                NavMeshHit NavIsHit;
                int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask))
                {                   
                    Vector3 target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    Points.Add(target);

                }
            }
        }      
    }
}
