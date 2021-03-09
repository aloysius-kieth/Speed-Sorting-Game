using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSFX : MonoBehaviour
{
    public TrinaxAudioManager.AUDIOS clip;
    Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { OnValueChanged(toggle); });
    }

    void OnValueChanged(Toggle toggle)
    {
        TrinaxAudioManager.Instance.PlayUISFX(clip, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
    }
}
