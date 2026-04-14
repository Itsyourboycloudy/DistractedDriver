using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextHoverColor : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    [Header("Text")]
    public TMP_Text targetText;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color highlightColor = Color.red;

    private bool isSelected = false;

    private void Start()
    {
        RefreshColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        RefreshColor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        RefreshColor();
    }

    private void RefreshColor()
    {
        if (targetText == null)
            return;

        targetText.color = isSelected ? highlightColor : normalColor;
    }
}