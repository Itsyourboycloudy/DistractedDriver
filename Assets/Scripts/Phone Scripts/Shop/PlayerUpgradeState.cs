using UnityEngine;

public class PlayerUpgradeState : MonoBehaviour
{
    public static PlayerUpgradeState Instance { get; private set; }

    [Header("Unlocks / Special Effects")]
    public bool hasTimeStop = false;
    public int removeMinigameCharges = 0;

    private bool jokerCopyInProgress = false;

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
        if (jokerCopyInProgress) return;
        if (ShopManager.Instance == null) return;
        if (ShopManager.Instance.purchasedUpgrades == null || ShopManager.Instance.purchasedUpgrades.Count == 0) return;

        jokerCopyInProgress = true;

        ShopUpgradeData copiedUpgrade = null;
        int safety = 50;

        while (safety-- > 0)
        {
            ShopUpgradeData randomOwned = ShopManager.Instance.purchasedUpgrades[
                Random.Range(0, ShopManager.Instance.purchasedUpgrades.Count)
            ];

            if (randomOwned == null) continue;
            if (randomOwned.upgradeType == ShopUpgradeType.Joker) continue;

            copiedUpgrade = randomOwned;
            break;
        }

        if (copiedUpgrade != null)
        {
            Debug.Log("Joker copied: " + copiedUpgrade.upgradeName);
            ShopManager.Instance.PurchaseUpgrade(copiedUpgrade);
        }
        else
        {
            Debug.Log("Joker found nothing valid to copy.");
        }

        jokerCopyInProgress = false;
    }
}