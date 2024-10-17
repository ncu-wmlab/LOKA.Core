using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LabFrame2023;
using System.Linq;
using System;
using Newtonsoft.Json;

[RequireComponent(typeof(LokaHost))]
[RequireComponent(typeof(LokaRtcStatsManager))]
public class LokaDataManager : MonoBehaviour
{
    public static LokaDataManager Instance { get; private set; }


    [Header("是否收集玩家實驗資料 LabData")]
    public bool CollectEyeTrack = false;
    public bool CollectGanglion = false;
    public bool CollectBreathStrap = false;

    [Header("是否收集玩家連線指標")]
    public bool CollectConnectionStats = false;

    LokaRtcStatsManager lokaRtcStatsManager;


    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("LokaDataManager already exists. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        lokaRtcStatsManager = GetComponent<LokaRtcStatsManager>();
        lokaRtcStatsManager.OnStatsUpdated += (player, report) =>
        {
            if(CollectConnectionStats)
            {
                SaveData(player, report);
            }
        } ;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        foreach(LokaPlayer player in LokaHost.Instance.ConnectedPlayers.Values)
        {
            var channel = player.LabDeviceChannel;
            if(CollectGanglion && channel.GetGanglionIsConnected())
            {
                SaveData(player, channel.GetGanglionEEGData());
                SaveData(player, channel.GetGanglionImpedanceData());
            }
            if(CollectEyeTrack && channel.GetEyeTrackIsAvailable())
            {
                SaveData(player, channel.GetEyeTrackEyeLeftRightData());
                SaveData(player, channel.GetEyeTrackEyeCombinedData());
                SaveData(player, channel.GetEyeTrackEyeFocusData());
            }
            if(CollectBreathStrap && channel.GetBreathStrapIsConnected())
            {
                SaveData(player, channel.GetBreathStrapData());
            }            
        }        
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 儲存單一玩家的實驗資料
    /// </summary>
    /// <param name="player"></param>
    /// <param name="data"></param>
    public void SaveData(LokaPlayer player, LabDataBase data)
    {
        SaveData(player.ConnectionId, data);
    }

    public void SaveData(string appendix, LabDataBase data)
    {
        if(!LabDataManager.Instance.IsInited)
        {
            // 用時間做為實驗資料的 UserID (LabData 資料夾名)
            LabDataManager.Instance.LabDataInit(DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        }

        if(data == null)
        {
            // Debug.LogWarning("Data is null");
            return;
        }
        try
        {
            LabDataManager.Instance.WriteData(data, appendix);
        }
        catch(Exception e)
        {
            Debug.LogError($"[LokaDataManager] SaveData Error: {e.Message}");
        }
    }
}