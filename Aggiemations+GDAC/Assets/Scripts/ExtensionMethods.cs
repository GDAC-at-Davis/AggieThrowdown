using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    // Vector2 project onto plane
    public static Vector2 ProjectOntoPlane(this Vector2 vector, Vector2 planeNormal)
    {
        return vector - Vector2.Dot(vector, planeNormal) * planeNormal;
    }
}
