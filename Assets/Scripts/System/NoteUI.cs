using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    [SerializeField]
    private Image _bgPanel;
    [SerializeField]
    private TMP_Text _dateText;
    [SerializeField]
    private TMP_InputField _descriptionInputField;
    [SerializeField]
    private Button _deleteButton;

    private int _id;

    public Action<GameObject> deleteCallback;

    public void Init(int id, Color color, string date, string description, Action<int,string,string> descriptionCallback)
    {
        _id = id;
        _bgPanel.color = color;

        _dateText.text = (string.IsNullOrEmpty(date)) ? DateTime.Now.ToString("g") : date;

        _descriptionInputField.text = (string.IsNullOrEmpty(description)) ? "" : description;
        _descriptionInputField.onSubmit.AddListener((x) =>
        {
            descriptionCallback(_id, _dateText.text, x);
        });

        _deleteButton.onClick.AddListener(() => { deleteCallback(gameObject); });

        gameObject.SetActive(true);
    }

    public void ResetData()
    {
        _id = -1;
        _dateText.text = "";
        _descriptionInputField.text = "";
        _descriptionInputField.onSubmit.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();
        deleteCallback = null;

        gameObject.SetActive(false);
    }

    public Color GetBgColor()
    {
        return _bgPanel.color;
    }
    public string GetDescription()
    {
        return _descriptionInputField.text;
    }

    public int GetId()
    {
        return _id;
    }
}
