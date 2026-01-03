using System;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : PanelSystem
{
    [SerializeField]
    private Button _registerButton;
    [SerializeField]
    private Button _loadUserButton;
    [SerializeField]
    private Button _appointmentButton;
    [SerializeField]
    private Button _priceButton;

    private Action<PanelSystem,string> sceneTriggerCallback;

    public void Init(Action<PanelSystem,string> sceneTrigger)
    {
        sceneTriggerCallback = sceneTrigger;
    }

    public override void Show()
    {
        base.Show();

        _registerButton.onClick.AddListener(() => sceneTriggerCallback(panelSystemList[0], "0"));
        _loadUserButton.onClick.AddListener(() => sceneTriggerCallback(panelSystemList[1], null));
        _appointmentButton.onClick.AddListener(() => sceneTriggerCallback(panelSystemList[2], null));
        _priceButton.onClick.AddListener(() => sceneTriggerCallback(panelSystemList[3], null));
    }

    public override void Hide()
    {
        base.Hide();

        _registerButton.onClick.RemoveAllListeners();
        _loadUserButton.onClick.RemoveAllListeners();
        _appointmentButton.onClick.RemoveAllListeners();
        _priceButton.onClick.RemoveAllListeners();
    }
}
