using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PricePanel : PanelSystem
{
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private TMP_Dropdown nameListDropdown;
    [SerializeField]
    private PriceUI priceUI;
    [SerializeField]
    private RectTransform priceContainer;
    [SerializeField]
    private List<SessionPrice> sessionList = new List<SessionPrice>();

    private ClientData _updateClientData;
    private int _selectClient;
    private List<ClientData> clientList = new List<ClientData>();

    private Action<ClientData> editTriggerAction;
    private Action<PanelSystem, string> sceneTriggerCallback;
    private Func<UniTask<List<ClientData>>> loadTriggerFunc;

    public async UniTask Init(Func<UniTask<List<ClientData>>> loadTrigger, Action<ClientData> editTrigger,
                                Action<PanelSystem, string> sceneTrigger)
    {
        loadTriggerFunc = loadTrigger;
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

    public override void Show()
    {
        base.Show();

        LoadClient().Forget();

        _closeButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[0], null);
            }
        });
    }

    public override void Hide()
    {
        base.Hide();
        nameListDropdown.captionText.text = "";
        nameListDropdown.onValueChanged.RemoveAllListeners();
        nameListDropdown.options.Clear();
        _closeButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Load client list into dropdpwn menu
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadClient()
    {
        clientList = await loadTriggerFunc();
        for (int i = 0; i < clientList.Count; i++)
        {
            TMP_Dropdown.OptionData clientOption = new TMP_Dropdown.OptionData();
            clientOption.text = clientList[i].Name + "(" + clientList[i].IC + ")";
            nameListDropdown.options.Add(clientOption);
        }
        nameListDropdown.onValueChanged.AddListener(SelectClient);
    }

    private void SelectClient(int index)
    {
        nameListDropdown.captionText.text = nameListDropdown.options[index].text;
        nameListDropdown.Select();

        _selectClient = index;
    }

    private void SelectPrice(int index)
    {
        _updateClientData = clientList[_selectClient];
        _updateClientData.Session = sessionList[index].session;

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
