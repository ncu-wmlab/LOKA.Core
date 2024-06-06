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
            _SetMetric($"Ganglion", "IsAvailable", _focusingLabDeviceChannel.GetData<bool>(LabDeviceChannel.LabDeviceControl.GANGLION_ISAVAILABLE));
            bool ganglionIsConnected = _focusingLabDeviceChannel.GetData<bool>(LabDeviceChannel.LabDeviceControl.GANGLION_ISCONNECTED);
            _SetMetric($"Ganglion", "IsConnected", ganglionIsConnected);
            if (ganglionIsConnected)
            {
                Debug.LogError(_focusingLabDeviceChannel.GetData<Ganglion_EEGData>());
                _SetMetric($"Ganglion", "EEGData", _focusingLabDeviceChannel.GetData<Ganglion_EEGData>());
                _SetMetric($"Ganglion", "ImpedanceData", _focusingLabDeviceChannel.GetData<Ganglion_ImpedanceData>());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[LabDevicePanel] Error fetching Ganglion Data" + e);
        }

        // EyeTrack
        try
        {
            _SetMetric($"EyeTrack", "IsAvailable", _focusingLabDeviceChannel.GetData<bool>(LabDeviceChannel.LabDeviceControl.EYETRACK_ISAVAILABLE));
            var eyeLeftRightData = _focusingLabDeviceChannel.GetData<EyeLeftRightData>();
            _SetMetric($"EyeTrack.EyeLeftRightData", "Timestamp", eyeLeftRightData?.Timestamp);
            _SetMetric($"EyeTrack.EyeLeftRightData", "LeftEyeOpenness", eyeLeftRightData?.LeftEyeOpenness);
            _SetMetric($"EyeTrack.EyeLeftRightData", "RightEyeOpenness", eyeLeftRightData?.RightEyeOpenness);
            _SetMetric($"EyeTrack.EyeLeftRightData", "LeftEyePositionGuide", eyeLeftRightData?.LeftEyePositionGuide);
            _SetMetric($"EyeTrack.EyeLeftRightData", "RightEyePositionGuide", eyeLeftRightData?.RightEyePositionGuide);
            var eyeCombinedData = _focusingLabDeviceChannel.GetData<EyeCombinedData>();
            _SetMetric($"EyeTrack.CombinedData", "Timestamp", eyeCombinedData.Timestamp);
            _SetMetric($"EyeTrack.CombinedData", "CombineEyeGazeVector", eyeCombinedData?.CombineEyeGazeVector);
            _SetMetric($"EyeTrack.CombinedData", "CombineEyeGazePoint", eyeCombinedData?.CombineEyeGazePoint);
            var eyeFocusData = _focusingLabDeviceChannel.GetData<EyeFocusData>();
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
            _SetMetric($"BreathStrap", "IsAvailable", _focusingLabDeviceChannel.GetData<bool>(LabDeviceChannel.LabDeviceControl.BREATHSTRAP_ISAVAILABLE));
            _SetMetric($"BreathStrap", "IsConnected", _focusingLabDeviceChannel.GetData<bool>(LabDeviceChannel.LabDeviceControl.BREATHSTRAP_ISCONNECTED));
            _SetMetric($"BreathStrap", "BreathValue", _focusingLabDeviceChannel.GetData<BreathStrapData>(LabDeviceChannel.LabDeviceControl.BREATHSTRAP_BREATHDATA)?.breathValue);
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
