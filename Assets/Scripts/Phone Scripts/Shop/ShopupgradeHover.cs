using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgradeHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ShopHoverUI.Instance != null)
            ShopHoverUI.Instance.Show(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ShopHoverUI.Instance != null)
            ShopHoverUI.Instance.Hide();
    }
}