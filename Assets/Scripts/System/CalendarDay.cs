using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarDay : MonoBehaviour
{
    [SerializeField]
    private Button dayButton;
    [SerializeField]
    private TMP_Text dayText;
    [SerializeField]
    private Image highlightImage;
    [SerializeField]
    private CanvasGroup canvasGroup;

    public void Init()
    {
        dayButton.onClick.RemoveAllListeners();
        dayText.text = "";
        ChangeTextColor(Color.black);
        ChangeHighlightColor(Color.white);
        ShowDay(false);
    }

    public TMP_Text ChangeTextColor(Color color)
    {
        dayText.color = color;

        return dayText;
    }

    public void InsertText(string text)
    {
        dayText.text = text;
    }

    public void ChangeHighlightColor(Color color)
    {
        highlightImage.color = color;
    }

    public void ShowDay(bool show)
    {
        canvasGroup.alpha = (show)? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }

    public void EnableButton(Action callback)
    {
        dayButton.onClick.AddListener(() => callback());
    }

    public void DisableButton()
    {
        dayButton.onClick.RemoveAllListeners();
    }
}
