using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private LayerMask _enemyMask;

    [SerializeField] protected float _meshResolution = 3f; // 각도당 ray 수
    [SerializeField] protected int _edgeResolveIterations = 5; // 장애물 경계 탐색 반복 횟수
    [SerializeField] protected float _edgeDistanceThreshold = 0.3f;

    [Space(10)]
    [Header("Sector")]
    [SerializeField] private MeshFilter _sectorMeshFilter;
    [SerializeField] private float _sectorViewRadius = 20f;
    [SerializeField, Range(0, 360)] private float _sectorViewAngle = 70;

    [Space(10)]
    [Header("Circle")]
    [SerializeField] private MeshFilter _circleMeshFilter;
    [SerializeField] private int _circleRayCount = 60;
    [SerializeField] private float _circleViewRadius;

    private HashSet<Enemy> _previousEnemy;
    private HashSet<Enemy> _currentEnemy;

    private Mesh _circleFovMesh;
    private Mesh _sectorFovMesh;


    private void Awake()
    {
        _currentEnemy = new HashSet<Enemy>();
        _previousEnemy = new HashSet<Enemy>();

        _circleFovMesh = new Mesh();
        _circleMeshFilter.mesh = _circleFovMesh;

        _sectorFovMesh = new Mesh();
        _sectorMeshFilter.mesh = _sectorFovMesh;
    }

    private void LateUpdate()
    {
        DrawSectorFOV();
        DrawCircleFOV();

        _currentEnemy.Clear();
        UpdateEnemyVisibility(_sectorViewRadius, _sectorViewAngle / 2f);
        UpdateEnemyVisibility(_circleViewRadius, 180f);

        foreach (Enemy enemy in _currentEnemy)
        {
            enemy.SetVisible(true);
        }

        foreach(Enemy enemy in _previousEnemy)
        {
            if(enemy != null && !_currentEnemy.Contains(enemy))
            {
                enemy.SetVisible(false);
            }
        }

        (_currentEnemy, _previousEnemy) = (_previousEnemy, _currentEnemy);
    }

    protected void BuildMesh(List<Vector3> viewPoints, Mesh targetMesh)
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

        targetMesh.Clear();
        targetMesh.vertices = vertices;
        targetMesh.triangles = triangles;
        targetMesh.RecalculateNormals();
    }

    private void DrawSectorFOV()
    {
        int rayCount = Mathf.RoundToInt(_sectorViewAngle * _meshResolution);
        float anglePerRay = _sectorViewAngle / rayCount;

        var viewPoints = new List<Vector3>(rayCount + 2);
        ViewCastInfo prevCast = default;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.y - _sectorViewAngle / 2f + anglePerRay * i;
            ViewCastInfo cast = ViewCast(angle, _sectorViewRadius);

            // 장애물 경계부분 정밀 보정
            if (i > 0)
            {
                bool distExceeded = Mathf.Abs(prevCast.Distance - cast.Distance) > _edgeDistanceThreshold;

                if (prevCast.Hit != cast.Hit
                    || (prevCast.Hit && cast.Hit && distExceeded))
                {
                    EdgeInfo edge = FindEdge(prevCast, cast);
                    if (edge.PointA != Vector3.zero) viewPoints.Add(edge.PointA);
                    if (edge.PointB != Vector3.zero) viewPoints.Add(edge.PointB);
                }
            }

            viewPoints.Add(cast.Point);
            prevCast = cast;
        }

        BuildMesh(viewPoints, _sectorFovMesh);

    }

    private void DrawCircleFOV()
    {
        float anglePerRay = 360f / _circleRayCount;
        var viewPoints = new List<Vector3>(_circleRayCount + 1);

        for (int i = 0; i < _circleRayCount + 1; i++)
        {
            float angle = anglePerRay * i;
            ViewCastInfo cast = ViewCast(angle, _circleViewRadius);
            viewPoints.Add(cast.Point);
        }

        BuildMesh(viewPoints, _circleFovMesh);

    }

    protected void UpdateEnemyVisibility(float radius, float angle)
    {
        Collider[] eniemiesCollider = Physics.OverlapSphere(transform.position, radius, _enemyMask);

        foreach (Collider col in eniemiesCollider)
        {
            bool isVisible = IsVisible(col.transform.position, radius, angle);

            if (isVisible)
            {
                _currentEnemy.Add(col.GetComponent<Enemy>());
            }
        }
    }

    private bool IsVisible(Vector3 targetPos, float radius, float angle)
    {
        Vector3 dirToTarget = (targetPos - transform.position);

        // 시야각
        if (Vector3.Angle(transform.forward, dirToTarget) > angle)
        {
            return false;
        }

        float dist = dirToTarget.magnitude;

        // 장애물
        if (Physics.Raycast(transform.position, dirToTarget.normalized, dirToTarget.magnitude, _obstacleMask))
        {
            return false;
        }

        return true;
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

    private EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast)
    {
        float minAngle = minCast.Angle;
        float maxAngle = maxCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float midAngle = (minAngle + maxAngle) / 2f;
            ViewCastInfo midCast = ViewCast(midAngle, _sectorViewRadius);

            bool distExceeded = Mathf.Abs(minCast.Distance - midCast.Distance) > _edgeDistanceThreshold;

            if (midCast.Hit == minCast.Hit
                && !distExceeded)
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

    protected readonly struct ViewCastInfo
    {
        public readonly bool Hit;
        public readonly Vector3 Point;
        public readonly float Distance;
        public readonly float Angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            Hit = hit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    protected readonly struct EdgeInfo
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;

        public EdgeInfo(Vector3 a, Vector3 b)
        {
            PointA = a;
            PointB = b;
        }
    }
}
