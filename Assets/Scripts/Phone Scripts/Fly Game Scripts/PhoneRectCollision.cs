using UnityEngine;

// Compare convex UI quads in their shared local plane, never world X/Y.
public static class PhoneRectCollision
{
    static readonly Vector2[] aCorners = new Vector2[4], bCorners = new Vector2[4];
    public static bool Overlaps(RectTransform a, RectTransform b, RectTransform space)
    {
        if (a == null || b == null || space == null || !a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy) return false;
        Corners(a, space, aCorners); Corners(b, space, bCorners);
        return !Separated(aCorners, bCorners) && !Separated(bCorners, aCorners);
    }
    static void Corners(RectTransform rect, RectTransform space, Vector2[] result)
    {
        Matrix4x4 matrix = space.worldToLocalMatrix * rect.localToWorldMatrix;
        Rect r = rect.rect;
        result[0] = matrix.MultiplyPoint3x4(new Vector3(r.xMin, r.yMin));
        result[1] = matrix.MultiplyPoint3x4(new Vector3(r.xMin, r.yMax));
        result[2] = matrix.MultiplyPoint3x4(new Vector3(r.xMax, r.yMax));
        result[3] = matrix.MultiplyPoint3x4(new Vector3(r.xMax, r.yMin));
    }
    static bool Separated(Vector2[] a, Vector2[] b)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 edge = a[(i + 1) % 4] - a[i];
            Vector2 axis = new Vector2(-edge.y, edge.x).normalized;
            if (axis.sqrMagnitude < 0.5f) return true;
            float minA = float.PositiveInfinity, maxA = float.NegativeInfinity;
            float minB = float.PositiveInfinity, maxB = float.NegativeInfinity;
            for (int j = 0; j < 4; j++)
            {
                float pa = Vector2.Dot(a[j], axis), pb = Vector2.Dot(b[j], axis);
                minA = Mathf.Min(minA, pa); maxA = Mathf.Max(maxA, pa);
                minB = Mathf.Min(minB, pb); maxB = Mathf.Max(maxB, pb);
            }
            if (maxA <= minB + 0.001f || maxB <= minA + 0.001f) return true;
        }
        return false;
    }
}
