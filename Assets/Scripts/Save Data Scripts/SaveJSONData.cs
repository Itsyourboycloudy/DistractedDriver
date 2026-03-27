using UnityEngine;
using System.IO;

public class SaveJSONData : MonoBehaviour
{
    private string path = "";

    private void Start()
    {
        SetPath();
    }

    private void SetPath()
    {
        // one file per run with timestamp
        path = Application.persistentDataPath + "/TaxiRun_" +
               System.DateTime.UtcNow.ToLocalTime().ToString("M-d-yy-HH-mm") +
               ".json";

        // If you want always same file, use:
        // path = Application.persistentDataPath + "/TaxiSaveData.json";
    }

    public void SaveDataNow()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("SaveJSONData: No GameManager.Instance, can't save.");
            return;
        }

        TaxiGameData data = GameManager.Instance.CreateSaveData();

        string json = JsonUtility.ToJson(data, true); // pretty print
        File.WriteAllText(path, json);

        Debug.Log("Saved taxi data to: " + path);
        Debug.Log("JSON: " + json);
    }
}

