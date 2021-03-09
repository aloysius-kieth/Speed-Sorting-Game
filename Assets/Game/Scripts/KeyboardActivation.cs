using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class KeyboardActivation : MonoBehaviour
{
    public GameObject keyboard;
    public GameObject overlay;
    TMP_InputField field;
    public TMP_InputField field2;

    bool keyboardOpened = false;
    bool keyboardClosed = true;

    void Awake()
    {
        field = GetComponent<TMP_InputField>();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<Button>()) return;
        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.tag == "eyeMask") return;
        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.tag == "keyboard" && EventSystem.current.currentSelectedGameObject.GetComponent<Image>()) return;

        if (field.isFocused || field2.isFocused && !keyboardOpened && keyboardClosed)
        {
            keyboardOpened = true;
            keyboardClosed = false;
            ShowKeyboard();
        }

        if (!field.isFocused && !field2.isFocused && keyboardOpened && !keyboardClosed)
        {
            keyboardOpened = false;
            keyboardClosed = true;
            HideKeyboard();
        }
    }

    async void ShowKeyboard()
    {
        await new WaitForEndOfFrame();
        if (!keyboard.activeSelf)
        {
            overlay.SetActive(true);
            keyboard.SetActive(true);
            keyboard.transform.DOLocalMoveY(-275f, 0.45f).OnComplete(() =>
            {
                keyboard.GetComponent<CanvasGroup>().interactable = true;
                keyboard.GetComponent<CanvasGroup>().blocksRaycasts = true;
            });
        }
    }

    async void HideKeyboard()
    {
        await new WaitForEndOfFrame();
        if (field.isFocused || field2.isFocused) return;
        if (keyboard.activeSelf)
        {
            keyboard.transform.DOLocalMoveY(-830f, 0.25f).OnComplete(() =>
            {
                keyboard.GetComponent<CanvasGroup>().interactable = false;
                keyboard.GetComponent<CanvasGroup>().blocksRaycasts = false; keyboard.SetActive(false);
                overlay.SetActive(false);
            });
        }
    }
}
