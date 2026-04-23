using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OwnedUpgradeSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI countText;

    public void Setup(ShopUpgradeData upgradeData, int count)
    {
        if (iconImage != null)
            iconImage.sprite = upgradeData != null ? upgradeData.icon : null;

        if (countText != null)
            countText.text = "x" + count;
    }
}