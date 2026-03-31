using UnityEngine;

[CreateAssetMenu(fileName = "NewShopUpgrade", menuName = "Shop/Upgrade Data")]
public class ShopUpgradeData : ScriptableObject
{
    public string upgradeName;

    [TextArea(3, 6)]
    public string description;

    public Sprite icon;

    public UpgradeRarity rarity;

    [Header("Effect")]
    public ShopUpgradeType upgradeType;
    public float value = 0f;
    public int durationDays = 0; // only for temporary upgrades like Surge Pricing

    [Header("Duplicate Rules")]
    [Tooltip("If true, this upgrade is allowed to appear more than once in the same shop roll.")]
    public bool allowDuplicateInShop = false;

    [Tooltip("If false, player can only ever buy one copy of this upgrade.")]
    public bool canBuyMultiple = true;
}