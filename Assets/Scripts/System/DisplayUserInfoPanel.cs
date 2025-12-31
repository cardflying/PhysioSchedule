using Newtonsoft.Json;
using System;
using TMPro;
using UI.Dates;
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
    private DatePicker _dobDatePicker;
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

    private ClientData _clientData;
    
    private Action<ClientData> summitTriggerAction;
    private Action<ClientData> editTriggerAction;
    private Action<PanelSystem,string> sceneTriggerAction;

    public void Init(Action<ClientData> summitTrigger, Action<ClientData> editTrigger, Action<PanelSystem,string> sceneTrigger)
    {
        _clientData = new ClientData();

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
        if (!string.IsNullOrEmpty(_clientData.DOB))
        {
            DateTime dobDateTime;
            if (DateTime.TryParse(_clientData.DOB, out dobDateTime))
            {
                _dobDatePicker.SelectedDate = new SerializableDate { Date = dobDateTime };
            }
        }
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

    public void ReceiveUsername(string name)
    {
        _clientData.Name = name;
    }
    public void ReceiveIC(string ic)
    {
        _clientData.IC = ic;
    }
    public void ReceiveDOB(DateTime date)
    {
        _clientData.DOB = date.ToDateString();
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
}
