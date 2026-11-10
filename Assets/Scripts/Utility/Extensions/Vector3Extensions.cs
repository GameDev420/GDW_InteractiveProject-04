using UnityEngine;

public static class Vector3Extensions
{
    // Kindly provided via Unity Forums
    // https://discussions.unity.com/t/inverselerp-for-vector3/177038/2
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }
}
