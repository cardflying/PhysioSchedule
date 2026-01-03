using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PricePanel : PanelSystem
{
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private PriceUI priceUI;
    [SerializeField]
    private RectTransform priceContainer;
    [SerializeField]
    private List<SessionPrice> sessionList = new List<SessionPrice>();

    private ClientData _updateClientData;

    private Action<ClientData> editTriggerAction;
    private Action<PanelSystem, string> sceneTriggerCallback;

    public async UniTask Init(Action<ClientData> editTrigger, Action<PanelSystem, string> sceneTrigger)
    {
        editTriggerAction = editTrigger;
        sceneTriggerCallback = sceneTrigger;

        for (int i = 0; i < sessionList.Count; i++)
        {
            int index = i;

            PriceUI newPriceUI = Instantiate(priceUI, priceContainer);
            newPriceUI.Init(sessionList[i].description, sessionList[i].price, () => SelectPrice(index));
        }

        await UniTask.CompletedTask;
    }

    public override void SetData(string data)
    {
        _updateClientData = DeserializeData(data);
    }

    public override void Show()
    {
        base.Show();

        _closeButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[0], SerializeData(_updateClientData));
            }
        });
    }

    public override void Hide()
    {
        base.Hide();
        _updateClientData = null;
        _closeButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Choose option and add session
    /// </summary>
    /// <param name="index"></param>
    private void SelectPrice(int index)
    {
        _updateClientData.Session += sessionList[index].session;

        if (editTriggerAction != null)
        {
            editTriggerAction(_updateClientData);
        }

        _closeButton.onClick.Invoke();
    }

    [Serializable]
    private class SessionPrice
    {
        public string description;
        public int price;
        public int session;
    }
}
