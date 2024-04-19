using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostUI : MonoBehaviour
{
    [Header("Master UI")]
    [SerializeField] Button _toggleUiButton;
    [SerializeField] GameObject _ui;

    [Header("Connected Players Panel")]
    [SerializeField] Button _connectedPlayerButtonTemplate;
    Dictionary<string, Button> _connectedPlayerButtons = new Dictionary<string, Button>();

    [Header("Main Info Panel")]
    [SerializeField] GameObject _mainPanel;
    [SerializeField] LabDevicePanel _labDevicePanel;

    LokaHost _host;

    // Start is called before the first frame update
    void Start()
    {
        // UI
        _connectedPlayerButtonTemplate.gameObject.SetActive(false);
        _toggleUiButton.onClick.AddListener(() => {
            _ui.SetActive(!_ui.activeSelf);
        });

        // Register Host Events
        _host = FindObjectOfType<LokaHost>();
        _host.OnPlayerJoin += (player) => {
            var button = Instantiate(_connectedPlayerButtonTemplate, _connectedPlayerButtonTemplate.transform.parent);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TMP_Text>().text = player.ConnectionId;
            button.onClick.AddListener(() => {
                ShowPlayerInfo(player);
            });
            _connectedPlayerButtons[player.ConnectionId] = button;
        };
        _host.OnPlayerExit += (connectionId) => {
            Destroy(_connectedPlayerButtons[connectionId].gameObject);
            _connectedPlayerButtons.Remove(connectionId);
            ShowPlayerInfo(null);
        };

        UnshowPlayerInfo();
        _ui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowPlayerInfo(LokaPlayer player)
    {
        // unshow
        if(player == null)
        {
            _mainPanel.gameObject.SetActive(false);
            _labDevicePanel.Clear();            
            return;
        }

        // show
        _mainPanel.gameObject.SetActive(true);        
        _labDevicePanel.SetLabDeviceChannel(player.LabDeviceChannel);
    }

    public void UnshowPlayerInfo()
    {
        ShowPlayerInfo(null);
    }
}
