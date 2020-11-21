using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MiscHelper
{
    public static bool IsVisibleFrom_Renderer(this Renderer parent, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, parent.bounds);
    }

    public static bool IsVisibleFrom_BoxCollider2D(this BoxCollider2D parent, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, parent.bounds);
    }
}
