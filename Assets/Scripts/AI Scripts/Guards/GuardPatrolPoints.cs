using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{
    public GameObject Marker; //Marker to keep track of where the patrol points are at
    public LayerMask FloorMask;


    [HideInInspector] public List<Vector3> Points = new List<Vector3>(); //List of patrol points

    private Camera mainCamera;
    [HideInInspector] public bool GuardIsSelected;//REPLACE

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Points.Add(transform.position); //Spawns a patrol point where the guard spawns at
        Instantiate(Marker, transform.position, Quaternion.identity); //Spawns a marker at the patrol point
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
                    
                    Vector3 target = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    Points.Add(target);
                    Instantiate(Marker, target, Quaternion.identity);
                    
                }
            }
        }
        
    }
}
