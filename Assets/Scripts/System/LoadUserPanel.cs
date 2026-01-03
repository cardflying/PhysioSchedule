using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadUserPanel : PanelSystem
{
    [SerializeField]
    private Button _closeListButton;
    [SerializeField]
    private TMP_InputField _searchInputField;

    [SerializeField]
    private ClientUI _clientPrefabs;
    [SerializeField]
    private RectTransform _clientContainer;

    private ClientUI newClientPrefab;
    private float _height;
    private Action<PanelSystem,string> sceneTriggerCallback;

    private List<ClientUI> clientButtonList = new List<ClientUI>();
    private List<ClientUI> clientButtonPool = new List<ClientUI>();
    private List<ClientData> currentClientList = new List<ClientData>();

    private Func<UniTask<List<ClientData>>> loadTriggerFunc;

    public async UniTask Init(Func<UniTask<List<ClientData>>> loadTrigger, Action<PanelSystem,string> sceneTrigger)
    {
        _height = _clientPrefabs.GetComponent<RectTransform>().sizeDelta.y;

        loadTriggerFunc = loadTrigger;
        sceneTriggerCallback = sceneTrigger;

        await UniTask.CompletedTask;
    }

    public override void Show()
    {
        base.Show();

        _closeListButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[0], null);
            }
        });

        LoadClientList().Forget();
    }

    /// <summary>
    /// load client list from data
    /// </summary>
    public async UniTask LoadClientList()
    {
        currentClientList = await loadTriggerFunc();

        _searchInputField.onValueChanged.AddListener(SearchList);

        for (int i = 0; i < currentClientList.Count; i++)
        {
            int index = i;

            if (clientButtonPool.Count > 0)
            {
                newClientPrefab = clientButtonPool[0];
                clientButtonPool.RemoveAt(0);
                newClientPrefab.transform.SetParent(_clientContainer, false);
            }
            else
            {
                newClientPrefab = Instantiate(_clientPrefabs, _clientContainer);
            }

            newClientPrefab.Init(currentClientList[i], () => LoadClientData(index));

            clientButtonList.Add(newClientPrefab);
        }
        _clientContainer.sizeDelta = new Vector2(_clientContainer.sizeDelta.x, clientButtonList.Count * (_height + 5) + 40);
    }

    public override void Hide()
    {
        base.Hide();

        for (int i = 0; i < clientButtonList.Count; i++)
        {
            clientButtonList[i].Disable();
        }
        clientButtonPool.AddRange(clientButtonList);
        clientButtonList.Clear();

        _searchInputField.onValueChanged.RemoveAllListeners();
        _closeListButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Pass selected client data to display panel
    /// </summary>
    /// <param name="index"></param>
    private void LoadClientData(int index)
    {
        if (sceneTriggerCallback != null)
        {
            var item = JsonConvert.SerializeObject(currentClientList[index]);
            Debug.Log(item);
            sceneTriggerCallback(panelSystemList[1], "1_" + item);
        }
    } 

    /// <summary>
    /// Filter client list by keyword
    /// </summary>
    /// <param name="keyword"></param>
    private void SearchList(string keyword)
    {
        int matchCount = 0;

        for (int i = 0; i < currentClientList.Count; i++)
        {
            if (currentClientList[i].Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                clientButtonList[i].gameObject.SetActive(true);
                matchCount++;
            }
            else
            {
                clientButtonList[i].gameObject.SetActive(false);
            }
        }
        _clientContainer.sizeDelta = new Vector2(_clientContainer.sizeDelta.x, matchCount * (_height + 5) + 40);
    }
}
