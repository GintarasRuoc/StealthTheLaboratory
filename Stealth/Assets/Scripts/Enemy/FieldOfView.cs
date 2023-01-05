using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private const string playerTag = "Player";

    private float viewRadius;
    [SerializeField] private float awakeViewRadius = 3f;
    [SerializeField] private float phoneViewRadius = 1f;
    [Range(0, 360)]
    [SerializeField] private float viewAngle;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Transform raycastPosition;
    [SerializeField] private float playerHeight = 1.75f;

    [SerializeField] private float meshResolution;
    [SerializeField] private int edgeResolveIterations;
    [SerializeField] private float edgeDstThreshold;

    [SerializeField] private MeshFilter viewMeshFilter;
    Mesh viewMesh;

    [SerializeField] private bool showMesh = false;
    private bool mouseOver = false;
    [SerializeField] private float showDistance = 50f;

    void Start()
    {
        viewRadius = awakeViewRadius;

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        if (!showMesh)
            viewMeshFilter.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, showDistance, targetMask);
        if (colliders.Length > 0 || mouseOver)
            showFov(true);
        else showFov(false);
        if (showMesh)
            DrawFieldOfView();
    }

    // If mouse pointer is on character, show field of view
    private void OnMouseOver()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }

    private void showFov(bool show)
    {
        showMesh = show;
        viewMeshFilter.gameObject.SetActive(show);
    }

    // Check, if player is in field of view
    public Vector3 checkForPlayer()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        if (colliders.Length != 0)
            if (Vector3.Angle(raycastPosition.transform.forward, (colliders[0].transform.position - transform.position).normalized) <= viewAngle / 2)
            {
                Vector3 linecastPosition = colliders[0].transform.position;
                linecastPosition.y += playerHeight;

                if (checkForPlayerVisibilty(linecastPosition))
                    return colliders[0].transform.position;
            }

        return new Vector3();
    }

    // Check, if player isn't blocked by an object
    private bool checkForPlayerVisibilty(Vector3 linecastTarget)
    {
        RaycastHit hit;

        if (Physics.Linecast(raycastPosition.position, linecastTarget, out hit))
            if (hit.transform.CompareTag(playerTag))
                return true;
        if (linecastTarget.y == playerHeight && checkForPlayerVisibilty(linecastTarget - new Vector3(0, 1f, 0)))
            return true;
        return false;
    }

    // Change field of view radius
    public void changeViewRadius(int type)
    {
        if (type == 2)
        {
            viewRadius = awakeViewRadius;
        }
        else if (type == 1) viewRadius = phoneViewRadius;
        else viewRadius = 0f;
    }

    // Gives current field of view radius
    public float getViewRadius()
    {
        return viewRadius;
    }

    // Draw field of view mesh by Sebastian Lague
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

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

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
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

        return new EdgeInfo(minPoint, maxPoint);
    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Quaternion leftRayRotation = Quaternion.AngleAxis(-(viewAngle / 2.0f), Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis((viewAngle / 2.0f), Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * raycastPosition.forward * viewRadius;
        Vector3 rightRayDirection = rightRayRotation * raycastPosition.forward * viewRadius;

        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, leftRayDirection);
        Gizmos.DrawRay(transform.position, rightRayDirection);

        Gizmos.DrawRay(raycastPosition.position, raycastPosition.forward * viewRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
