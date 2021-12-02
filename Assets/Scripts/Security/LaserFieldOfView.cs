using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFieldOfView : MonoBehaviour
{
    [Range(0, 200)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Transform detectionBox;
    public bool drawBox;

    public List<Transform> visibleTargets = new List<Transform>();
    public List<GameObject> waypoints = new List<GameObject>();

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;
    

    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    public float maskCutawayDist = 0.25f;

    public bool facialRecognition = false;

    private void Start()
    {
        
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", 0.25f);
    }

    private void LateUpdate()
    {
        //DrawFieldOfView();
        if (GameManager.Instance.nightWatchPhase)
        {
            print("Find targets");
            FindVisibleTargets();
        }
    }

    public IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            
                
        }
    }

    //Find all 'targets' such as thieves or doors within this object's field of view
    public virtual void FindVisibleTargets()
    {

        //RemoveWaypoints();
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200f, obstacleMask))
        {
            print("Hit Point is " + hit.point);
            if ((transform.position.x - hit.point.x) != 0)
            {
                print((transform.position.x - hit.point.x) * 1 + " > " + (transform.position.z - hit.point.z) * 1);
                detectionBox.position = new Vector3(transform.position.x - ((transform.position.x - hit.point.x) / 2), transform.position.y, transform.position.z);
                detectionBox.localScale = new Vector3((transform.position.x - hit.point.x), 5, 6);
            }
            else
            {
                print((transform.position.x - hit.point.x) * 1 + " < " + (transform.position.z - hit.point.z) * 1);
                print("Z = " + transform.position.z + " while hit Z is " + hit.point.z);
                detectionBox.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - ((transform.position.z - hit.point.z) / 2));
                detectionBox.localScale = new Vector3(6, 5, (transform.position.z - hit.point.z));
            }

        }
        visibleTargets.Clear(); //clear the current list of existing targets


        print("Setting Collider");
        //Get an array of all targets within a box radius
        Collider[] targetsInViewRadius = Physics.OverlapBox(detectionBox.position, detectionBox.localScale, Quaternion.identity, targetMask);
        int w = 0;
        print("Checking found objects");
        while (w < targetsInViewRadius.Length)
        {
            print("Target is in box");
        }
        //Check each target found to see if they are within view
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            print("Target is in box");
            Transform target = targetsInViewRadius[i].transform; //Get target transform

            //Check if target is within the 'viewAngle'

            visibleTargets.Add(target); //Target is visible!


            if (target.gameObject.GetComponent<Waypoints>())
            {
                waypoints.Add(target.gameObject);
                print("Laser is adding waypoint");
                target.gameObject.GetComponent<Waypoints>().security.Add(transform.parent.gameObject);

            }

            Vector3 dirToTarget = (target.position - transform.position).normalized; //Get vector towards target
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //Perform raycast to make sure target is not behind a wall
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {

                }
            }
            else
            {
                print("Vector3 angle is " + Vector3.Angle(transform.forward, dirToTarget) + " and view angle " + viewAngle);
            }

        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (drawBox)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(detectionBox.position, detectionBox.localScale);
    }
}

/* public void RemoveWaypoints()
 {
     for (int w = 0; w < waypoints.Count; w++)
     {
         if (transform.parent.GetComponent<SecurityMeasure>().securityType == SecurityMeasure.SecurityType.laser)
         {
             print("Laser is removing waypoint");
         }
         waypoints[w].GetComponent<Waypoints>().security.Remove(transform.parent.gameObject);

     }
     waypoints.Clear();
 }*/

//Returns a vector3 pointing in the direction of the given angle
/* public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
 {
     if (!angleIsGlobal)
         angleInDegrees += transform.eulerAngles.y;

     return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
 }

 //Splits the view radius into many rays, and then draws a mesh using verticies and triangles
 private void DrawFieldOfView()
 {
     int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
     float stepAngleSize = viewAngle / stepCount;
     List<Vector3> viewPoints = new List<Vector3>();

     ViewCastInfo oldViewCast = new ViewCastInfo();

     for (int i = 0; i < stepCount; i++)
     {
         float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
         ViewCastInfo newViewCast = ViewCast(angle);


         if (i > 0)
         {
             bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;

             //Check if wall edge is between the last ray drawn and the new one
             if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded))
             {
                 EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                 if (edge.pointA != Vector3.zero)
                     viewPoints.Add(edge.pointA);

                 if (edge.pointB != Vector3.zero)
                     viewPoints.Add(edge.pointB);
             }
         }

         viewPoints.Add(newViewCast.point);
         oldViewCast = newViewCast;
     }

     int vertexCount = viewPoints.Count + 1;
     Vector3[] verticies = new Vector3[vertexCount];
     int[] triangles = new int[(vertexCount - 2) * 3];

     verticies[0] = Vector3.zero;
     for (int i = 0; i < vertexCount - 1; i++)
     {
         verticies[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector3.forward * maskCutawayDist);

         if (i < vertexCount - 2)
         {
             triangles[i * 3] = 0;
             triangles[i * 3 + 1] = i + 1;
             triangles[i * 3 + 2] = i + 2;
         }
     }

     viewMesh.Clear();
     viewMesh.vertices = verticies;
     viewMesh.triangles = triangles;
     viewMesh.RecalculateNormals();
 }

 //Finds a raycast point that will closely align with the edge of a wall - allows for sharper, cleaner triangles drawn at wall edges
 EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
 {
     float minAngle = minViewCast.angle;
     float maxAngle = maxViewCast.angle;
     Vector3 minPoint = Vector3.zero;
     Vector3 maxPoint = Vector3.zero;

     for (int i = 0; i < edgeResolveIterations; i++)
     {
         float angle = (minAngle + maxAngle) / 2;
         ViewCastInfo newViewCast = ViewCast(angle);

         bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
         if (newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
         {
             minAngle = angle;
             minPoint = newViewCast.point;
         }
         else
         {
             maxAngle = angle;
             maxPoint = newViewCast.point;
         }
     }

     //at this point, min and max point should be very close to the wall edge
     return new EdgeInfo(minPoint, maxPoint);
 }

 ViewCastInfo ViewCast(float globalAngle)
 {
     Vector3 dir = DirFromAngle(globalAngle, true);
     RaycastHit hit;

     if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
         return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
     else
         return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
 }

 void OnDrawGizmos()
 {
     Gizmos.color = Color.red;
     //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
     if (drawBox)
         //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
         Gizmos.DrawWireCube(detectionBox.position, detectionBox.localScale);
 }

 public struct ViewCastInfo
 {
     public bool hit;
     public Vector3 point;
     public float dist;
     public float angle;

     public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
     {
         hit = _hit;
         point = _point;
         dist = _dist;
         angle = _angle;
     }
 }

 public struct EdgeInfo
 {
     public Vector3 pointA;
     public Vector3 pointB;

     public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
     {
         pointA = _pointA;
         pointB = _pointB;
     }
 }

}*/
