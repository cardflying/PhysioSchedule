using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionPanel : PanelSystem
{
    [SerializeField]
    private SessionUI _sessionUI;
    [SerializeField]
    private RectTransform _sessionContainer;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _purchaseButton;

    private ClientData _clientData;
    private SessionUI _newSessionUI;

    private List<SessionUI> sessionList = new List<SessionUI>();
    private List<SessionUI> sessionPool = new List<SessionUI>();
    private List<SessionNote> sessionNotes = new List<SessionNote>();
    private Action<PanelSystem, string> sceneTriggerCallback;

    public async UniTask Init(Action<PanelSystem, string> sceneTrigger)
    {
        sceneTriggerCallback = sceneTrigger;

        await UniTask.CompletedTask;
    }

    public override void SetData(string data)
    {
        _clientData = DeserializeData<ClientData>(data);
        sessionNotes = ConvertDataToSessionNote(_clientData.SessionNote);
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
            sessionList[i].Hide();
        }
        sessionPool.AddRange(sessionList);
        sessionList.Clear();

        if (sessionNotes != null)
            sessionNotes.Clear();
    }

    private void DisplaySession()
    {
        for (int i = 0; i < _clientData.Session; i++)
        {
            if (sessionPool.Count > 0)
            {
                _newSessionUI = sessionPool[0];
                _newSessionUI.gameObject.SetActive(true);
                sessionPool.RemoveAt(0);
            }
            else
            {
                _newSessionUI = Instantiate(_sessionUI, _sessionContainer);
            }
            sessionList.Add(_newSessionUI);

            int sessionIndex = i;
            if (sessionNotes.Count > i)
            {
                _newSessionUI.Init(sessionNotes[i].sessionDate, sessionNotes[i].GetNoteOnly(), () => OpenNote(sessionIndex));
            }
            else
            {
                _newSessionUI.Init("", "", () => OpenNote(sessionIndex));
            }
        }
    }

    public void OpenNote(int index)
    {
        if (sceneTriggerCallback != null)
        {
            sceneTriggerCallback(panelSystemList[2], index + "|" + SerializeData(_clientData));
        }
    }

    public static List<SessionNote> ConvertDataToSessionNote(string data)
    {
        if (string.IsNullOrEmpty(data))
            return new List<SessionNote>();

        List<SessionNote> sessionNotes = new List<SessionNote>();

        string[] sessionDataList = data.Split('!');

        foreach (string rawdata in sessionDataList)
        {
            string[] sessionData = rawdata.Split('#');

            SessionNote note = new SessionNote();
            note.sessionDate = sessionData[0];

            for (int i = 1; i < sessionData.Length; i += 2)
            {
                Note newNote = new Note();
                newNote.noteText = sessionData[i];
                newNote.strokeData = JsonConvert.DeserializeObject<StrokeData>(sessionData[i + 1]);
                note.notes.Add(newNote);
            }

            sessionNotes.Add(note);
        }

        return sessionNotes;
    }

    public static string ConvertSessionNoteToString(List<SessionNote> data)
    {
        if (data == null || data.Count == 0)
            return null;

        string output = "";

        for (int i = 0; i < data.Count; i++)
        {
            if (i != 0)
            {
                output += "!";
            }

            output += data[i].sessionDate;

            for (int j = 0; j < data[i].notes.Count; j++)
            {
                output += "#" + data[i].notes[j].noteText + "#" + JsonConvert.SerializeObject(data[i].notes[j].strokeData);
            }
        }

        return output;
    }

    [Serializable]
    public class SessionNote
    {
        public string sessionDate;
        public List<Note> notes = new List<Note>();

        string serializeData;

        public string ToSerializeString()
        {
            serializeData = string.Empty;

            for (int i = 0; i < notes.Count; i++)
            {
                serializeData += "-" + notes[i] + "-" + JsonConvert.SerializeObject(notes[i].strokeData);
            }

            serializeData = sessionDate + serializeData;

            return serializeData;
        }

        public string GetNoteOnly()
        {
            serializeData = string.Empty;

            for (int i = 0; i < notes.Count; i++)
            {
                serializeData += "- " + notes[i].noteText + "\n";
            }

            return serializeData;
        }

        /// <summary>
        /// Get index based on id in notes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetSessionNoteIndex(int id)
        {
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].id == id)
                    return i;
            }
            return -1;
        }
    }
}
