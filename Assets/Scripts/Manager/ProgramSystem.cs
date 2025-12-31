using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProgramSystem : MonoBehaviour
{
    public LoadUserPanel _loadUserPanel;
    public DisplayUserInfoPanel _displayUserInfoPanel;
    public FirebaseSystem _firebaseSystem;
    public MainPanel _mainPanel;

    private PanelSystem _currentPanel;

    void Start()
    {
        _mainPanel.Init(ChangePanel);
        _loadUserPanel.Init(LoadClientData, ChangePanel);
        _displayUserInfoPanel.Init((x) => _firebaseSystem.SaveClientDataToCloud(x).Forget(),
                                    (x) => _firebaseSystem.UpdateClientDataInCloud(x).Forget(),
                                    ChangePanel);

        //LoadClientData();

        ChangePanel(_mainPanel);
    }

    /// <summary>
    /// Get client data from cloud and show in load user panel.
    /// </summary>
    private async void LoadClientData()
    {
        var clientList = await _firebaseSystem.LoadClientDataFromCloud();
        _loadUserPanel.Show(clientList);
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
