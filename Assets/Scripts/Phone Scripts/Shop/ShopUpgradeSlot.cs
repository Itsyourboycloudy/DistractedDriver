using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopUpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    public RawImage backgroundImage;
    public Image iconImage;
    public TMP_Text nameText;
    public Button upgradeButton;
    public GameObject boughtOverlay;

    [Header("Rarity Backgrounds")]
    public Texture commonBackground;
    public Texture uncommonBackground;
    public Texture rareBackground;
    public Texture epicBackground;
    public Texture legendaryBackground;

    [Header("Current Upgrade")]
    public ShopUpgradeData currentUpgrade;

    private string cachedDescription = "";
    private bool purchased = false;

    void Awake()
    {
        CacheDescription();

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(BuyUpgrade);
            upgradeButton.onClick.AddListener(BuyUpgrade);
        }
    }

    public void SetUpgrade(ShopUpgradeData upgrade)
    {
        currentUpgrade = upgrade;
        purchased = false;

        if (currentUpgrade == null)
        {
            ClearSlot();
            return;
        }

        if (iconImage != null)
            iconImage.sprite = currentUpgrade.icon;

        if (nameText != null)
            nameText.text = currentUpgrade.upgradeName;

        if (backgroundImage != null)
            backgroundImage.texture = GetBackgroundForRarity(currentUpgrade.rarity);

        if (upgradeButton != null)
            upgradeButton.interactable = true;

        if (boughtOverlay != null)
            boughtOverlay.SetActive(false);

        CacheDescription();
    }

    public void ClearSlot()
    {
        currentUpgrade = null;
        purchased = false;
        cachedDescription = "";

        if (iconImage != null)
            iconImage.sprite = null;

        if (nameText != null)
            nameText.text = "";

        if (backgroundImage != null)
            backgroundImage.texture = null;

        if (upgradeButton != null)
            upgradeButton.interactable = false;

        if (boughtOverlay != null)
            boughtOverlay.SetActive(false);
    }

    void CacheDescription()
    {
        if (currentUpgrade != null && !string.IsNullOrWhiteSpace(currentUpgrade.description))
            cachedDescription = currentUpgrade.description;
        else
            cachedDescription = "";
    }

    Texture GetBackgroundForRarity(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common: return commonBackground;
            case UpgradeRarity.Uncommon: return uncommonBackground;
            case UpgradeRarity.Rare: return rareBackground;
            case UpgradeRarity.Epic: return epicBackground;
            case UpgradeRarity.Legendary: return legendaryBackground;
        }

        return commonBackground;
    }

    void ShowHover()
    {
        if (ShopHoverUI.Instance == null) return;
        if (string.IsNullOrWhiteSpace(cachedDescription)) return;

        ShopHoverUI.Instance.Show(cachedDescription);
    }

    void HideHover()
    {
        if (ShopHoverUI.Instance == null) return;
        ShopHoverUI.Instance.Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideHover();
    }

    public void HoverEnter()
    {
        ShowHover();
    }

    public void HoverExit()
    {
        HideHover();
    }

    public void BuyUpgrade()
    {
        if (currentUpgrade == null || purchased) return;

        purchased = true;

        if (ShopManager.Instance != null)
            ShopManager.Instance.PurchaseUpgrade(currentUpgrade);

        if (upgradeButton != null)
            upgradeButton.interactable = false;

        if (boughtOverlay != null)
            boughtOverlay.SetActive(true);
    }
}