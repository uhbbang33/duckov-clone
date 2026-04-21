using System.Collections.Generic;
using UnityEngine;

public class SectorFOV : FieldOfView
{
    [SerializeField] private float _viewRadius = 20f;
    [SerializeField, Range(0, 360)] private float _viewAngle = 70;

    protected override void DrawFOV()
    {
        int rayCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float anglePerRay = _viewAngle / rayCount;

        var viewPoints = new List<Vector3>(rayCount + 2);
        ViewCastInfo prevCast = default;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.y - _viewAngle / 2f + anglePerRay * i;
            ViewCastInfo cast = ViewCast(angle, _viewRadius);

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


    private EdgeInfo FindEdge(ViewCastInfo minCast, ViewCastInfo maxCast)
    {
        float minAngle = minCast.Angle;
        float maxAngle = maxCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float midAngle = (minAngle + maxAngle) / 2f;
            ViewCastInfo midCast = ViewCast(midAngle, _viewRadius);

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

}
