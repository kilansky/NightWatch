using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{
    public GameObject Marker; //Marker to keep track of where the patrol points are at
    public LayerMask FloorMask;
    public bool MarkersVisible; //Keep visible in inspector for now for testing purposes

    // 
    [HideInInspector] public List<Vector3> Points = new List<Vector3>(); //List of patrol points

    private Camera mainCamera;
    public bool GuardIsSelected;//REPLACE

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GuardIsSelected == true) //If statement that prevents patrol points from being spawned if guard isn't selected
        {
            SpawnPatrolPoint();
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
                    if (MarkersVisible == true)
                    {
                        Instantiate(Marker, target, Quaternion.identity);
                    }
                    
                    
                }
            }
        }
        
    }
}
