using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LabDevicePanel : BaseDictPanel
{
    LabDeviceChannel _labDeviceChannel;

    /* -------------------------------------------------------------------------- */

    public void SetLabDeviceChannel(LabDeviceChannel labDeviceChannel)
    {
        _labDeviceChannel = labDeviceChannel;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    new void Awake()
    {
        SetLabDeviceChannel(FindObjectOfType<LabDeviceChannel>());
        base.Awake();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected override void Update()
    {
        if(!_labDeviceChannel)
            return;

        _SetMetric("", "IsConnected", _labDeviceChannel.IsConnected);

        // Ganglion
        try
        {
            _SetMetric($"Ganglion", "IsAvailable", _labDeviceChannel.GetData<bool>(LabDeviceChannel.DataEnum.GANGLION_ISAVAILABLE));
            _SetMetric($"Ganglion", "IsConnected", _labDeviceChannel.GetData<bool>(LabDeviceChannel.DataEnum.GANGLION_EEGDATA));
            if (GanglionManager.Instance.IsConnected)
            {
                _SetMetric($"Ganglion", "EEGData", _labDeviceChannel.GetData<Ganglion_EEGData>());
                _SetMetric($"Ganglion", "ImpedanceData", _labDeviceChannel.GetData<Ganglion_ImpedanceData>());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[LabDevicePanel] Error fetching Ganglion Data" + e);
        }

        // EyeTrack
        try
        {
            _SetMetric($"EyeTrack", "IsAvailable", _labDeviceChannel.GetData<bool>(LabDeviceChannel.DataEnum.EYETRACK_ISAVAILABLE));
            var eyeLeftRightData = _labDeviceChannel.GetData<EyeLeftRightData>();
            _SetMetric($"EyeTrack.EyeLeftRightData", "Timestamp", eyeLeftRightData.Timestamp);
            _SetMetric($"EyeTrack.EyeLeftRightData", "LeftEyeOpenness", eyeLeftRightData.LeftEyeOpenness);
            _SetMetric($"EyeTrack.EyeLeftRightData", "RightEyeOpenness", eyeLeftRightData.RightEyeOpenness);
            _SetMetric($"EyeTrack.EyeLeftRightData", "LeftEyePositionGuide", eyeLeftRightData.LeftEyePositionGuide);
            _SetMetric($"EyeTrack.EyeLeftRightData", "RightEyePositionGuide", eyeLeftRightData.RightEyePositionGuide);
            var eyeCombinedData = _labDeviceChannel.GetData<EyeCombinedData>();
            _SetMetric($"EyeTrack.CombinedData", "Timestamp", eyeCombinedData.Timestamp);
            _SetMetric($"EyeTrack.CombinedData", "CombineEyeGazeVector", eyeCombinedData.CombineEyeGazeVector);
            _SetMetric($"EyeTrack.CombinedData", "CombineEyeGazePoint", eyeCombinedData.CombineEyeGazePoint);
            var eyeFocusData = _labDeviceChannel.GetData<EyeFocusData>();
            _SetMetric($"EyeTrack.EyeFocusData", "FocusDistance", eyeFocusData.FocusDistance);
            _SetMetric($"EyeTrack.EyeFocusData", "Timestamp", eyeFocusData.Timestamp);
            _SetMetric($"EyeTrack.EyeFocusData", "FocusName", eyeFocusData.FocusName);
            _SetMetric($"EyeTrack.EyeFocusData", "FocusNormal", eyeFocusData.FocusNormal);
            _SetMetric($"EyeTrack.EyeFocusData", "FocusPoint", eyeFocusData.FocusPoint);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LabDevicePanel] Error fetching EyeTrack Data: " + e);
        }

        // Breath
        try
        {
            _SetMetric($"BreathStrap", "IsConnected", _labDeviceChannel.GetData<bool>(LabDeviceChannel.DataEnum.BREATHSTRAP_ISCONNECTED));
            _SetMetric($"BreathStrap", "BreathValue", _labDeviceChannel.GetData<BreathStrapData>(LabDeviceChannel.DataEnum.BREATHSTRAP_BREATHDATA).breathValue);
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
}
