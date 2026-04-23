using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class PauseOptionsAudioUI : MonoBehaviour
{
    [Serializable]
    public class MixerParameterTarget
    {
        public AudioMixer mixer;
        public string parameterName;
    }

    [Serializable]
    public class SliderGroup
    {
        public string playerPrefsKey;
        [Range(0.0001f, 1f)] public float defaultValue = 1f;
        public Slider slider;
        public TextMeshProUGUI percentText;
        public MixerParameterTarget[] targets;
    }

    [Header("Slider Groups")]
    public SliderGroup masterGroup;
    public SliderGroup musicGroup;
    public SliderGroup sfxGroup;

    private void Start()
    {
        InitializeGroup(masterGroup);
        InitializeGroup(musicGroup);
        InitializeGroup(sfxGroup);
    }

    private void InitializeGroup(SliderGroup group)
    {
        if (group == null || group.slider == null)
            return;

        float loadedValue = PlayerPrefs.GetFloat(group.playerPrefsKey, group.defaultValue);

        group.slider.minValue = 0.0001f;
        group.slider.maxValue = 1f;
        group.slider.wholeNumbers = false;

        group.slider.onValueChanged.RemoveAllListeners();
        group.slider.value = loadedValue;
        group.slider.onValueChanged.AddListener((value) => OnSliderValueChanged(group, value));

        ApplyGroupValue(group, loadedValue);
    }

    private void OnSliderValueChanged(SliderGroup group, float value)
    {
        PlayerPrefs.SetFloat(group.playerPrefsKey, value);
        PlayerPrefs.Save();

        ApplyGroupValue(group, value);
    }

    private void ApplyGroupValue(SliderGroup group, float value)
    {
        float dbValue = LinearToDecibels(value);

        if (group.targets != null)
        {
            for (int i = 0; i < group.targets.Length; i++)
            {
                MixerParameterTarget target = group.targets[i];

                if (target == null || target.mixer == null || string.IsNullOrEmpty(target.parameterName))
                    continue;

                target.mixer.SetFloat(target.parameterName, dbValue);
            }
        }

        if (group.percentText != null)
        {
            int percent = Mathf.RoundToInt(value * 100f);
            group.percentText.text = percent + "%";
        }
    }

    private float LinearToDecibels(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }
}