using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppointmentPanel : PanelSystem
{
    [SerializeField]
    private Button _closeAppointmentButton;
    [SerializeField]
    private Button _timeButtonPrefab;
    [SerializeField]
    private RectTransform _timeButtonContainer;
    [SerializeField]
    private Button _bookedClientPrefab;
    [SerializeField]
    private RectTransform _bookedClientContainer;
    [SerializeField]
    private TMP_Dropdown nameListDropdown;
    [SerializeField]
    private RectTransform _calendarRectTransform;

    private CalendarController _calendar;
    private DateTime _selectedDate;
    private int _selectedDayObject;
    private Button _bookedClientButton;

    private List<ClientData> clientList = new List<ClientData>();
    [HideInInspector] private List<AppointmentData> appointmentListOnDate = new List<AppointmentData>();
    private Dictionary<int,Button> timeButtonDic = new Dictionary<int, Button>();
    private Dictionary<int,Button> bookedClientButtonDic = new Dictionary<int,Button>();
    private List<Button> bookedClientButtonPoolList = new List<Button>();
    private AppointmentData appointmentData = new AppointmentData();

    private Action<AppointmentData> bookAppointmentTriggerCallback;
    private Action<AppointmentData> cancelAppointmentTriggerCallback;
    private Func<AppointmentData, UniTask<List<AppointmentData>>> getAppointmentTriggerFunc;
    private Action<PanelSystem, string> sceneTriggerCallback;
    private Func<UniTask<List<ClientData>>> loadTriggerFunc;

    public async UniTask Init(Func<UniTask<List<ClientData>>> loadTrigger, Action<AppointmentData> bookAppointmentTrigger,
                              Action<AppointmentData> cancelAppointmentTrigger, Func<AppointmentData, UniTask<List<AppointmentData>>> getAppointmentTrigger, 
                              CalendarController calendar, Action<PanelSystem, string> sceneTrigger)
    {
        loadTriggerFunc = loadTrigger;
        _calendar = calendar;
        bookAppointmentTriggerCallback = bookAppointmentTrigger;
        cancelAppointmentTriggerCallback = cancelAppointmentTrigger;
        getAppointmentTriggerFunc = getAppointmentTrigger;
        sceneTriggerCallback = sceneTrigger;

        for (int i = 10; i < 22; i++)
        {
            Button newTimeButton = Instantiate<Button>(_timeButtonPrefab, _timeButtonContainer);
            newTimeButton.GetComponentInChildren<TMP_Text>().text = (i).ToString("D2") + ":00";
            timeButtonDic.Add(i,newTimeButton);
        }

        await UniTask.CompletedTask;
    }

    public override void Show()
    {
        base.Show();

        _calendar.ShowCalendar(_calendarRectTransform.position, Vector3.one * 4.3f);
        _calendar.dateTrigger += DisplayDate;
        _calendar._calendarMode = CalendarController.CalendarMode.SelectShow;

        LoadClientList().Forget();
        
        ShowTimeButton(true);

        _closeAppointmentButton.onClick.AddListener(() =>
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
        nameListDropdown.options.Clear();
        _calendar.HideCalendar();
        _calendar.dateTrigger -= DisplayDate;
        _selectedDayObject = -1; 

        ShowTimeButton(false);

        foreach (var data in bookedClientButtonDic)
        {
            data.Value.gameObject.SetActive(false);
            bookedClientButtonPoolList.Add(data.Value);
        }
        bookedClientButtonDic.Clear();

        _closeAppointmentButton.onClick.RemoveAllListeners();
        nameListDropdown.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Load client List into dropdown menu
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadClientList()
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
    
    /// <summary>
    /// Creates and displays a button representing a booked appointment date in the user interface.
    /// </summary>
    /// <remarks>This method either reuses an existing button from the pool or instantiates a new one if none
    /// are available. The button is updated to show the appointment's identification code and date, and is added to the
    /// list of displayed appointment buttons. The container's size is adjusted to accommodate the new button.</remarks>
    private void CreateBookedAppointmentDate()
    {
        if (bookedClientButtonPoolList.Count > 0)
        {
            _bookedClientButton = bookedClientButtonPoolList[0];
            bookedClientButtonPoolList.RemoveAt(0);
            _bookedClientButton.gameObject.SetActive(true);
        }
        else
        {
            _bookedClientButton = Instantiate<Button>(_bookedClientPrefab, _bookedClientContainer);
        }

        AppointmentData tempData = new AppointmentData
        {
            IC = appointmentData.IC,
            Date = appointmentData.Date
        };

        TMP_Text[] texts = _bookedClientButton.GetComponentsInChildren<TMP_Text>();

        texts[0].text = appointmentData.IC;
        texts[1].text = appointmentData.Date.ToDateTime().ToLocalTime().ToString();
        _bookedClientButton.onClick.AddListener(() => CancelBookedAppointmentDate(tempData));
        bookedClientButtonDic.Add(appointmentData.Date.ToDateTime().ToLocalTime().Hour, _bookedClientButton);
        //Debug.Log(appointmentData.Date.ToDateTime().ToLocalTime().Hour);
        _bookedClientContainer.sizeDelta = new Vector2(_bookedClientContainer.sizeDelta.x, bookedClientButtonDic.Count * (200 + 10));
    }

    /// <summary>
    /// cancels a booked appointment and updates the user interface accordingly.
    /// </summary>
    /// <param name="appointment"></param>
    private void CancelBookedAppointmentDate(AppointmentData appointment)
    {
        if (cancelAppointmentTriggerCallback != null)
        {
            cancelAppointmentTriggerCallback(appointment);
        }
        CheckAppointmentList(appointment, true);

        if (appointmentListOnDate.Count < 1)
            _calendar.HighlightSelectDay(_selectedDayObject, false);

        //Debug.Log("Cancelled appointment on: " + date );
        ShowTimeButton(true, appointment.Date.ToDateTime().ToLocalTime().Hour);

        RemoveBookedAppointmentDate(appointment.Date.ToDateTime().ToLocalTime().Hour);
    }

    /// <summary>
    /// Shows or hides appointment time buttons in the user interface.
    /// </summary>
    /// <param name="show"></param>
    /// <param name="time"></param>
    private void ShowTimeButton(bool show, int time = -1)
    {
        if (time < 0)
        {
            foreach (var data in timeButtonDic)
            {
                data.Value.gameObject.SetActive(show);
                data.Value.onClick.RemoveAllListeners();
                if (show)
                {
                    data.Value.onClick.AddListener(() => SummitAppointment(data.Key));
                }
            }
        }
        else
        {
            timeButtonDic[time].gameObject.SetActive(show);
            timeButtonDic[time].onClick.RemoveAllListeners();
            if (show)
            {
                timeButtonDic[time].onClick.AddListener(() => SummitAppointment(time));
            }
        }
    }

    /// <summary>
    /// Removes a booked appointment date from the user interface.
    /// </summary>
    /// <param name="date"></param>
    private void RemoveBookedAppointmentDate(int date = -1)
    {
        if (date == -1)
        {
           foreach (var data in bookedClientButtonDic)
           {
               data.Value.onClick.RemoveAllListeners();
               data.Value.gameObject.SetActive(false);
               bookedClientButtonPoolList.Add(data.Value);
           }
           bookedClientButtonDic.Clear();
        }
        else
        {
            bookedClientButtonDic[date].onClick.RemoveAllListeners();
            bookedClientButtonDic[date].gameObject.SetActive(false);
            bookedClientButtonPoolList.Add(bookedClientButtonDic[date]);
            bookedClientButtonDic.Remove(date);
        }
        _bookedClientContainer.sizeDelta = new Vector2(_bookedClientContainer.sizeDelta.x, bookedClientButtonDic.Count * (200 + 10));
    }


    /// <summary>
    /// Compile appointment data and summit
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    private void SummitAppointment(int time)
    {
        if (nameListDropdown.captionText.text == "Select Client")
            return;

        appointmentData.IC = clientList[nameListDropdown.value].IC;

        DateTime newDateTime = _selectedDate;
        newDateTime = newDateTime.AddHours(time).ToUniversalTime();
        appointmentData.Date = Timestamp.FromDateTime(newDateTime);
        ShowTimeButton(false, time);

        CreateBookedAppointmentDate();

        if (appointmentListOnDate.Count == 0)
        {
            _calendar.HighlightSelectDay(_selectedDayObject, true);
        }
        CheckAppointmentList(appointmentData, false);

        if (bookAppointmentTriggerCallback != null)
        {
            bookAppointmentTriggerCallback(appointmentData);
        }
    }

    /// <summary>
    /// Displays available appointment times for a selected date.
    /// </summary>
    /// <param name="dayObject"></param>
    /// <param name="date"></param>
    private async void DisplayDate(int dayObject, DateTime date)
    {
        if (string.IsNullOrEmpty(nameListDropdown.captionText.text))
        {
            nameListDropdown.captionText.text = "Select Client";
            nameListDropdown.Select();
        }

        AppointmentData newAppointment = new AppointmentData();
        newAppointment.Date = Timestamp.FromDateTime(date.ToLocalTime());
        //Debug.Log("Date" + newAppointment.Date + " " + date);

        RemoveBookedAppointmentDate();
        if (getAppointmentTriggerFunc != null)
        {
            appointmentListOnDate = new List<AppointmentData>();
            appointmentListOnDate = await getAppointmentTriggerFunc(newAppointment);
            ShowTimeButton(true);

            for (int i = 0; i < appointmentListOnDate.Count; i++)
            {
                appointmentData = new AppointmentData();
                appointmentData.IC = appointmentListOnDate[i].IC;
                appointmentData.Date = appointmentListOnDate[i].Date;
                ShowTimeButton(false, appointmentData.Date.ToDateTime().ToLocalTime().Hour);

                CreateBookedAppointmentDate();
            }
        }

        _selectedDate = date;
        _selectedDayObject = dayObject;
    }

    /// <summary>
    /// Check appointmentListOnDate based on bool to add or remove AppointmentData
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="remove"></param>
    private void CheckAppointmentList(AppointmentData appointment, bool remove)
    {
        for (int i = 0; i < appointmentListOnDate.Count; i++)
        {
            if (appointmentListOnDate[i].IC == appointment.IC && appointmentListOnDate[i].Date.ToDateTime() == appointment.Date.ToDateTime())
            {
                if (remove)
                {
                    appointmentListOnDate.RemoveAt(i);
                    return;
                }
            }
        }

        if (remove == false)
        {
            appointmentListOnDate.Add(new AppointmentData { Date = appointment.Date, IC = appointment.IC});
        }
    }

    /// <summary>
    /// Select client based on the dropdown list
    /// </summary>
    /// <param name="index"></param>
    private void SelectClient(int index)
    {
        nameListDropdown.captionText.text = nameListDropdown.options[index].text;
        nameListDropdown.Select();
    }
}
