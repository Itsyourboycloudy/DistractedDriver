using System.Collections.Generic;
using UnityEngine;

public class PauseUpgradesPanelUI : MonoBehaviour
{
    [Header("UI")]
    public Transform contentParent;
    public OwnedUpgradeSlotUI ownedUpgradeSlotPrefab;

    private void OnEnable()
    {
        RefreshUpgrades();
    }

    public void RefreshUpgrades()
    {
        ClearExisting();

        if (contentParent == null || ownedUpgradeSlotPrefab == null)
            return;

        if (ShopManager.Instance == null)
        {
            Debug.LogWarning("[PauseUpgradesPanelUI] ShopManager.Instance missing.");
            return;
        }

        if (ShopManager.Instance.purchasedUpgrades == null || ShopManager.Instance.purchasedUpgrades.Count == 0)
            return;

        Dictionary<ShopUpgradeData, int> upgradeCounts = new Dictionary<ShopUpgradeData, int>();

        for (int i = 0; i < ShopManager.Instance.purchasedUpgrades.Count; i++)
        {
            ShopUpgradeData upgrade = ShopManager.Instance.purchasedUpgrades[i];

            if (upgrade == null)
                continue;

            if (upgradeCounts.ContainsKey(upgrade))
                upgradeCounts[upgrade]++;
            else
                upgradeCounts.Add(upgrade, 1);
        }

        foreach (KeyValuePair<ShopUpgradeData, int> pair in upgradeCounts)
        {
            OwnedUpgradeSlotUI newSlot = Instantiate(ownedUpgradeSlotPrefab, contentParent);
            newSlot.Setup(pair.Key, pair.Value);
        }
    }

    private void ClearExisting()
    {
        if (contentParent == null)
            return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }
}