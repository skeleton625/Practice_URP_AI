using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("View Setting"), Space(10)]
    [SerializeField] private float ViewRadius = 0f;
    [SerializeField] private float ViewAngle = 0f;

    public Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return Vector3.zero;
    }
}
