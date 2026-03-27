using UnityEngine;

public class PhoneMinigameHub : MonoBehaviour
{
    [Header("Where the minigame prefab spawns")]
    public Transform host;

    private GameObject currentInstance;
    private IMinigame currentGame;

    public void LoadMinigame(GameObject minigamePrefab)
    {
        UnloadCurrent();

        currentInstance = Instantiate(minigamePrefab, host);
        currentGame = currentInstance.GetComponentInChildren<IMinigame>();

        if (currentGame == null)
            Debug.LogError("Minigame prefab has no IMinigame component!");

        currentGame?.Begin();
    }

    public void UnloadCurrent()
    {
        if (currentInstance != null)
            Destroy(currentInstance);

        currentInstance = null;
        currentGame = null;
    }
}