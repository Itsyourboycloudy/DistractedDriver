using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Shop Slots")]
    public ShopUpgradeSlot[] slots;

    [Header("Upgrade Pools")]
    public List<ShopUpgradeData> commonUpgrades = new List<ShopUpgradeData>();
    public List<ShopUpgradeData> uncommonUpgrades = new List<ShopUpgradeData>();
    public List<ShopUpgradeData> rareUpgrades = new List<ShopUpgradeData>();
    public List<ShopUpgradeData> epicUpgrades = new List<ShopUpgradeData>();
    public List<ShopUpgradeData> legendaryUpgrades = new List<ShopUpgradeData>();

    [Header("Rarity Chances")]
    [Range(0, 100)] public float commonChance = 40f;
    [Range(0, 100)] public float uncommonChance = 30f;
    [Range(0, 100)] public float rareChance = 15f;
    [Range(0, 100)] public float epicChance = 10f;
    [Range(0, 100)] public float legendaryChance = 5f;

    [Header("Safety")]
    public int maxAttemptsPerSlot = 50;

    [Header("Purchased Upgrades")]
    public List<ShopUpgradeData> purchasedUpgrades = new List<ShopUpgradeData>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateShop();
    }

    public void GenerateShop()
    {
        if (slots == null || slots.Length == 0) return;

        List<ShopUpgradeData> usedUpgrades = new List<ShopUpgradeData>();

        for (int i = 0; i < slots.Length; i++)
        {
            ShopUpgradeData rolledUpgrade = RollUniqueUpgrade(usedUpgrades);

            if (rolledUpgrade != null)
            {
                slots[i].SetUpgrade(rolledUpgrade);
                usedUpgrades.Add(rolledUpgrade);
            }
            else
            {
                slots[i].ClearSlot();
                Debug.LogWarning("Could not find a unique upgrade for slot " + i);
            }
        }

        if (ShopHoverUI.Instance != null)
            ShopHoverUI.Instance.Hide();
    }

    public void PurchaseUpgrade(ShopUpgradeData upgrade)
    {
        if (upgrade == null) return;

        if (!purchasedUpgrades.Contains(upgrade))
            purchasedUpgrades.Add(upgrade);

        Debug.Log("Purchased upgrade: " + upgrade.upgradeName);

        // Later:
        // unlock minigame
        // apply stat bonus
        // save bought upgrades
    }

    ShopUpgradeData RollUniqueUpgrade(List<ShopUpgradeData> usedUpgrades)
    {
        for (int attempt = 0; attempt < maxAttemptsPerSlot; attempt++)
        {
            UpgradeRarity rarity = RollRarity();
            List<ShopUpgradeData> pool = GetPoolForRarity(rarity);

            if (pool == null || pool.Count == 0)
                continue;

            ShopUpgradeData candidate = pool[Random.Range(0, pool.Count)];

            if (candidate == null)
                continue;

            if (usedUpgrades.Contains(candidate))
                continue;

            return candidate;
        }

        List<ShopUpgradeData> allUpgrades = GetAllUpgrades();

        foreach (ShopUpgradeData candidate in allUpgrades)
        {
            if (candidate == null) continue;
            if (usedUpgrades.Contains(candidate)) continue;

            return candidate;
        }

        return null;
    }

    UpgradeRarity RollRarity()
    {
        float roll = Random.Range(0f, 100f);

        if (roll < legendaryChance)
            return UpgradeRarity.Legendary;

        roll -= legendaryChance;
        if (roll < epicChance)
            return UpgradeRarity.Epic;

        roll -= epicChance;
        if (roll < rareChance)
            return UpgradeRarity.Rare;

        roll -= rareChance;
        if (roll < uncommonChance)
            return UpgradeRarity.Uncommon;

        return UpgradeRarity.Common;
    }

    List<ShopUpgradeData> GetPoolForRarity(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common: return commonUpgrades;
            case UpgradeRarity.Uncommon: return uncommonUpgrades;
            case UpgradeRarity.Rare: return rareUpgrades;
            case UpgradeRarity.Epic: return epicUpgrades;
            case UpgradeRarity.Legendary: return legendaryUpgrades;
        }

        return commonUpgrades;
    }

    List<ShopUpgradeData> GetAllUpgrades()
    {
        List<ShopUpgradeData> all = new List<ShopUpgradeData>();

        all.AddRange(commonUpgrades);
        all.AddRange(uncommonUpgrades);
        all.AddRange(rareUpgrades);
        all.AddRange(epicUpgrades);
        all.AddRange(legendaryUpgrades);

        return all;
    }
}