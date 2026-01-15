using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class ConsentUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _consentText;
    [SerializeField]
    private TMP_Text _guardianText;
    [SerializeField]
    private TMP_InputField _guardianNameInputfield;
    [SerializeField]
    private TMP_Text _acknowledgementText;
    [SerializeField]
    private TMP_Text _agreementText;
    [SerializeField]
    private TMP_Text _activitiesText;
    [SerializeField]
    private TMP_Text _signNameText;
    [SerializeField]
    private TMP_Text _signDateText;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Button _signButton;
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private Texture2DDrawer _texture2DDrawer;
    [SerializeField]
    private RawImage _displaySignature;
    [SerializeField]
    private VerticalLayoutGroup _verticalLayoutGroup;

    private string _names = "";
    private string _personalConsent;
    private string _guardianConsent;
    private string _acknowledgementConsent;
    private string _activitiesConsent;
    private string _agreementConsent;
    private string _signNameConsent;
    private string _signDateConsent;
    private bool _underAge;
    public bool _sign;
    public bool _guardianName;
    private bool _complete;

    private Action<int> completeTriggerCallback;

    private void Start()
    {
        Show("Ddd", 5, null);
    }

    public void Show(string name, int age, Action<int> completeCallback)
    {
        _names = name;
        if (age < 18)
        {
            _underAge = true;
            _verticalLayoutGroup.spacing = 25;

            _guardianNameInputfield.onEndEdit.AddListener(InsertGuardianName);
        }
        else
        {
            _underAge = false;
            _verticalLayoutGroup.spacing = 80;
        }

        completeTriggerCallback = completeCallback;

        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        UpdateConsent();
        UpdateAcknowledgement();

        _closeButton.onClick.AddListener(TriggerCloseConsentUI);
        _signButton.onClick.AddListener(TriggerSigning);
    }

    private void Hide()
    {
        _names = "";
        _underAge = false;
        _guardianName = false;
        _sign = false;
        completeTriggerCallback = null;

        _consentText.text = "";
        _guardianText.text = "";
        _acknowledgementText.text = "";
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _closeButton.onClick.RemoveAllListeners();
    }

    private void TriggerCloseConsentUI()
    {
        _complete = (((_underAge == true && _guardianName == true) || (_underAge == false)) && _sign == true);

        if (completeTriggerCallback != null)
        {
            completeTriggerCallback(_complete ? 1 : 0);
        }
        Hide();
    }

    private void UpdateConsent()
    {
        _personalConsent = $"I, {_names}, hereby give permission to the physiotherapist on duty to advice and treat accordingly.\n" +
                           $"I understand that this session involves physical assessment and treatment and I consent under the care of HAVENXAU.";

        _guardianConsent = "                                            guardian to Consent hereby give permission to the physiotherapist on duty" +
                            "to advice and treat accordingly.\r\nI understand that this session involves physical assessment and treatment and I consent under the care of HAVENXAU.";

        if (_underAge)
        {
            _consentText.text = _personalConsent;
            _guardianText.text = _guardianConsent;

            _guardianText.gameObject.SetActive(true);
        }
        else
        {
            _consentText.text = _personalConsent;
            _guardianText.text = "";
            _guardianText.gameObject.SetActive(false);
        }
    }

    private void TriggerSigning()
    {
        _texture2DDrawer.EnableSign((x) =>
        {
            _displaySignature.texture = x;
            _sign = true;
        });
    }

    private void InsertGuardianName(string name)
    {
        _guardianNameInputfield.text = "I, " + name;
        _guardianName = true;
    }

    private void UpdateAcknowledgement()
    {
        _acknowledgementConsent = "The Personal Data obtained from the client is processed or disclosed to third parties as required or permitted by law. " +
                                  "Pursuant to section 7 of the Personal Data Protection Act (PDPA) 2010, a copy of the written notice which includes the " +
                                  "purpose for which the client's personal data and sensitive personal data is collected/processed and classes of third parties" +
                                  "to whom HAVENXAU will/may disclose the client personal data upon request.";

        _activitiesConsent = "I / We, hereby give consent to HAVENXAU to contact me/ us for future events, promotions, updates, and any other marketing activities";
        _agreementConsent = "I/We, undersigned, hereby certify that all the above information provided is true and correct in every aspect.";
        _signNameConsent = $"Client Name : {_names} \nClient Signature : ";
        _signDateConsent = "Date : " + DateTime.Now.ToString("g");

        _acknowledgementText.text = _acknowledgementConsent;
        _activitiesText.text = _activitiesConsent;
        _agreementText.text = _agreementConsent;
        _signNameText.text = _signNameConsent;
        _signDateText.text = _signDateConsent;
    }
}
