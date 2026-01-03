using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProgramSystem : MonoBehaviour
{
    public LoadUserPanel _loadUserPanel;
    public DisplayUserInfoPanel _displayUserInfoPanel;
    public AppointmentPanel _appointmentPanel;
    public FirebaseSystem _firebaseSystem;
    public CalendarController _calendarController;
    public MainPanel _mainPanel;
    public PricePanel _pricePanel;
    public SessionPanel _sessionPanel;

    private PanelSystem _currentPanel;

    async void Start()
    {
        await _firebaseSystem.Init();
        await _calendarController.init(_firebaseSystem.GetAppointmentList);

        _mainPanel.Init(ChangePanel);
        await _loadUserPanel.Init(_firebaseSystem.LoadClientDataFromCloud, ChangePanel);
        _displayUserInfoPanel.Init((x) => _firebaseSystem.SaveClientDataToCloud(x).Forget(),
                                    (x) => _firebaseSystem.UpdateClientDataInCloud(x).Forget(),
                                    _calendarController,
                                    ChangePanel);
        await _appointmentPanel.Init(_firebaseSystem.LoadClientDataFromCloud,
                                    (x) => _firebaseSystem.BookAppointment(x).Forget(),
                                    (x) => _firebaseSystem.CancelAppointment(x).Forget(),
                                    _firebaseSystem.GetAppointmentList,
                                    _calendarController,
                                    ChangePanel);
        await _pricePanel.Init((x) => _firebaseSystem.UpdateClientDataInCloud(x).Forget(),
                               ChangePanel);

        await _sessionPanel.Init(ChangePanel);

        await UniTask.Delay(1000);
        //LoadClientData();
        ChangePanel(_mainPanel);
    }

    /// <summary>
    /// Change the currently displayed panel to a new panel.
    /// </summary>
    /// <param name="newPanel"></param>
    private void ChangePanel(PanelSystem newPanel, string data = null)
    {
        Debug.Log("Changing panel to: " + newPanel.name);

        if (_currentPanel != null)
        {
            _currentPanel.Hide();
        }

        if (data != null)
        {
            newPanel.SetData(data);
        }
        
        newPanel.Show();
        _currentPanel = newPanel;
    }
}
