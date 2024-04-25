using System;
using System.Collections.Generic;
using UnityEngine;

// CLIENT (LOCAL) CODE
public partial class LabDeviceChannel : LokaChannel
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void LocalStart()
    {
        if (!IsLocal)
            return;

        // Ganglion
        GanglionManager.Instance.OnEEGUpdated += (eegData) =>
        {
            ClientSendMessage(DataEnum.GANGLION_EEGDATA, eegData);
        };
        GanglionManager.Instance.OnImpedanceUpdated += (impedanceData) =>
        {
            ClientSendMessage(DataEnum.GANGLION_IMPEDANCEDATA, impedanceData);
        };

        // Breath
        BreathStrapManager.Instance.OnBreathValueUpdated += (breathData) =>
        {
            ClientSendMessage(DataEnum.BREATHSTRAP_BREATHDATA, breathData);
        };
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void LocalUpdate()
    {
        if (!IsLocal)
            return;

        // Ganglion
        try
        {
            ClientSendMessage(DataEnum.GANGLION_ISAVAILABLE, true); // FIXME Ganglion_IsAvailable
            ClientSendMessage(DataEnum.GANGLION_ISCONNECTED, GanglionManager.Instance.IsConnected);
        }
        catch (Exception e)
        {
            // Debug.LogWarning("[LabDevicePanel] Error fetching Ganglion Data: " + e);
        }

        // EyeTrack
        // try
        // {
        //     ClientSendMessage(DataEnum.EYETRACK_ISAVAILABLE, true); // TODO EyeTrack_IsAvailable
        //     ClientSendMessage(DataEnum.EYETRACK_EYELEFTRIGHTDATA, EyeTrackManager.Instance.GetEyeLeftRightData());
        //     ClientSendMessage(DataEnum.EYETRACK_COMBINEDDATA, EyeTrackManager.Instance.GetEyeCombinedData());
        //     // ClientSendMessage($"EyeTrack.EyeFocusData", EyeTrackManager.Instance.GetEyeFocusData());
        // }
        // catch (Exception e)
        // {
        //     // Debug.LogWarning("[LabDevicePanel] Error fetching EyeTrack Data: " + e);
        // }

        // Breath
        try
        {
            ClientSendMessage(DataEnum.BREATHSTRAP_ISAVAILABLE, true); // TODO Breath_IsAvailable
            ClientSendMessage(DataEnum.BREATHSTRAP_ISCONNECTED, BreathStrapManager.Instance.IsConnected());
        }
        catch (Exception e)
        {
            // Debug.LogWarning("[LabDevicePanel] Error fetching Breath Data: " + e);
        }
    }

    private void ClientSendMessage(DataEnum tag, object msg)
    {
        _datas[tag] = msg;

        // TODO ignore duplication
        if (IsConnected && msg != null)
        {
            Send((int)tag, msg);
        }
    }

    private void ClientReceiveMessage(int tag, object msg)
    {
        LabDeviceOp dataEnum = (LabDeviceOp)tag;
        switch(dataEnum)
        {
            case LabDeviceOp.GANGLION_DO_CONNECT:
                if((bool)msg)
                    GanglionManager.Instance.ManualConnect();
                else
                    GanglionManager.Instance.ManualDisconnect();
                break;
            case LabDeviceOp.GANGLION_RECEIVE_EEG:
                if((bool)msg)
                    GanglionManager.Instance.StreamData();
                else
                    GanglionManager.Instance.StopStreamData();
                break;
            case LabDeviceOp.GANGLION_RECEIVE_IMPEDANCE:
                if((bool)msg)
                    GanglionManager.Instance.StreamImpedance();
                else
                    GanglionManager.Instance.StopStreamImpedance();
                break;
            case LabDeviceOp.BREATHSTRAP_DO_CONNECT:
                if((bool)msg)
                    BreathStrapManager.Instance.Connect();
                else
                    BreathStrapManager.Instance.Disconnect();
                break;
        }
    }
}