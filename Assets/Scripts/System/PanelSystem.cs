using Newtonsoft.Json;
using UnityEngine;

public class PanelSystem : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    [SerializeField]
    protected PanelSystem[] panelSystemList;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public virtual void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public virtual void SetData(string data)
    {
        
    }

    protected ClientData DeserializeData(string data)
    {
        return JsonConvert.DeserializeObject<ClientData>(data);
    }

    protected string SerializeData(ClientData data)
    {
        return JsonConvert.SerializeObject(data);
    }
}
