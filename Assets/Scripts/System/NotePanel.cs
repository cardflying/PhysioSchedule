using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SessionPanel;
using ColorUtility = UnityEngine.ColorUtility;

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

    private ClientData _clientData;
    private NoteUI newNoteUI;
    private int sessionIndex;
    private int noteIndex = -1;
    private SessionNote newSessionNote;
    private List<NoteUI> noteUIList = new List<NoteUI>();
    private List<NoteUI> noteUIPool = new List<NoteUI>();
    private List<SessionNote> sessionNotes = new List<SessionNote>();

    private Action<PanelSystem, string> sceneTriggerCallback;

    public async UniTask Init(Action<PanelSystem, string> sceneTrigger)
    {
        sceneTriggerCallback = sceneTrigger;

        await UniTask.CompletedTask;
    }

    public override void SetData(string data)
    {
        string[] rawData = data.Split('|');
        _clientData = DeserializeData<ClientData>(rawData[1]);

        if (int.TryParse(rawData[0], out sessionIndex))
            GetSessionNote();
    }

    public override void Show()
    {
        base.Show();

        _closeButton.onClick.AddListener(() =>
        {
            if (sceneTriggerCallback != null)
            {
                sceneTriggerCallback(panelSystemList[0], SerializeData(_clientData));
            }
        });
        _addNoteUIButton.onClick.AddListener(() => CreateNoteUI(Color.white));

        _drawing.Enable();
        _drawing.drawStrokeTrigger += (w,x,y,z) => CreateNoteUI(w,x,y,z); 
    }

    public override void Hide()
    {
        base.Hide();
        _closeButton.onClick.RemoveAllListeners();
        _addNoteUIButton.onClick.RemoveAllListeners();

        _drawing.Disable();
        _drawing.drawStrokeTrigger = null;

        for (int i = 0; i< noteUIList.Count; i++)
        {
            noteUIList[i].ResetData();
            noteUIPool.Add(noteUIList[i]);
        }
        noteUIList.Clear();
        sessionNotes.Clear();

        sessionIndex = -1;
        noteIndex = -1;
    }

    private int CreateNoteUI(Color color, string date = null, string description = null, StrokeData strokeData = null)
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
        noteIndex++;
        newNoteUI.Init(noteIndex, color, date, description, (x,y,z) => SetSessionNote(x,y,z,strokeData));
        newNoteUI.transform.SetAsFirstSibling();
        newNoteUI.deleteCallback += RemoveNoteUI;

        noteUIList.Add(newNoteUI);

        _noteUIContainer.anchoredPosition = Vector2.zero;

        return noteIndex;
    }

    /// <summary>
    /// Remove NoteUI and stroke
    /// </summary>
    /// <param name="target"></param>
    private void RemoveNoteUI(GameObject target)
    {
        NoteUI noteUI = target.GetComponent<NoteUI>();
        _drawing.RemoveStroke(noteUI.GetBgColor());

        for (int i = 0; i < sessionNotes[sessionIndex].notes.Count; i++)
        {
            if (sessionNotes[sessionIndex].notes[i].id == noteUI.GetId())
            {
                sessionNotes[sessionIndex].notes.RemoveAt(i);
                _clientData.SessionNote = ConvertSessionNoteToString(sessionNotes);
            }
        }

        noteUI.ResetData();
        noteUIPool.Add(noteUI);
        noteUIList.Remove(noteUI);
    }

    private void GetSessionNote()
    {
        sessionNotes = ConvertDataToSessionNote(_clientData.SessionNote);

        if (sessionIndex < sessionNotes.Count)
        {
            for (int i = 0; i < sessionNotes[sessionIndex].notes.Count; i++)
            {
                Note newNote = sessionNotes[sessionIndex].notes[i];
                
                Color newColor = Color.white;

                //Draw stroke
                if (newNote.strokeData != null)
                {
                    ColorUtility.TryParseHtmlString("#"+newNote.strokeData.color, out newColor);
                    _drawing.CreateLine(newColor, newNote.strokeData);
                }

                //Create NoteUI
                newNote.id = CreateNoteUI(newColor, sessionNotes[sessionIndex].sessionDate, newNote.noteText);
            }
        }
    }

    /// <summary>
    /// Save date, data, and stroke into _clientData
    /// </summary>
    /// <param name="date"></param>
    /// <param name="data"></param>
    /// <param name="strokeData"></param>
    private void SetSessionNote(int id, string date, string data, StrokeData strokeData)
    {
        Note newNote = new Note();
        newNote.noteText = data;
        newNote.strokeData = strokeData;

        if (sessionNotes.Count == 0)
        {
            newNote.id = id;

            newSessionNote = new SessionNote();
            newSessionNote.sessionDate = date;
            newSessionNote.notes.Add(newNote);
            sessionNotes.Add(newSessionNote);

        }
        else
        {
            newSessionNote = sessionNotes[sessionIndex];

            int noteIndex = newSessionNote.GetSessionNoteIndex(id);

            if (noteIndex != -1)
            {
                newSessionNote.notes[noteIndex].noteText = data;
            }
            else
            {
                newNote.id = id;
                newSessionNote.notes.Add(newNote);
            }
        }
        _clientData.SessionNote = ConvertSessionNoteToString(sessionNotes);
    }
}
