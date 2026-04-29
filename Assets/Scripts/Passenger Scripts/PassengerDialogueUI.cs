using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PassengerDialogueUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Typing")]
    public float characterDelay = 0.03f;

    [Header("Default Timing")]
    public float defaultShowDelay = 0.25f;
    public float defaultVisibleAfterComplete = 5f;

    private Coroutine dialogueRoutine;
    private Coroutine typingRoutine;

    private string currentFullLine = "";
    private bool isTyping = false;

    private void Awake()
    {
        HideImmediate();
    }

    private void Update()
    {
        bool advancePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);

        if (!advancePressed)
            return;

        if (isTyping)
        {
            FinishTypingImmediately();
        }
    }

    public void ShowLine(string passengerName, Sprite portrait, string line)
    {
        ShowLine(passengerName, portrait, line, defaultShowDelay, defaultVisibleAfterComplete);
    }

    public void ShowLine(string passengerName, Sprite portrait, string line, float showDelay, float visibleAfterComplete)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        dialogueRoutine = StartCoroutine(ShowLineRoutine(passengerName, portrait, line, showDelay, visibleAfterComplete));
    }

    private IEnumerator ShowLineRoutine(string passengerName, Sprite portrait, string line, float showDelay, float visibleAfterComplete)
    {
        currentFullLine = line;
        isTyping = false;

        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);

        if (portraitImage != null)
            portraitImage.sprite = portrait;

        if (nameText != null)
            nameText.text = passengerName;

        if (dialogueText != null)
            dialogueText.text = "";

        if (showDelay > 0f)
            yield return new WaitForSeconds(showDelay);

        typingRoutine = StartCoroutine(TypeLineRoutine(line));
        yield return typingRoutine;

        if (visibleAfterComplete > 0f)
            yield return new WaitForSeconds(visibleAfterComplete);

        HideImmediate();
    }

    private IEnumerator TypeLineRoutine(string line)
    {
        isTyping = true;

        if (dialogueText == null)
        {
            isTyping = false;
            yield break;
        }

        dialogueText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(characterDelay);
        }

        isTyping = false;
    }

    private void FinishTypingImmediately()
    {
        if (!isTyping)
            return;

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        if (dialogueText != null)
            dialogueText.text = currentFullLine;

        isTyping = false;
    }

    public void HideImmediate()
    {
        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        currentFullLine = "";
        isTyping = false;

        if (dialogueText != null)
            dialogueText.text = "";

        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}