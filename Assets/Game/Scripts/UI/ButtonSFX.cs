using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public TrinaxAudioManager.AUDIOS clip;
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { TrinaxAudioManager.Instance.PlayUISFX(clip, TrinaxAudioManager.AUDIOPLAYER.UI_SFX); });
    }
}
