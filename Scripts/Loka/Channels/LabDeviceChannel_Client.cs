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

        // Ganglion
        GanglionManager.Instance.OnEEGUpdated += (eegData) =>
        {
            ClientSendMessage(LabDeviceControl.GANGLION_EEGDATA, eegData);
        };
        GanglionManager.Instance.OnImpedanceUpdated += (impedanceData) =>
        {
            ClientSendMessage(LabDeviceControl.GANGLION_IMPEDANCEDATA, impedanceData);
        };

        // Breath
        BreathStrapManager.Instance.OnBreathValueUpdated += (breathData) =>
        {
            ClientSendMessage(LabDeviceControl.BREATHSTRAP_BREATHDATA, breathData);
        };
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void LocalUpdate()
    {
        if (!IsLocal)
            return;

        try
        {
            // Ganglion
            ClientSendMessage(LabDeviceControl.GANGLION_ISAVAILABLE, true); // FIXME Ganglion_IsAvailable
            ClientSendMessage(LabDeviceControl.GANGLION_ISCONNECTED, GanglionManager.Instance.IsConnected);            

            // EyeTrack
            ClientSendMessage(LabDeviceControl.EYETRACK_ISAVAILABLE, true); // TODO EyeTrack_IsAvailable
            ClientSendMessage(LabDeviceControl.EYETRACK_EYELEFTRIGHTDATA, EyeTrackManager.Instance.GetEyeLeftRightData());
            ClientSendMessage(LabDeviceControl.EYETRACK_COMBINEDDATA, EyeTrackManager.Instance.GetEyeCombinedData());
            // ClientSendMessage($"EyeTrack.EyeFocusData", EyeTrackManager.Instance.GetEyeFocusData());  // client 一般都對著投影看，沒有意義

            // Breath
            ClientSendMessage(LabDeviceControl.BREATHSTRAP_ISAVAILABLE, true); // TODO Breath_IsAvailable
            ClientSendMessage(LabDeviceControl.BREATHSTRAP_ISCONNECTED, BreathStrapManager.Instance.IsConnected());
  
            // Hands
            ClientSendMessage(LabDeviceControl.HAND_LEFT_JOINTS_POSE, LocalHandJointsManager.Instance?.GetJoints(Handedness.Left));
            ClientSendMessage(LabDeviceControl.HAND_RIGHT_JOINTS_POSE, LocalHandJointsManager.Instance?.GetJoints(Handedness.Right));
        }
        catch(Exception e)
        {
            Debug.LogError($"[LabDeviceChannel_Client] {e}");
        }
 
    }

    private void ClientSendMessage(LabDeviceControl tag, object msg)
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
}