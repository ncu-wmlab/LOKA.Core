using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LabDevicePanel : BaseDictPanel, ILokaHostUISubpanel
{
    LabDeviceChannel _focusingLabDeviceChannel;

    public void OnShow(LokaPlayer player)
    {
        _focusingLabDeviceChannel = player.LabDeviceChannel;
    }

    public void OnHide()
    {
        _focusingLabDeviceChannel = null;
    } 

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    new void Awake()
    {
        // SetLabDeviceChannel(FindObjectOfType<LabDeviceChannel>());
        base.Awake();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected override void Update()
    {
        if(!_focusingLabDeviceChannel)
            return;

        _SetMetric("", "IsConnected", _focusingLabDeviceChannel.IsConnected);

        // Ganglion
        try
        {
            _SetMetric($"Ganglion", "IsAvailable", _focusingLabDeviceChannel.GetGanglionIsAvailable());
            bool ganglionIsConnected = _focusingLabDeviceChannel.GetGanglionIsConnected();
            _SetMetric($"Ganglion", "IsConnected", ganglionIsConnected);
            if (ganglionIsConnected)
            {
                _SetMetric($"Ganglion", "EEGData", _focusingLabDeviceChannel.GetGanglionEEGData());
                _SetMetric($"Ganglion", "ImpedanceData", _focusingLabDeviceChannel.GetGanglionImpedanceData());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[LabDevicePanel] Error fetching Ganglion Data" + e);
        }

        // EyeTrack
        try
        {
            _SetMetric($"EyeTrack", "IsAvailable", _focusingLabDeviceChannel.GetData<bool>(LabDeviceChannel.LabDeviceSignal.EYETRACK_ISAVAILABLE));
            var eyeLeftRightData = _focusingLabDeviceChannel.GetEyeTrackEyeLeftRightData();
            _SetMetric($"EyeTrack.EyeLeftRightData", "Timestamp", eyeLeftRightData?.Timestamp);
            _SetMetric($"EyeTrack.EyeLeftRightData", "LeftEyeOpenness", eyeLeftRightData?.LeftEyeOpenness);
            _SetMetric($"EyeTrack.EyeLeftRightData", "RightEyeOpenness", eyeLeftRightData?.RightEyeOpenness);
            _SetMetric($"EyeTrack.EyeLeftRightData", "LeftEyePositionGuide", eyeLeftRightData?.LeftEyePositionGuide);
            _SetMetric($"EyeTrack.EyeLeftRightData", "RightEyePositionGuide", eyeLeftRightData?.RightEyePositionGuide);
            var eyeCombinedData = _focusingLabDeviceChannel.GetEyeTrackEyeCombinedData();
            _SetMetric($"EyeTrack.CombinedData", "Timestamp", eyeCombinedData.Timestamp);
            _SetMetric($"EyeTrack.CombinedData", "CombineEyeGazeVector", eyeCombinedData?.CombineEyeGazeVector);
            _SetMetric($"EyeTrack.CombinedData", "CombineEyeGazePoint", eyeCombinedData?.CombineEyeGazePoint);
            var eyeFocusData = _focusingLabDeviceChannel.GetEyeTrackEyeFocusData();
            _SetMetric($"EyeTrack.EyeFocusData", "FocusDistance", eyeFocusData?.FocusDistance);
            _SetMetric($"EyeTrack.EyeFocusData", "Timestamp", eyeFocusData?.Timestamp);
            _SetMetric($"EyeTrack.EyeFocusData", "FocusName", eyeFocusData?.FocusName);
            _SetMetric($"EyeTrack.EyeFocusData", "FocusNormal", eyeFocusData?.FocusNormal);
            _SetMetric($"EyeTrack.EyeFocusData", "FocusPoint", eyeFocusData?.FocusPoint);
        }
        catch (Exception e)
        {
            // Debug.LogWarning("[LabDevicePanel] Error fetching EyeTrack Data: " + e);
        }

        // Breath
        try
        {
            _SetMetric($"BreathStrap", "IsAvailable", _focusingLabDeviceChannel.GetBreathStrapIsAvailable());
            _SetMetric($"BreathStrap", "IsConnected", _focusingLabDeviceChannel.GetBreathStrapIsConnected());
            _SetMetric($"BreathStrap", "BreathValue", _focusingLabDeviceChannel.GetBreathStrapData()?.breathValue);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LabDevicePanel] Error fetching Breath Data: " + e);
        }

        // write data
        // try
        // {
        //     var json = JsonConvert.SerializeObject(_dict, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        //     print(json);
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError("[LabDevicePanel] Error serializing data: " + e);
        // }

        base.Update();
    }

    /* -------------------------------------------------------------------------- */

    public void RequestGanglionConnect(bool start)
    {
        _focusingLabDeviceChannel.SendRequest(LabDeviceChannel.LabDeviceCommand.GANGLION_DO_CONNECT, start);
    }

    public void RequestGanglionEEG(bool start)
    {
        _focusingLabDeviceChannel.SendRequest(LabDeviceChannel.LabDeviceCommand.GANGLION_RECEIVE_EEG, start);
    }

    public void RequestGanglionImpedance(bool start)
    {
        _focusingLabDeviceChannel.SendRequest(LabDeviceChannel.LabDeviceCommand.GANGLION_RECEIVE_IMPEDANCE, start);
    }

    public void RequestBreathStrapConnect(bool start)
    {
        _focusingLabDeviceChannel.SendRequest(LabDeviceChannel.LabDeviceCommand.BREATHSTRAP_DO_CONNECT, start);
    }
}
