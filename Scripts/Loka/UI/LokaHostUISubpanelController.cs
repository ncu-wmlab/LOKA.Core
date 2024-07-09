using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LokaHostUISubpanelController : MonoBehaviour
{
    [SerializeField] LabDevicePanel _labDevicePanel;
    [SerializeField] LokaInputActionsPanel _inputActionsPanel;
    [SerializeField] LokaRtcStatsReportPanel _rtcStatsReportPanel;

    LokaPlayer _focusPlayer;
    ILokaHostUISubpanel _currentSubpanel;


    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _labDevicePanel.gameObject.SetActive(false);
        _inputActionsPanel.gameObject.SetActive(false);
        _rtcStatsReportPanel.gameObject.SetActive(false);
    }

    /* -------------------------------------------------------------------------- */

    public void FocusPlayer(LokaPlayer player)
    {
        _focusPlayer = player;
    }

    public void UnfocusPlayer()
    {
        _focusPlayer = null;
    }

    void ShowPanel(ILokaHostUISubpanel subpanel)
    {
        if(_currentSubpanel != null)
        {
            _currentSubpanel.OnHide();
            _currentSubpanel.gameObject.SetActive(false);
        }

        _currentSubpanel = subpanel;
        _currentSubpanel.gameObject.SetActive(true);
        _currentSubpanel.OnShow(_focusPlayer);
    }

    /* -------------------------------------------------------------------------- */
    // Unity UI Button Callbacks

    public void ShowLabDevicePanel()
    {
        ShowPanel(_labDevicePanel);
    }

    public void ShowInputActionsPanel()
    {
        ShowPanel(_inputActionsPanel);
    }

    public void ShowRtcStatsReportPanel()
    {
        ShowPanel(_rtcStatsReportPanel);
    }
}