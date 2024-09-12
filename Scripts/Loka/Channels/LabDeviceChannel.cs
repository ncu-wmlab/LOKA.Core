using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using System;
using Newtonsoft.Json.Linq;
using Unity.WebRTC;

public partial class LabDeviceChannel : LokaChannel
{
    /// <summary>
    /// Host 與 Client 要試圖同步的資料 <br />
    /// <c>_datas[dataName] = dataValueObj</c>
    /// </summary>
    Dictionary<LabDeviceSignal, object> _datas = new Dictionary<LabDeviceSignal, object>();

/* ---------------------------- 各模態資料的取得/操作 tag enum --------------------------- */

    // Client -> Host
    public enum LabDeviceSignal
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
    
/* ------------------------------- 各模態資料的取得函數 ------------------------------- */

    // 10
    public bool GetGanglionIsAvailable()
    {
        return GetData<bool>(LabDeviceSignal.GANGLION_ISAVAILABLE);
    }
    public bool GetGanglionIsConnected()
    {
        return GetData<bool>(LabDeviceSignal.GANGLION_ISCONNECTED);
    }
    public Ganglion_EEGData GetGanglionEEGData()
    {
        return GetData<Ganglion_EEGData>(LabDeviceSignal.GANGLION_EEGDATA);
    }
    public Ganglion_ImpedanceData GetGanglionImpedanceData()
    {
        return GetData<Ganglion_ImpedanceData>(LabDeviceSignal.GANGLION_IMPEDANCEDATA);
    }

    // 20
    public bool GetEyeTrackIsAvailable()
    {
        return GetData<bool>(LabDeviceSignal.EYETRACK_ISAVAILABLE);
    }
    public EyeLeftRightData GetEyeTrackEyeLeftRightData()
    {
        return GetData<EyeLeftRightData>(LabDeviceSignal.EYETRACK_EYELEFTRIGHTDATA);
    }
    public EyeCombinedData GetEyeTrackEyeCombinedData()
    {
        return GetData<EyeCombinedData>(LabDeviceSignal.EYETRACK_COMBINEDDATA);
    }
    public EyeFocusData GetEyeTrackEyeFocusData()
    {
        return GetData<EyeFocusData>(LabDeviceSignal.EYETRACK_EYEFOCUSDATA);
    }

    // 30
    public bool GetBreathStrapIsAvailable()
    {
        return GetData<bool>(LabDeviceSignal.BREATHSTRAP_ISAVAILABLE);
    }
    public bool GetBreathStrapIsConnected()
    {
        return GetData<bool>(LabDeviceSignal.BREATHSTRAP_ISCONNECTED);
    }
    public BreathStrapData GetBreathStrapData()
    {
        return GetData<BreathStrapData>(LabDeviceSignal.BREATHSTRAP_BREATHDATA);
    }

    // 40
    public List<Pose?> GetHandLeftJointsPose()
    {
        return GetData<List<Pose?>>(LabDeviceSignal.HAND_LEFT_JOINTS_POSE);
    }
    public List<Pose?> GetHandRightJointsPose()
    {
        return GetData<List<Pose?>>(LabDeviceSignal.HAND_RIGHT_JOINTS_POSE);
    }


/* --------------------------- Unity Monobehaiour --------------------------- */

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

/* -------------------------- LokaChannel Override -------------------------- */

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
    /// Get Current stored data by type and Data key.
    /// If not found, return null!
    /// <typeparam name="T">The type</typeparam>
    /// <param name="key">the key</param>
    /// <returns></returns>
    public T GetData<T>(LabDeviceSignal key)
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