using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{
    public GameObject Marker;
    public LayerMask FloorMask;

    [HideInInspector] public List<Vector3> Points = new List<Vector3>();

    private Camera mainCamera;
    private bool GuardIsSelected;//REPLACE

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Points.Add(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (GuardIsSelected == true)
        {
            SpawnPatrolPoint();
        }
    }

    private void SpawnPatrolPoint()
    {
        
        print("Guard Is Selected");
        if (PlayerInputs.Instance.LeftClickPressed)
        {
            print("Click");
            Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
            RaycastHit hit;
            print("RayHit");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
            {
                print("Ray Detects FloorMask");
                NavMeshHit NavIsHit;
                int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                if (NavMesh.SamplePosition(hit.point, out NavIsHit, 1.0f, walkableMask))
                {
                    
                    Vector3 target = hit.point;
                    Points.Add(target);
                    Instantiate(Marker, target, Quaternion.identity);
                    print("HitNav");
                }
            }
        }
        
    }
}
