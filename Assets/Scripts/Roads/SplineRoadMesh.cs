using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SplineRoadMesh : MonoBehaviour
{
    [Header("Road Shape")]
    [Min(0.1f)] public float roadWidth = 40f;
    public float yOffset = -0.02f;

    [Header("Sampling")]
    [Min(2)] public int samples = 24;

    [Header("UVs")]
    [Min(0.01f)] public float textureTilingLength = 120f;

    [Header("Trim")]
    [Min(0f)] public float trimStart = 0f;
    [Min(0f)] public float trimEnd = 0f;

    [Header("Extras")]
    public bool generateMeshCollider = true;

    private SplineContainer splineContainer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;

    private void OnEnable()
    {
        Cache();
        Rebuild();
    }

    private void OnValidate()
    {
        Cache();
        Rebuild();
    }

    [ContextMenu("Rebuild Road")]
    public void Rebuild()
    {
        Cache();

        if (splineContainer == null || splineContainer.Spline == null || splineContainer.Spline.Count < 2)
            return;

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Spline Road Mesh";
        }
        else
        {
            mesh.Clear();
        }

        int sampleCount = Mathf.Max(2, samples);
        float halfWidth = roadWidth * 0.5f;

        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        List<Vector2> uvs = new();
        List<int> triangles = new();

        float startT = Mathf.Clamp01(trimStart);
        float endT = 1f - Mathf.Clamp01(trimEnd);

        if (endT <= startT)
            endT = startT + 0.01f;

        float totalDistance = 0f;
        Vector3 lastPos = Vector3.zero;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = Mathf.Lerp(startT, endT, i / (float)(sampleCount - 1));

            splineContainer.Evaluate(t, out float3 posWS, out float3 tangentWS, out float3 upWS);

            Vector3 pos = transform.InverseTransformPoint((Vector3)posWS);
            Vector3 tangent = transform.InverseTransformDirection((Vector3)tangentWS).normalized;
            Vector3 up = transform.InverseTransformDirection((Vector3)upWS).normalized;
            Vector3 right = Vector3.Cross(up, tangent).normalized;

            pos += up * yOffset;

            Vector3 leftPoint = pos - right * halfWidth;
            Vector3 rightPoint = pos + right * halfWidth;

            if (i > 0)
                totalDistance += Vector3.Distance(lastPos, pos);

            lastPos = pos;

            vertices.Add(leftPoint);
            vertices.Add(rightPoint);

            normals.Add(Vector3.up);
            normals.Add(Vector3.up);

            float v = totalDistance / textureTilingLength;
            uvs.Add(new Vector2(0, v));
            uvs.Add(new Vector2(1, v));

            if (i < sampleCount - 1)
            {
                int baseIndex = i * 2;

                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);

                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 3);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;

        if (generateMeshCollider)
        {
            if (meshCollider == null)
                meshCollider = GetComponent<MeshCollider>();

            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

    private void Cache()
    {
        if (splineContainer == null) splineContainer = GetComponent<SplineContainer>();
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshCollider == null) meshCollider = GetComponent<MeshCollider>();
    }
}