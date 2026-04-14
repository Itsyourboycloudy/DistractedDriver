using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuCursorTarget : MonoBehaviour, IPointerEnterHandler
{
    public MenuHandCursor menuHandCursor;
    public int buttonIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuHandCursor != null)
            menuHandCursor.SetIndex(buttonIndex);
    }
}