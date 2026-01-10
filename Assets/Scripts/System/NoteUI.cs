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

    public Action<GameObject> deleteCallback;

    public void Init(Color color)
    {
        _bgPanel.color = color;

        _dateText.text = DateTime.Now.ToString("g");

        _deleteButton.onClick.AddListener(() => { deleteCallback(gameObject); });

        gameObject.SetActive(true);
    }

    public void ResetData()
    {
        _dateText.text = "";
        _descriptionInputField.text = "";
        _deleteButton.onClick.RemoveAllListeners();
        deleteCallback = null;

        gameObject.SetActive(false);
    }

    public Color GetBgColor()
    {
        return _bgPanel.color;
    }
}
