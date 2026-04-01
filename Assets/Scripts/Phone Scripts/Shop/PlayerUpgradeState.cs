using UnityEngine;

public class PlayerUpgradeState : MonoBehaviour
{
    public static PlayerUpgradeState Instance { get; private set; }

    [Header("Unlocks / Special Effects")]
    public bool hasTimeStop = false;
    public int removeMinigameCharges = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TriggerJokerCopy()
    {
        if (ShopManager.Instance == null) return;
        if (ShopManager.Instance.purchasedUpgrades.Count == 0) return;

        ShopUpgradeData randomOwned = ShopManager.Instance.purchasedUpgrades[
            Random.Range(0, ShopManager.Instance.purchasedUpgrades.Count)
        ];

        if (randomOwned == null) return;

        Debug.Log("Joker copied: " + randomOwned.upgradeName);
        ShopManager.Instance.PurchaseUpgrade(randomOwned);
    }
}
