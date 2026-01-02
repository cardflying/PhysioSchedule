using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ClientUI : MonoBehaviour
{
    [SerializeField]
    private Button clickButton;
    [SerializeField]
    private TMP_Text displayNameText;
    [SerializeField]
    private TMP_Text displayInfoText;

    string displaySession;

    public void Init(ClientData clientData, Action eventCallback)
    {
        if (clientData.Session != 0)
        {
            displaySession = clientData.Session + " Sessions   ACTIVE";
        }
        else
        {
            displaySession = "INACTIVE";
        }

        displayNameText.text = clientData.Name + " " + clientData.IC;
        displayInfoText.text = (UserCondition)clientData.Condition + "   " + displaySession;
        clickButton.onClick.AddListener(() => eventCallback());

        gameObject.SetActive(true);
    }

    public void Disable()
    {
        clickButton.onClick.RemoveAllListeners();
        displayNameText.text = string.Empty;
        displayInfoText.text = string.Empty;
        gameObject.SetActive(false);
    }
}
