using UnityEngine;

public class WaterScrollURP : MonoBehaviour
{
    public Renderer waterRenderer;
    public Vector2 scrollSpeed = new Vector2(0.02f, 0.01f);

    private Material mat;
    private Vector2 offset;

    void Start()
    {
        if (waterRenderer == null) waterRenderer = GetComponent<Renderer>();

        // Instance material so you don't modify the shared asset globally
        mat = waterRenderer.material;

        // Start from current offset (in case you set it in inspector)
        offset = mat.GetTextureOffset("_BaseMap");
    }

    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;
        mat.SetTextureOffset("_BaseMap", offset);
    }
}
