using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LabFrame2023;
using Unity.WebRTC;
using System.Linq;
using System;

public class LokaRtcStatsManager : MonoBehaviour
{
    public float RefreshInterval = 0.3f;
    /// <summary>
    /// <c>[Player]</c>
    /// </summary>
    public Dictionary<LokaPlayer, LokaRtcStatsReport> StatsReports = new Dictionary<LokaPlayer, LokaRtcStatsReport>();

    /// <summary>
    /// Stats Updated Event
    /// </summary>
    public event Action<LokaPlayer, LokaRtcStatsReport> OnStatsUpdated;

    /* -------------------------------------------------------------------------- */


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        StartCoroutine(RefreshStatsAsync());
    }

    IEnumerator RefreshStatsAsync()
    {
        while(true)
        {
            foreach(LokaPlayer player in LokaHost.Instance.ConnectedPlayers.Values)
            {
                int i = 0;
                foreach(var transceiver in player.VideoSender.Transceivers.Values)
                {
                    yield return UpdateStatsAsync(transceiver.Sender?.GetStats(), player, "VideoOut"+i);
                    i++;
                }
                // FIXME audio 先不收
                // i = 0;
                // foreach(var transceiver in player.AudioSender.Transceivers.Values)
                // {
                //     yield return UpdateStatsAsync(transceiver.Sender?.GetStats(), player, "AudioOut"+i);
                //     i++;
                // }                
            }

            yield return new WaitForSecondsRealtime(RefreshInterval);
        }
    }
    

    /// <summary>
    /// Update stats
    /// </summary>
    /// <param name="getStatsTask">no need to await first</param>
    /// <param name="transceiverTag">transceiver tag (Display on panel)</param>
    /// <returns></returns>
    protected IEnumerator UpdateStatsAsync(RTCStatsReportAsyncOperation getStatsTask, LokaPlayer player, string transceiverTag)
    {
        if(getStatsTask == null)
        {
            yield break;
        }

        // run getStatsTask
        yield return getStatsTask;

        // handle error
        if(getStatsTask.IsError)
        {
            Debug.LogWarning($"[StatsPanel] Read Stats Error: {getStatsTask.Error.message}");
            yield break;
        }

        // Get report of that player
        if(!StatsReports.ContainsKey(player))
        {
            StatsReports[player] = new LokaRtcStatsReport();
        }
        LokaRtcStatsReport report = StatsReports[player];

        // get report timestamp & check duplicate data
        // 注意：remote 的 stats 可能會有不同的 timestamp，看要不要檢查 (e.g. RemoteInboundRtp)
        var firstStat = getStatsTask.Value.Stats.FirstOrDefault();
        if(firstStat.Value == null)
            yield break;
        long timeStamp = firstStat.Value.Timestamp; 
        long lastTimestamp = 0;  
        if(report.Metrics.ContainsKey($"{transceiverTag}.timestamp"))
            lastTimestamp = (long)report.Metrics[$"{transceiverTag}.timestamp"];
        if(timeStamp <= lastTimestamp)
            yield break;

        // iterate stats of each type
        // FIXME should remove LokaRtcStatsReportPanelBase.cs
        foreach(var stat in getStatsTask.Value.Stats)
        {
            RTCStatsType rtcStatType = stat.Value.Type;   
            
            // if(timeStamp != stat.Value.Timestamp)
            // {
            //     Debug.LogWarning($"RTC Stat time unmatch: {rtcStatType} expect {timeStamp} but this stat is {stat.Value.Timestamp}");
            // }

            // Save highlighted Stats
            if(rtcStatType == RTCStatsType.InboundRtp)
            {
                var inboundRtpStat = (RTCInboundRTPStreamStats)stat.Value;
                // bitrate
                ulong bytesReceived = inboundRtpStat.bytesReceived;
                ulong lastBytesReceived = 0;
                if(report.Metrics.ContainsKey($"{transceiverTag}.{rtcStatType}.bytesReceived"))
                    lastBytesReceived = (ulong)report.Metrics[$"{transceiverTag}.{rtcStatType}.bytesReceived"];
                double timeDelta = (timeStamp - lastTimestamp)/1_000_000d; // μs to s   
                ulong bytesDelta = bytesReceived - lastBytesReceived;
                var bitrate = bytesDelta * 8 / timeDelta;                
                report.HighlightedMetrics[$"{transceiverTag}.bitrate (kbps)"] =  bitrate/1_000d;
                
                // other stats
                report.HighlightedMetrics[$"{transceiverTag}.received (MB)"] = inboundRtpStat.bytesReceived/1_000_000d;
                report.HighlightedMetrics[$"{transceiverTag}.jitter (ms)"] = inboundRtpStat.jitter*1000d;
            }
            else if(rtcStatType == RTCStatsType.OutboundRtp)
            {
                var outRtpStat = (RTCOutboundRTPStreamStats)stat.Value;  

                // bitrate              
                ulong bytesSent = outRtpStat.bytesSent;
                ulong lastBytesSent = 0;
                if(report.Metrics.ContainsKey($"{transceiverTag}.{rtcStatType}.bytesSent"))
                    lastBytesSent = (ulong)report.Metrics[$"{transceiverTag}.{rtcStatType}.bytesSent"];                
                double timeDelta = (timeStamp - lastTimestamp)/1_000_000d; // μs to s
                ulong bytesDelta = bytesSent - lastBytesSent;
                var bitrate = bytesDelta * 8 / timeDelta;
                report.HighlightedMetrics[$"{transceiverTag}.bitrate (kbps)"] = bitrate/1_000d;

                report.HighlightedMetrics[$"{transceiverTag}.targetBitrate (kbps)"] = outRtpStat.targetBitrate/1_000d;
                report.HighlightedMetrics[$"{transceiverTag}.sent (MB)"] = outRtpStat.bytesSent/1_000_000d;
                report.HighlightedMetrics[$"{transceiverTag}.packetSent"] = outRtpStat.packetsSent;
                report.HighlightedMetrics[$"{transceiverTag}.framePerSeconds"] = outRtpStat.framesPerSecond;
            }
            else if(rtcStatType == RTCStatsType.RemoteInboundRtp)
            {
                var remoteInRtpStat = (RTCRemoteInboundRtpStreamStats)stat.Value;
                report.HighlightedMetrics[$"{transceiverTag}.packetsLost"] = remoteInRtpStat.packetsLost;
                report.HighlightedMetrics[$"{transceiverTag}.roundTripTime (ms)"] = remoteInRtpStat.roundTripTime*1000d; // s to ms
                report.HighlightedMetrics[$"{transceiverTag}.jitter (ms)"] = remoteInRtpStat.jitter*1000d; 
            }
            
            // Save all stats
            foreach(var pair in stat.Value.Dict)
            {                
                report.Metrics[$"{transceiverTag}.{rtcStatType}.{pair.Key}"] = pair.Value;
            }            

        }        

        // update timestamp
        report.Timestamp = DateTimeOffset.Now.ToString("o");
        report.Metrics[$"{transceiverTag}.timestamp"] = timeStamp;

        // 這裡 clone 一份 report 給外部使用
        LokaRtcStatsReport cloneReport = new LokaRtcStatsReport
        {
            Timestamp = report.Timestamp,
            Metrics = new Dictionary<string, object>(report.Metrics),
            HighlightedMetrics = new Dictionary<string, object>(report.HighlightedMetrics)
        };
        OnStatsUpdated?.Invoke(player, cloneReport);
    }   

}

public class LokaRtcStatsReport : LabDataBase
{
    /// <summary>
    /// 特別標記的指標，可在下方建構子設定
    /// </summary>
    public Dictionary<string, object> HighlightedMetrics = new Dictionary<string, object>();
    /// <summary>
    /// 連線指標
    /// <c>Metrics[{transceiverTag}.{rtcStatType}.{metricName}]</c>
    /// </summary>
    public Dictionary<string, object> Metrics = new Dictionary<string, object>();

    public LokaRtcStatsReport(){}
}