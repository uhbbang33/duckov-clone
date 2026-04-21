using System.Collections.Generic;
using UnityEngine;

public class CircleFOV : FieldOfView
{
    [SerializeField] private int _rayCount = 60;
    [SerializeField] private float _viewRadius;

    protected override void DrawFOV()
    {
        float anglePerRay = 360f / _rayCount;
        var viewPoints = new List<Vector3>(_rayCount + 1);

        for (int i = 0; i < _rayCount + 1; i++)
        {
            float angle = anglePerRay * i;
            ViewCastInfo cast = ViewCast(angle, _viewRadius);
            viewPoints.Add(cast.Point);
        }

        BuildMesh(viewPoints);
    }
}
