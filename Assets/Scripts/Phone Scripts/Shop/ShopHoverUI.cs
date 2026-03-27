using UnityEngine;
using TMPro;

public class ShopHoverUI : MonoBehaviour
{
    public static ShopHoverUI Instance { get; private set; }

    [Header("UI")]
    public GameObject rootPanel;
    public TMP_Text descriptionText;
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
    }

    public void Show(string description)
    {
        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (descriptionText != null)
            descriptionText.text = description;

        if (warningText != null)
            warningText.text = defaultWarning;
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }
}