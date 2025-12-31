using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUserInfoPanel : PanelSystem
{
    private enum Mode
    {
        register = 0,
        load
    }
    private Mode _currentMode;

    [SerializeField]
    private Button _summitButton;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _editButton;

    [SerializeField]
    private TMP_InputField _nameInputField;
    [SerializeField]
    private TMP_InputField _icInputField;
    [SerializeField]
    private Button _dobCalendarButton;
    [SerializeField]
    private TMP_InputField _ageInputField;
    [SerializeField]
    private TMP_Dropdown _genderDropdown;
    [SerializeField]
    private TMP_InputField _nationalityInputField;
    [SerializeField]
    private TMP_InputField _maritalStatusInputField;
    [SerializeField]
    private TMP_InputField _hpInputField;
    [SerializeField]
    private TMP_InputField _emailInputField;
    [SerializeField]
    private TMP_InputField _occupationInputField;
    [SerializeField]
    private TMP_InputField _addressInputField;
    [SerializeField]
    private TMP_Dropdown _languageDropdown;
    [SerializeField]
    private TMP_InputField _emergencyNameInputField;
    [SerializeField]
    private TMP_InputField _emergencyRelationshipInputField;
    [SerializeField]
    private TMP_InputField _emergencyContactInputField;

    private ClientData _clientData;
    private CalendarController _calendar;

    private Action<ClientData> summitTriggerAction;
    private Action<ClientData> editTriggerAction;
    private Action<PanelSystem,string> sceneTriggerAction;

    public void Init(Action<ClientData> summitTrigger, Action<ClientData> editTrigger,
                    CalendarController calendar, Action<PanelSystem,string> sceneTrigger)
    {
        _clientData = new ClientData();
        _calendar = calendar;
        summitTriggerAction = summitTrigger;
        editTriggerAction = editTrigger;
        sceneTriggerAction = sceneTrigger;
    }

    public override void SetData(string data)
    {
        string[] split = data.Split("_");

        _currentMode = (Mode)Enum.Parse(typeof(Mode), split[0]);

        switch (_currentMode)
        {
            case Mode.register:
                _summitButton.onClick.AddListener(RegisterClient);
                _closeButton.onClick.AddListener(() =>
                {
                    if (sceneTriggerAction != null)
                    {
                        sceneTriggerAction(panelSystemList[0], null);
                    }
                });
                break;
            case Mode.load:
                DeserializeData(split[1]);

                _editButton.onClick.AddListener(EditClient);
                _closeButton.onClick.AddListener(() =>
                {
                    if (sceneTriggerAction != null)
                    {
                        sceneTriggerAction(panelSystemList[1], null);
                    }
                });
                break;
            default:
                break;
        }
    }

    public override void Show()
    {
        base.Show();

        _nameInputField.onEndEdit.AddListener(ReceiveUsername);
        _icInputField.onEndEdit.AddListener(ReceiveIC);
        _ageInputField.onEndEdit.AddListener(ReceiveAge);
        _genderDropdown.onValueChanged.AddListener(ReceiveGender);
        _nationalityInputField.onEndEdit.AddListener(ReceiveNationality);
        _maritalStatusInputField.onEndEdit.AddListener(ReceiveMaritalStatus);
        _hpInputField.onEndEdit.AddListener(ReceiveHP);
        _emailInputField.onEndEdit.AddListener(ReceiveEmail);
        _occupationInputField.onEndEdit.AddListener(ReceiveOccupation);
        _addressInputField.onEndEdit.AddListener(ReceiveAddress);
        _languageDropdown.onValueChanged.AddListener(ReceiveLanguage);
        _emergencyNameInputField.onEndEdit.AddListener(ReceiveEmergencyName);
        _emergencyRelationshipInputField.onEndEdit.AddListener(ReceiveEmergencyRelationship);
        _emergencyContactInputField.onEndEdit.AddListener(ReceiveEmergencyContact);
        _dobCalendarButton.onClick.AddListener(ShowCalendar);

        _calendar._calendarMode = CalendarController.CalendarMode.SelectHide;
        _calendar.dateTrigger += DisplayDate;
    }

    public override void Hide()
    {
        base.Hide();

        _nameInputField.onEndEdit.RemoveAllListeners();
        _icInputField.onEndEdit.RemoveAllListeners();
        _ageInputField.onEndEdit.RemoveAllListeners();
        _genderDropdown.onValueChanged.RemoveAllListeners();
        _nationalityInputField.onEndEdit.RemoveAllListeners();
        _maritalStatusInputField.onEndEdit.RemoveAllListeners();
        _hpInputField.onEndEdit.RemoveAllListeners();
        _emailInputField.onEndEdit.RemoveAllListeners();
        _occupationInputField.onEndEdit.RemoveAllListeners();
        _addressInputField.onEndEdit.RemoveAllListeners();
        _languageDropdown.onValueChanged.RemoveAllListeners();
        _dobCalendarButton.onClick.RemoveAllListeners();

        _calendar.dateTrigger -= DisplayDate;
        _calendar.HideCalendar();

        _summitButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
        _editButton.onClick.RemoveAllListeners();
    }

    private void DeserializeData(string data)
    {
        _clientData = JsonConvert.DeserializeObject<ClientData>(data);
        Debug.Log(_clientData.Name);
        _nameInputField.text = _clientData.Name;
        _icInputField.text = _clientData.IC;
        _dobCalendarButton.GetComponentInChildren<TMP_Text>().text = _clientData.DOB;
        _ageInputField.text = _clientData.Age.ToString();
        _genderDropdown.value = _clientData.Gender;
        _nationalityInputField.text = _clientData.Country;
        _maritalStatusInputField.text = _clientData.Status;
        _hpInputField.text = _clientData.Phone.ToString();
        _emailInputField.text = _clientData.Email;
        _occupationInputField.text = _clientData.Job;
        _addressInputField.text = _clientData.Address;
        _languageDropdown.value = _clientData.Language;
    }

    /// <summary>
    /// create new client information
    /// </summary>
    private void RegisterClient()
    {
        if (summitTriggerAction != null)
        {
            summitTriggerAction(_clientData);
        }

        if (sceneTriggerAction != null)
        {
            sceneTriggerAction(panelSystemList[0], null);
        }
    }

    /// <summary>
    /// Edit existing client information
    /// </summary>
    private void EditClient()
    {
        if (summitTriggerAction != null)
        {
            editTriggerAction(_clientData);
        }

        if (sceneTriggerAction != null)
        {
            sceneTriggerAction(panelSystemList[1], null);
        }
    }

    /// <summary>
    /// Show the calendar for date of birth selection
    /// </summary>
    private void ShowCalendar()
    {
        RectTransform calendarButtonRectTransform = _dobCalendarButton.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        calendarButtonRectTransform.GetWorldCorners(corners);
        _calendar.ShowCalendar(corners[0], Vector3.one * 2.7f);
    }

    /// <summary>
    /// Show the selected date on the date of birth button
    /// </summary>
    /// <param name="date"></param>
    private void DisplayDate(DateTime date)
    {
        string dateString = date.ToString("dd MMMM yyyy");

        _dobCalendarButton.GetComponentInChildren<TMP_Text>().text = dateString;
        ReceiveDOB(dateString);
    }

    public void ReceiveUsername(string name)
    {
        _clientData.Name = name;
    }
    public void ReceiveIC(string ic)
    {
        _clientData.IC = ic;
    }
    public void ReceiveDOB(string date)
    {
        _clientData.DOB = date;
    }
    
    public void ReceiveAge(string age)
    {
        _clientData.Age = int.Parse(age);
    }
    public void ReceiveGender(Int32 gender)
    {
        _clientData.Gender = gender;
    }
    public void ReceiveNationality(string Nationality)
    {
        _clientData.Country = Nationality;
    }
    public void ReceiveMaritalStatus(string MaritalStatus)
    {
        _clientData.Status = MaritalStatus;
    }
    public void ReceiveHP(string hp)
    {
        _clientData.Phone = int.Parse(hp);
    }
    public void ReceiveEmail(string email)
    {
        _clientData.Email = email;
    }
    public void ReceiveOccupation(string occupation)
    {
        _clientData.Job = occupation;
    }
    public void ReceiveAddress(string address)
    {
        _clientData.Address = address;
    }
    public void ReceiveLanguage(Int32 language)
    {
        _clientData.Language = language;
    }
    public void ReceiveEmergencyName(string name)
    {
        _clientData.EmergencyContactName = name;
    }
    public void ReceiveEmergencyRelationship(string relationship)
    {
        _clientData.EmergencyContactRelationship = relationship;
    }
    public void ReceiveEmergencyContact(string contact)
    {
        _clientData.EmergencyContactNumber = int.Parse(contact);
    }
}
