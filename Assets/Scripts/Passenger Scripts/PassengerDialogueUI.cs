using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PassengerDialogueUI : MonoBehaviour
{
    public Image portraitImage;
    public TMP_Text nameText;
    public float showTime = 2f;

    Coroutine routine;

    public void Show(string passengerName, Sprite portrait)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(passengerName, portrait));
    }

    IEnumerator ShowRoutine(string passengerName, Sprite portrait)
    {
        portraitImage.sprite = portrait;
        nameText.text = passengerName;

        gameObject.SetActive(true);

        yield return new WaitForSeconds(showTime);

        gameObject.SetActive(false);
    }
}
