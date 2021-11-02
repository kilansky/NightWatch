using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefFieldofView : MonoBehaviour
{
    [Range(0, 30)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> hackableTargets = new List<Transform>();

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;

    public MeshFilter viewMeshFilter;
    public float maskCutawayDist = 0.25f;
    private Mesh viewMesh;
    private bool newTarget;
    private bool noLongerBlocked;

    private void Start()
    {
        newTarget = false;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", 0.25f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    //Find all 'targets' such as thieves or doors within this object's field of view
    private void FindVisibleTargets()
    {
        //Get an array of all targets within a sphere radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        newTarget = true; //Sets script up to be prepared to add a new target to the list.
        
        //Check each target found to see if they are within view
        //For loop is for targets within view radius
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform; //Get target transform
            Vector3 dirToTarget = (target.position - transform.position).normalized; //Get vector towards target

            //Check if target is within the 'viewAngle'
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //Perform raycast to make sure target is not behind a wall
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    //Checks if target does not have the PatrolMarker script and its camoRating is less than the thief's PerceptionStat
                    if (!target.parent.gameObject.GetComponent<PatrolMarker>())
                    {
                        if(target.parent.gameObject.GetComponent<SecurityMeasure>())
                        {
                            if(GetComponent<ThiefPathfinding>().PerceptionStat > target.parent.GetComponent<SecurityMeasure>().camoRating)
                            {
                                if(GetComponent<ThiefPathfinding>().HackingStat > target.parent.GetComponent<HackedSecurityScript>().hackResistance)
                                {
                                    //Checks if there's at least one target in the visible target list
                                    if (hackableTargets.Count > 0)
                                    {
                                        //For loop checking all objects in visibleTargets list
                                        for (int n = 0; n < hackableTargets.Count; n++)
                                        {
                                            //Checks if target is already in the visibleTargets list
                                            if (hackableTargets[n] == target.parent.transform)
                                            {
                                                newTarget = false;
                                                break;
                                            }
                                        }
                                    }
                                    //Adds target to the list if its new
                                    if (newTarget == true)
                                    {
                                        hackableTargets.Add(target.parent.transform); //Target is visible!
                                        newTarget = false;
                                    }
                                    for(int p = 0; p < GetComponent<ThiefPathfinding>().ShortestPath.Count; p++)
                                    {
                                        for(int w = 0; w < GetComponent<ThiefPathfinding>().ShortestPath[p].GetComponent<Waypoints>().security.Count; w++)
                                        {
                                            if(GetComponent<ThiefPathfinding>().ShortestPath[p].GetComponent<Waypoints>().security[w] == target.parent.gameObject)
                                            {
                                                if (target.parent.gameObject.GetComponent<HackedSecurityScript>())
                                                {
                                                    //Checks if target is within the thief's hacking range and the thief is currently not evading or performing a action
                                                    if (GetComponent<ThiefPathfinding>().currBehavior != ThiefPathfinding.BehaviorStates.Evade && GetComponent<ThiefPathfinding>().currAction == ThiefPathfinding.ActionStates.Neutral)
                                                    {
                                                        print("In Hacking Range");
                                                        //Checks if the target is not already hacked and the thief is skilled enough to hack it
                                                        if (!target.parent.gameObject.GetComponent<HackedSecurityScript>().Hacked && target.parent.gameObject.GetComponent<HackedSecurityScript>().hackResistance < GetComponent<ThiefPathfinding>().HackingStat)
                                                        {
                                                            //Activates the thief's CheckForHackableObjects function while inseting the target's parent gameObject as the gameObject
                                                            GetComponent<ThiefPathfinding>().CheckForHackableObjects(target.parent.gameObject);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Checks if there's at least one target in the visible target list
                                    if (visibleTargets.Count > 0)
                                    {
                                        //For loop checking all objects in visibleTargets list
                                        for (int n = 0; n < visibleTargets.Count; n++)
                                        {
                                            //Checks if target is already in the visibleTargets list
                                            if (visibleTargets[n] == target.parent.transform)
                                            {
                                                newTarget = false;
                                                break;
                                            }
                                        }
                                    }
                                    //Adds target to the list if its new
                                    if (newTarget == true)
                                    {
                                        visibleTargets.Add(target.parent.transform); //Target is visible!
                                        newTarget = false;
                                    }
                                }
                            }
                        }

                        if (target.parent.gameObject.GetComponent<GuardPathfinding>())
                        {
                            for (int n = 0; n < visibleTargets.Count; n++)
                            {
                                //Checks if target is already in the visibleTargets list
                                if (visibleTargets[n] == target)
                                {
                                    newTarget = false;
                                    break;
                                }
                            }
                            if (newTarget == true)
                            {
                                visibleTargets.Add(target.parent.transform); //Target is visible!
                                newTarget = false;
                            }
                        }

                        //Checks if taarget is a waypoint
                        if (target.GetComponent<Waypoints>())
                        {
                            //Checks all of thief's paths
                            for(int p = 0; p < GetComponent<ThiefPathfinding>().ShortestPath.Count; p++)
                            {
                                //Checks if waypoint is a part of thief's path
                                if(target == GetComponent<ThiefPathfinding>().ShortestPath[p])
                                {
                                    noLongerBlocked = true;
                                    //Checks all security that has detected the waypoint
                                    for (int n = 0; n < target.GetComponent<Waypoints>().security.Count; n++)
                                    {
                                        
                                        //Checks all thief's knownsecurity
                                        for (int s = 0; s < visibleTargets.Count; s++)
                                        {
                                            print(target.GetComponent<Waypoints>().security[n] + " vs " + visibleTargets[s].gameObject);
                                             //Checks if the thief and waypoint security objects are the same
                                            if (target.GetComponent<Waypoints>().security[n] == visibleTargets[s].gameObject)
                                            {
                                                if(GetComponent<ThiefPathfinding>().waypointWeights[target.GetComponent<Waypoints>().NumberReference] == 0)
                                                {
                                                    GetComponent<ThiefPathfinding>().addWeight(target.GetComponent<Waypoints>().NumberReference);
                                                    GetComponent<ThiefPathfinding>().pathIsBlocked(target.GetComponent<Waypoints>().gameObject);
                                                }
                                                noLongerBlocked = false;
                                            }
                                        }
                                        
                                    }
                                    if (noLongerBlocked && GetComponent<ThiefPathfinding>().waypointWeights[target.GetComponent<Waypoints>().NumberReference] > 0)
                                    {
                                        GetComponent<ThiefPathfinding>().removeWeight(target.GetComponent<Waypoints>().NumberReference);
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //Returns a vector3 pointing in the direction of the given angle
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
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
}
