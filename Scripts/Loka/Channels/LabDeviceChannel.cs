using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using System;

public partial class LabDeviceChannel : LokaChannel
{
    /// <summary>
    /// _datas[dataName] = value_obj
    /// </summary>
    Dictionary<DataEnum, object> _datas = new Dictionary<DataEnum, object>();

/* -------------------------------------------------------------------------- */

    public enum DataEnum
    {
        GANGLION_ISAVAILABLE = 10,
        GANGLION_ISCONNECTED,
        GANGLION_EEGDATA,
        GANGLION_IMPEDANCEDATA,
        EYETRACK_ISAVAILABLE = 20,
        EYETRACK_EYELEFTRIGHTDATA,
        EYETRACK_COMBINEDDATA,
        EYETRACK_EYEFOCUSDATA,
        BREATHSTRAP_ISAVAILABLE = 30,
        BREATHSTRAP_ISCONNECTED,
        BREATHSTRAP_BREATHDATA ,
    }

/* -------------------------------------------------------------------------- */

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(IsLocal)
            LocalStart();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(IsLocal)
            LocalUpdate();
    }

/* -------------------------------------------------------------------------- */

    protected override void MessageReceived(string tag, object msg)
    {
        if(IsLocal)
            return;

        _datas[(DataEnum)int.Parse(tag)] = msg;
        // print($"LabDeviceChannel RECV [{tag}] {msg}");
    }

/* -------------------------------------------------------------------------- */
    
    public T GetData<T>()
    {        
        switch(typeof(T).Name)
        {
            case "Ganglion_EEGData":
                return GetData<T>(DataEnum.GANGLION_EEGDATA);
            case "Ganglion_ImpedanceData":
                return GetData<T>(DataEnum.GANGLION_IMPEDANCEDATA);
            case "EyeLeftRightData":
                return GetData<T>(DataEnum.EYETRACK_EYELEFTRIGHTDATA);
            case "EyeCombinedData":
                return GetData<T>(DataEnum.EYETRACK_COMBINEDDATA);
            case "EyeFocusData":
                return GetData<T>(DataEnum.EYETRACK_EYEFOCUSDATA);
            case "BreathStrapData":
                return GetData<T>(DataEnum.BREATHSTRAP_BREATHDATA);
        }
        
        // fallback
        foreach(var key in _datas.Keys)
        {
            if(_datas[key] is T)
                return (T)_datas[key];
        }
        return default;
    }

    public T GetData<T>(DataEnum key)
    {
        if(!_datas.ContainsKey(key))
        {
            Debug.LogWarning($"[LabDeviceChannel] GetData: Key not found: {key}");
            return default;
        }
        return (T)_datas[key];
    }

/* -------------------------------------------------------------------------- */
}

// public class LokaLabDeviceData
// {
//     public bool Ganglion_IsConnected;
//     public Ganglion_EEGData Ganglion_EEGData;
//     public Ganglion_ImpedanceData Ganglion_ImpedanceData;

//     public bool EyeTrack_IsAvailable;
//     public EyeLeftRightData EyeTrack_EyeLeftRightData;
//     public EyeCombinedData EyeTrack_CombinedData;
//     public EyeFocusData EyeTrack_EyeFocusData;

//     public bool BreathStrap_IsConnected;
//     public BreathStrapData BreathStrap_BreathData;
// }