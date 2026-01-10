using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionPanel : PanelSystem
{
    [SerializeField]
    private Button _sessionUI;
    [SerializeField]
    private RectTransform _sessionContainer;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _purchaseButton;
    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;

    private ClientData _clientData;
    private Button newSessionButton;

    private List<Button> sessionList = new List<Button>();
    private List<Button> sessionPool = new List<Button>();

    private Action<PanelSystem, string> sceneTriggerCallback;

    public async UniTask Init(Action<PanelSystem, string> sceneTrigger)
    {
        sceneTriggerCallback = sceneTrigger;

        await UniTask.CompletedTask;
    }

    public override void SetData(string data)
    {
        _clientData = DeserializeData(data);
    }

    public override void Show()
    {
        base.Show();

        DisplaySession();

        _closeButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[0], "1_" + SerializeData(_clientData));
            }
        });
        _purchaseButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[1], SerializeData(_clientData));
            }
        });
    }

    public override void Hide()
    {
        base.Hide();
        _clientData = null;
        _closeButton.onClick.RemoveAllListeners();
        _purchaseButton.onClick.RemoveAllListeners();

        for (int i = 0; i < sessionList.Count; i++)
        {
            sessionList[i].gameObject.SetActive(false);
        }
        sessionPool.AddRange(sessionList);
        sessionList.Clear();
    }

    private void DisplaySession()
    {
        for (int i = 0; i < _clientData.Session; i++)
        {
            if (sessionPool.Count > 0)
            {
                newSessionButton = sessionPool[0];
                newSessionButton.gameObject.SetActive(true);
                sessionPool.RemoveAt(0);
            }
            else
            {
                newSessionButton = Instantiate(_sessionUI, _sessionContainer);
            }
            sessionList.Add(newSessionButton);
        }

        float total = sessionList.Count;
        float height = MathF.Ceiling(total / 5);

        _sessionContainer.sizeDelta = new Vector2(_sessionContainer.sizeDelta.x, _gridLayoutGroup.padding.top + height * (_gridLayoutGroup.spacing.y + _gridLayoutGroup.cellSize.y));
    }
}
