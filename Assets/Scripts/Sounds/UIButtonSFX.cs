using UnityEngine;

public class UIButtonSFX : MonoBehaviour
{
    public void PlayPhoneClick()
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayClick();
    }

    public void PlayBackClick()
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayBackClick();
    }

    public void PlayRideAccept()
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayRideAccept();
    }
}
