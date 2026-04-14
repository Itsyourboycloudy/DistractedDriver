using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuHandCursor : MonoBehaviour
{
    [Header("Cursor")]
    public RectTransform handCursor;
    public float xOffset = -390f;

    [Header("Animation")]
    public float bobDistance = 8f;
    public float bobSpeed = 4f;

    [Header("Buttons")]
    public Button[] buttons;

    private int currentIndex = 0;
    private Vector3 targetBasePosition;

    private void OnEnable()
    {
        currentIndex = 0;
        StartCoroutine(InitializeCursorNextFrame());
    }

    private IEnumerator InitializeCursorNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        if (buttons != null && buttons.Length > 0)
        {
            SelectButton(currentIndex, true);
        }
    }

    private void Update()
    {
        if (buttons == null || buttons.Length == 0 || handCursor == null)
            return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = buttons.Length - 1;

            SelectButton(currentIndex, true);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex++;
            if (currentIndex >= buttons.Length)
                currentIndex = 0;

            SelectButton(currentIndex, true);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            buttons[currentIndex].onClick.Invoke();
        }

        AnimateCursor();
    }

    public void SetIndex(int index)
    {
        if (buttons == null || buttons.Length == 0)
            return;

        if (index < 0 || index >= buttons.Length)
            return;

        currentIndex = index;
        SelectButton(currentIndex, true);
    }

    private void SelectButton(int index, bool snap)
    {
        if (buttons[index] == null || handCursor == null)
            return;

        RectTransform buttonRect = buttons[index].GetComponent<RectTransform>();

        targetBasePosition = new Vector3(
            buttonRect.position.x + xOffset,
            buttonRect.position.y,
            handCursor.position.z
        );

        if (snap)
            handCursor.position = targetBasePosition;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }

    private void AnimateCursor()
    {
        float bob = Mathf.Sin(Time.unscaledTime * bobSpeed) * bobDistance;

        handCursor.position = new Vector3(
            targetBasePosition.x + bob,
            targetBasePosition.y,
            targetBasePosition.z
        );
    }
}