using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

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

        /* 註冊接收訊號事件 */
        // Ganglion
        GanglionManager.Instance.OnEEGUpdated += (eegData) =>
        {
            ClientSendMessage(LabDeviceSignal.GANGLION_EEGDATA, eegData);
        };
        GanglionManager.Instance.OnImpedanceUpdated += (impedanceData) =>
        {
            ClientSendMessage(LabDeviceSignal.GANGLION_IMPEDANCEDATA, impedanceData);
        };

        // Breath
        BreathStrapManager.Instance.OnBreathValueUpdated += (breathData) =>
        {
            ClientSendMessage(LabDeviceSignal.BREATHSTRAP_BREATHDATA, breathData);
        };
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void LocalUpdate()
    {
        if (!IsLocal)
            return;

        /* 每一幀都收的訊號 */
        try
        {
            // Ganglion
            ClientSendMessage(LabDeviceSignal.GANGLION_ISAVAILABLE, true); // FIXME Ganglion_IsAvailable
            ClientSendMessage(LabDeviceSignal.GANGLION_ISCONNECTED, GanglionManager.Instance.IsConnected);            

            // EyeTrack
            ClientSendMessage(LabDeviceSignal.EYETRACK_ISAVAILABLE, true); // TODO EyeTrack_IsAvailable
            ClientSendMessage(LabDeviceSignal.EYETRACK_EYELEFTRIGHTDATA, EyeTrackManager.Instance.GetEyeLeftRightData());
            ClientSendMessage(LabDeviceSignal.EYETRACK_COMBINEDDATA, EyeTrackManager.Instance.GetEyeCombinedData());
            // ClientSendMessage($"EyeTrack.EyeFocusData", EyeTrackManager.Instance.GetEyeFocusData());  // client 一般都對著投影看，沒有意義

            // Breath
            ClientSendMessage(LabDeviceSignal.BREATHSTRAP_ISAVAILABLE, true); // TODO Breath_IsAvailable
            ClientSendMessage(LabDeviceSignal.BREATHSTRAP_ISCONNECTED, BreathStrapManager.Instance.IsConnected());
  
            // Hands
            ClientSendMessage(LabDeviceSignal.HAND_LEFT_JOINTS_POSE, LocalHandJointsManager.Instance?.GetJoints(Handedness.Left));
            ClientSendMessage(LabDeviceSignal.HAND_RIGHT_JOINTS_POSE, LocalHandJointsManager.Instance?.GetJoints(Handedness.Right));
        }
        catch(Exception e)
        {
            Debug.LogError($"[LabDeviceChannel_Client] {e}");
        }
    }

    /* -------------------------------------------------------------------------- */

    private void ClientSendMessage(LabDeviceSignal tag, object msg)
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
        LabDeviceCommand dataEnum = (LabDeviceCommand)tag;
        switch(dataEnum)
        {
            case LabDeviceCommand.GANGLION_DO_CONNECT:
                if((bool)msg)
                    GanglionManager.Instance.ManualConnect();
                else
                    GanglionManager.Instance.ManualDisconnect();
                break;
            case LabDeviceCommand.GANGLION_RECEIVE_EEG:
                if((bool)msg)
                    GanglionManager.Instance.StreamData();
                else
                    GanglionManager.Instance.StopStreamData();
                break;
            case LabDeviceCommand.GANGLION_RECEIVE_IMPEDANCE:
                if((bool)msg)
                    GanglionManager.Instance.StreamImpedance();
                else
                    GanglionManager.Instance.StopStreamImpedance();
                break;
            case LabDeviceCommand.BREATHSTRAP_DO_CONNECT:
                if((bool)msg)
                    BreathStrapManager.Instance.Connect();
                else
                    BreathStrapManager.Instance.Disconnect();
                break;
        }
    }

    /* -------------------------------------------------------------------------- */
}