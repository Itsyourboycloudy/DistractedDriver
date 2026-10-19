using UnityEngine;

// Derived from the existing owned-upgrade list, so stacking and save restoration
// use the same source of truth and never compound effects during scene reloads.
public static class TaxiBuildStats
{
    public static int Count(ShopUpgradeType type)
    {
        if (ShopManager.Instance == null) return 0;
        int count = 0;
        foreach (var upgrade in ShopManager.Instance.purchasedUpgrades)
            if (upgrade != null && upgrade.upgradeType == type && ++count == 3) return 3;
        return count;
    }
    public static bool IsBuild(ShopUpgradeType type) => type == ShopUpgradeType.DriftKit || type == ShopUpgradeType.DopamineRig || type == ShopUpgradeType.PremiumMeter;
    public static float Speed => Mathf.Pow(0.92f, Count(ShopUpgradeType.DriftKit));
    public static float Boost => 1f + 0.3f * Count(ShopUpgradeType.DriftKit);
    public static float BoostDuration => 1f + 0.25f * Count(ShopUpgradeType.DriftKit);
    public static float DopamineGain => Mathf.Pow(1.35f, Count(ShopUpgradeType.DopamineRig)) * Mathf.Pow(0.85f, Count(ShopUpgradeType.PremiumMeter));
    public static float DopamineDecay => Mathf.Pow(1.2f, Count(ShopUpgradeType.DopamineRig));
    public static float DopamineTopSpeed => 1.5f + 0.12f * Count(ShopUpgradeType.DopamineRig);
    public static float Fare => (1f + 0.35f * Count(ShopUpgradeType.PremiumMeter)) * Mathf.Pow(0.92f, Count(ShopUpgradeType.DopamineRig));
    public static float Steering => Mathf.Pow(0.92f, Count(ShopUpgradeType.PremiumMeter));
}
