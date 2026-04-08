using UnityEngine;
using TMPro;

public class ShopHoverUI : MonoBehaviour
{
    public static ShopHoverUI Instance { get; private set; }

    [Header("UI")]
    public GameObject rootPanel;
    public TMP_Text descriptionText;
    public TMP_Text costText;
    public TMP_Text warningText;

    [TextArea]
    public string defaultWarning = "Choosing an upgrade will cause a new minigame to appear in the Phone.";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Hide();

        if (descriptionText != null)
            descriptionText.alignment = TextAlignmentOptions.Center;

        if (costText != null)
            costText.alignment = TextAlignmentOptions.Center;

        if (warningText != null)
            warningText.alignment = TextAlignmentOptions.Center;
    }

    public void Show(string description, string costLine)
    {
        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (descriptionText != null)
            descriptionText.text = description;

        if (costText != null)
            costText.text = costLine;

        if (warningText != null)
            warningText.text = defaultWarning;
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }
}