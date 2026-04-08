using UnityEngine;

public class TimeStopMaterialDebug : MonoBehaviour
{
    public Material mat;

    void Update()
    {
        if (mat == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mat.SetFloat("_BWBlend", 1f);
            mat.SetFloat("_InvertStrength", 0f);
            mat.SetFloat("_WaveRadius", 2f);
            Debug.Log("Forced BW ON");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mat.SetFloat("_BWBlend", 0f);
            mat.SetFloat("_InvertStrength", 0f);
            mat.SetFloat("_WaveRadius", 0f);
            Debug.Log("Reset effect");
        }
    }
}