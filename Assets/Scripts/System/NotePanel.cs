using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotePanel : PanelSystem
{
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _addNoteUIButton;
    [SerializeField]
    private NoteUI _noteUIPrefab;
    [SerializeField]
    private RectTransform _noteUIContainer;
    [SerializeField]
    private Drawing _drawing;

    private NoteUI newNoteUI;
    private List<NoteUI> noteUIList = new List<NoteUI>();
    private List<NoteUI> noteUIPool = new List<NoteUI>();

    private Action<PanelSystem, string> sceneTriggerCallback;

    public async UniTask Init(Action<PanelSystem, string> sceneTrigger)
    {
        sceneTriggerCallback = sceneTrigger;

        await UniTask.CompletedTask;
    }

    public override void SetData(string data)
    {
    }

    public override void Show()
    {
        base.Show();

        _closeButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[0], null);
            }
        });
        _addNoteUIButton.onClick.AddListener(() => CreateNoteUI(Color.white));

        _drawing.Enable();
        _drawing.drawStrokeTrigger += CreateNoteUI; 
    }

    public override void Hide()
    {
        base.Hide();
        _closeButton.onClick.RemoveAllListeners();
        _addNoteUIButton.onClick.RemoveAllListeners();

        _drawing.Disable();
        _drawing.drawStrokeTrigger -= CreateNoteUI;
    }

    private void CreateNoteUI(Color color)
    {
        if (noteUIPool.Count > 0)
        {
            newNoteUI = noteUIPool[0];
            noteUIPool.RemoveAt(0);
        }
        else
        {
            newNoteUI = Instantiate(_noteUIPrefab, _noteUIContainer);
        }
        newNoteUI.Init(color);
        newNoteUI.transform.SetAsFirstSibling();
        newNoteUI.deleteCallback += RemoveNoteUI;

        noteUIList.Add(newNoteUI);

        RectTransform ySize = noteUIList[0].GetComponent<RectTransform>();
        VerticalLayoutGroup gap = _noteUIContainer.GetComponent<VerticalLayoutGroup>();
        ScaleRectTransform(_noteUIContainer, (ySize.sizeDelta.y + gap.spacing), Vector3.up);
    }

    private void RemoveNoteUI(GameObject target)
    {
        NoteUI noteUI = target.GetComponent<NoteUI>();
        _drawing.RemoveStroke(noteUI.GetBgColor());

        RectTransform ySize = noteUIList[0].GetComponent<RectTransform>();
        VerticalLayoutGroup gap = _noteUIContainer.GetComponent<VerticalLayoutGroup>();

        for (int i = 0; i < noteUIList.Count; i++)
        {
            if (noteUIList[i].gameObject == target)
            {
                noteUIList[i].ResetData();
                noteUIPool.Add(noteUIList[i]);
                noteUIList.RemoveAt(i);
            }
        }
        ScaleRectTransform(_noteUIContainer, (ySize.sizeDelta.y + gap.spacing), Vector3.up);
    }

    private void ScaleRectTransform(RectTransform target, float amount, Vector3 axis)
    {
        if (axis == Vector3.up || axis == Vector3.down)
            target.sizeDelta = new Vector2(target.sizeDelta.x, amount * noteUIList.Count + 140);
        if (axis == Vector3.left || axis == Vector3.right)
            target.sizeDelta = new Vector2(amount * noteUIList.Count, target.sizeDelta.y);
    }
}
