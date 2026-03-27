using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WavyTMPText : MonoBehaviour
{
    [Header("Wave")]
    public float amplitude = 8f;      // how high each letter moves
    public float frequency = 3f;      // how fast the wave moves
    public float horizontalSpacing = 0.5f; // offset between letters in the wave

    private TMP_Text textComponent;
    private Mesh mesh;
    private Vector3[] vertices;
    private TMP_TextInfo textInfo;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();
        textInfo = textComponent.textInfo;

        if (textInfo.characterCount == 0)
            return;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            Vector3[] sourceVertices = textInfo.meshInfo[i].vertices;
            Vector3[] copiedVertices = new Vector3[sourceVertices.Length];
            sourceVertices.CopyTo(copiedVertices, 0);
            textInfo.meshInfo[i].vertices = copiedVertices;
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] charVertices = textInfo.meshInfo[materialIndex].vertices;

            float wave = Mathf.Sin(Time.time * frequency + i * horizontalSpacing) * amplitude;
            Vector3 offset = new Vector3(0f, wave, 0f);

            charVertices[vertexIndex + 0] += offset;
            charVertices[vertexIndex + 1] += offset;
            charVertices[vertexIndex + 2] += offset;
            charVertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}