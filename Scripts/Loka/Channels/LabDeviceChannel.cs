using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using System;
using Newtonsoft.Json.Linq;

public partial class LabDeviceChannel : LokaChannel
{
    /// <summary>
    /// _datas[dataName] = value_obj
    /// </summary>
    Dictionary<LabDeviceControl, object> _datas = new Dictionary<LabDeviceControl, object>();

/* -------------------------------------------------------------------------- */

    // Usually Client -> Host
    public enum LabDeviceControl
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
        BREATHSTRAP_BREATHDATA,
        HAND_LEFT_JOINTS_POSE = 40,
        HAND_RIGHT_JOINTS_POSE
    }

    // Host -> Client
    public enum LabDeviceCommand
    {
        GANGLION_DO_CONNECT = 10010,
        GANGLION_RECEIVE_EEG,
        GANGLION_RECEIVE_IMPEDANCE,
        BREATHSTRAP_DO_CONNECT = 10030,
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
        else
            HostStart();
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

    protected override void OnMessageReceive(int tag, object msg)
    {
        if(IsLocal)
            ClientReceiveMessage(tag, msg);
        else
            HostReceiveMessage(tag, msg);        
        // print($"LabDeviceChannel RECV [{tag}] {msg}");
    }    

    

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Get Current retrieved data by type.
    /// If not found, return null!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetData<T>()
    {        
        switch(typeof(T).Name)
        {
            case "Ganglion_EEGData":
                return GetData<T>(LabDeviceControl.GANGLION_EEGDATA);
            case "Ganglion_ImpedanceData":
                return GetData<T>(LabDeviceControl.GANGLION_IMPEDANCEDATA);
            case "EyeLeftRightData":
                return GetData<T>(LabDeviceControl.EYETRACK_EYELEFTRIGHTDATA);
            case "EyeCombinedData":
                return GetData<T>(LabDeviceControl.EYETRACK_COMBINEDDATA);
            case "EyeFocusData":
                return GetData<T>(LabDeviceControl.EYETRACK_EYEFOCUSDATA);
            case "BreathStrapData":
                return GetData<T>(LabDeviceControl.BREATHSTRAP_BREATHDATA);
        }
        
        // fallback
        foreach(var key in _datas.Keys)
        {
            if(_datas[key] is T)
                return (T)_datas[key];
        }
        return default;
    }

    /// <summary>
    /// Get Current retrieved data by type and Data key.
    /// If not found, return null!
    /// <typeparam name="T">The type</typeparam>
    /// <param name="key">the key</param>
    /// <returns></returns>
    public T GetData<T>(LabDeviceControl key)
    {
        if(!_datas.ContainsKey(key))
        {
            // Debug.LogWarning($"[LabDeviceChannel] GetData: Key not found: {key}, returning default value (null).");
            return default;
        }

        // FIXME if we migrate from JSON.net, we need to change here :/
        // print($"[{key}]  is " + _datas[key].GetType());
        if(_datas[key] is JObject)  
            return ((JObject)_datas[key]).ToObject<T>();  
        if(_datas[key] is JArray)
            return ((JArray)_datas[key]).ToObject<T>();
        return (T)_datas[key];
    }
}