using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LabFrame2023;

[RequireComponent(typeof(LokaHost))]
public class LokaDataManager : MonoBehaviour
{
    protected bool _isInited = false;

    [Header("是否收集玩家實驗資料 LabData")]
    public bool CollectEyeTrack = false;
    public bool CollectGanglion = false;
    public bool CollectBreathStrap = false;
    

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
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
                CollectData(player, channel.GetGanglionEEGData());
                CollectData(player, channel.GetGanglionImpedanceData());
            }
            if(CollectEyeTrack && channel.GetEyeTrackIsAvailable())
            {
                CollectData(player, channel.GetEyeTrackEyeLeftRightData());
                CollectData(player, channel.GetEyeTrackEyeCombinedData());
                CollectData(player, channel.GetEyeTrackEyeFocusData());
            }
            if(CollectBreathStrap && channel.GetBreathStrapIsConnected())
            {
                CollectData(player, channel.GetBreathStrapData());
            }
        }        
    }

    /* -------------------------------------------------------------------------- */

    public void CollectData(LokaPlayer player, LabDataBase data)
    {
        if(!LabDataManager.Instance.IsInited)
        {
            // 用時間做為實驗資料的 UserID (LabData 資料夾名)
            LabDataManager.Instance.LabDataInit(System.DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        }

        if(data == null)
        {
            Debug.LogWarning("Data is null");
            return;
        }
        LabDataManager.Instance.WriteData(data, player.ConnectionId);
    }
}