using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField] protected float _meshResolution = 3f; // 각도당 ray 수
    [SerializeField] protected int _edgeResolveIterations = 5; // 장애물 경계 탐색 반복 횟수
    [SerializeField] protected float _edgeDistanceThreshold = 0.3f;

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

    protected virtual void DrawFOV() { }

    protected void BuildMesh(List<Vector3> viewPoints)
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

    protected ViewCastInfo ViewCast(float globalAngle, float radius)
    {
        Vector3 dir = DirFromAngle(globalAngle);

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, radius, _obstacleMask))
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);

        return new ViewCastInfo(false, transform.position + dir * radius, radius, globalAngle);
    }

    private Vector3 DirFromAngle(float angleDegrees)
    {
        return new Vector3(
            Mathf.Sin(angleDegrees * Mathf.Deg2Rad),
            0f,
            Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    protected readonly struct ViewCastInfo
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

    protected readonly struct EdgeInfo
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;

        public EdgeInfo(Vector3 a, Vector3 b)
        {
            PointA = a; PointB = b;
        }
    }
}
