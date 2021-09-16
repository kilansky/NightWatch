using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPatrolPoints : MonoBehaviour
{

    public Transform[] PatrolPoints;

    public bool GuardIsSelected;

    private Camera mainCamera;

    public LayerMask FloorMask;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main; 
    }

    // Update is called once per frame
    void Update()
    {
        if (GuardIsSelected == true)
        {
            print("Guard Is Selected");
            if (PlayerInputs.Instance.LeftClickPressed)
            {
                print("Click");
                Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.AimPosition);
                RaycastHit hit;
                print("RayHit");
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
                {
                    print("Ray Detects FloorMask");
                    NavMeshHit NavIsHit;
                    int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                    if (NavMesh.SamplePosition(hit.point, out NavIsHit, 1.0f, walkableMask))
                    {
                        print("HitNav");
                    }
                }
            }
        }
    }
}
