using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LokaHostUI : MonoBehaviour
{
    [Header("Master UI")]
    [SerializeField] Button _toggleUiButton;
    [SerializeField] GameObject _ui;

    [Header("Camera Control (If any)")]
    [SerializeField] Camera _camera;

    [Header("Connected Players Panel")]
    [SerializeField] Button _connectedPlayerButtonTemplate;
    Dictionary<string, Button> _connectedPlayerButtons = new Dictionary<string, Button>();

    [Header("Main Info Panel")]
    [SerializeField] GameObject _mainPanel;

    [Header("Subpanel")]
    [SerializeField] LokaHostUISubpanelController _subpanelController;

    /* -------------------------------------------------------------------------- */
    LokaHost _host;
    LokaPlayer _currentFocusPlayer;
    Pose _initCameraPose;

    /* -------------------------------------------------------------------------- */

    // Start is called before the first frame update
    void Start()
    {
        // UI
        _connectedPlayerButtonTemplate.gameObject.SetActive(false);
        _toggleUiButton.onClick.AddListener(() => {
            _ui.SetActive(!_ui.activeSelf);
        });

        // Record Initial Camera Pose
        if(!_camera)
        {
            Debug.LogWarning("[LokaUI] No Camera Assigned. Will use main camera.");
            _camera = Camera.main;
        }
        _initCameraPose = new Pose(_camera.transform.position, _camera.transform.rotation);

        // Register Host Events
        _host = FindObjectOfType<LokaHost>();
        _host.OnPlayerJoin += (player) => {
            var button = Instantiate(_connectedPlayerButtonTemplate, _connectedPlayerButtonTemplate.transform.parent);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TMP_Text>().text = player.ConnectionId;
            button.onClick.AddListener(() => {
                FocusPlayer(player);
            });
            _connectedPlayerButtons[player.ConnectionId] = button;
        };
        _host.OnPlayerExit += (player) => {
            Destroy(_connectedPlayerButtons[player.ConnectionId].gameObject);
            _connectedPlayerButtons.Remove(player.ConnectionId);

            // current focus player is leaving
            if(_currentFocusPlayer == player)
            {
                UnfocusPlayer();
            }
        };

        UnfocusPlayer();
        _ui.SetActive(false);
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 當使用者關注一個 user 時，顯示其資訊
    /// </summary>
    /// <param name="player">player to focus. set null to unFocus</param>
    public void FocusPlayer(LokaPlayer player)
    {
        // 
        _currentFocusPlayer = player;
        
        if(player == null)
        {
            // hide
            _mainPanel.gameObject.SetActive(false);
            _subpanelController.UnfocusPlayer();
        }
        else
        {
            _mainPanel.gameObject.SetActive(true);
            _subpanelController.FocusPlayer(player);
        }
    }

    public void UnfocusPlayer()
    {
        FocusPlayer(null);
    }

    public void TogglePlayerFollow()
    {  
        if(_camera.transform.parent == null)
        {
            // Follow Player
            var followCam = _currentFocusPlayer?.GetComponentInChildren<Camera>();
            if(!followCam)
            {
                Debug.Log("No Player Focused!");
                return;
            }
            _camera.transform.SetParent(followCam.transform);
            _camera.transform.localPosition = Vector3.zero;
            _camera.transform.localRotation = Quaternion.identity;
        }
        else
        {
            _camera.transform.SetParent(null);
            _camera.transform.SetPositionAndRotation(_initCameraPose.position, _initCameraPose.rotation);
        }
    }
}
