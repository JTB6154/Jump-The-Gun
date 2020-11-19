using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FiringPoint
{
    public Transform firingPointTransform;
    public float minAngleBound;
    public float maxAngleBound;

    public Vector3 GetFiringPosition()
    {
        return firingPointTransform.position;
    }

    public Vector3 GetFlippedFiringPosition()
    {
        Vector3 local = firingPointTransform.localPosition;
        Vector3 flippedLocal = new Vector3(-local.x, local.y, local.z);
        return firingPointTransform.parent.TransformPoint(flippedLocal);
    }

    public bool IsWithinRange(float angle)
    {
        return angle >= minAngleBound && angle < maxAngleBound;
    }
}
