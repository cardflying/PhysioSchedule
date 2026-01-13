using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionUI : MonoBehaviour
{
    [SerializeField]
    private Button _openNoteButton;
    [SerializeField]
    private TMP_Text _dateText;
    [SerializeField]
    private TMP_Text _noteText;

    private RectTransform _RectTransform;

    private void Awake()
    {
        _RectTransform = GetComponent<RectTransform>();
    }

    public void Init(string date, string note, Action callback)
    {
        if (string.IsNullOrEmpty(date) == false)
            _dateText.text = date;

        if (string.IsNullOrEmpty(note) == false)
            _noteText.text = note;

        float height = _noteText.preferredHeight;
        _noteText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        if (height > 300)
            _RectTransform.sizeDelta = new Vector2(_RectTransform.sizeDelta.x, height);

        _openNoteButton.onClick.AddListener(() => callback());
    }

    public void Hide()
    {
        _openNoteButton.onClick.RemoveAllListeners();
        _dateText.text = "";
        _noteText.text = "";
    }

    public void UpdateNote(string note)
    {
        _noteText.text = note;
    }
}
