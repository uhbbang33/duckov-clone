using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float _viewRadius = 20f;
    [SerializeField, Range(0, 360)] private float _viewAngle = 70;
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] private float _meshResolution = 3f; // 각도당 ray 수
    [SerializeField] private int _edgeResolveIterations = 5; // 장애물 경계 탐색 반복 횟수
    [SerializeField] private float _edgeDistanceThreshold = 0.3f;

    private MeshFilter _meshFilter;
    private Mesh _fovMesh;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _fovMesh = new Mesh { name = "FOV Mesh" };
        _meshFilter.mesh = _fovMesh;
    }

    private void LateUpdate()
    {
        DrawFOV();
    }

    private void DrawFOV()
    {
        int rayCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float anglePerRay = _viewAngle / rayCount;

        var viewPoints = new List<Vector3>(rayCount + 2);
        ViewCastInfo prevCast = default;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.y - _viewAngle / 2f + anglePerRay * i;
            ViewCastInfo cast = ViewCast(angle);

            // 장애물 경계부분 정밀 보정
            if (i > 0)
            {
                bool distExceeded = Mathf.Abs(prevCast.Distance - cast.Distance)
                                    > _edgeDistanceThreshold;

                if (prevCast.Hit != cast.Hit || (prevCast.Hit && cast.Hit && distExceeded))
                {
                    EdgeInfo edge = FindEdge(prevCast, cast);
                    if (edge.PointA != Vector3.zero) viewPoints.Add(edge.PointA);
                    if (edge.PointB != Vector3.zero) viewPoints.Add(edge.PointB);
                }
            }

            viewPoints.Add(cast.Point);
            prevCast = cast;
        }

        BuildMesh(viewPoints);
    }

    private void BuildMesh(List<Vector3> viewPoints)
    {
        int vertCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertCount];
        var triangles = new int[(vertCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertCount - 2)
            {
                triangles[i * 3 + 0] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _fovMesh.Clear();
        _fovMesh.vertices = vertices;
        _fovMesh.triangles = triangles;
        _fovMesh.RecalculateNormals();
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle);

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _viewRadius, _obstacleMask))
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);

        return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius, globalAngle);
    }

    private EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast)
    {
        float minAngle = minCast.Angle;
        float maxAngle = maxCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float midAngle = (minAngle + maxAngle) / 2f;
            ViewCastInfo midCast = ViewCast(midAngle);

            bool distExceeded = Mathf.Abs(minCast.Distance - midCast.Distance)
                                > _edgeDistanceThreshold;

            if (midCast.Hit == minCast.Hit && !distExceeded)
            {
                minAngle = midAngle;
                minPoint = midCast.Point;
            }
            else
            {
                maxAngle = midAngle;
                maxPoint = midCast.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private Vector3 DirFromAngle(float angleDegrees)
    {
        return new Vector3(
            Mathf.Sin(angleDegrees * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    private readonly struct ViewCastInfo
    {
        public readonly bool Hit;
        public readonly Vector3 Point;
        public readonly float Distance;
        public readonly float Angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            Hit = hit; Point = point; Distance = distance; Angle = angle;
        }
    }

    private readonly struct EdgeInfo
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;

        public EdgeInfo(Vector3 a, Vector3 b)
        {
            PointA = a; PointB = b;
        }
    }
}
