using UnityEngine;
public static class VectorUtility
{
    public static Vector4 ToVector4(this Vector3Int v, float w = default)
    {
        return new Vector4( v.x, v.y, v.z, w );
    }
    public static Vector4 ToVector4(this Vector3 v, float w = default)
    {
        return new Vector4( v.x, v.y, v.z, w );
    }
}
