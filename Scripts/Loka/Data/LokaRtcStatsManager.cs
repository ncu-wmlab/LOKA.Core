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
                var audioOut = player.AudioSender?.Transceivers?.FirstOrDefault().Value?.Sender;
                if(audioOut != null)
                {
                    yield return UpdateStatsAsync(audioOut.GetStats(), player, "AudioOut");
                }

                var videoOut = player.VideoSender?.Transceivers?.FirstOrDefault().Value?.Sender;
                if(videoOut != null)
                {
                    yield return UpdateStatsAsync(videoOut.GetStats(), player, "VideoOut");
                }
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
        yield return getStatsTask;
        if(getStatsTask.IsError)
        {
            Debug.LogWarning($"[StatsPanel] Read Stats Error: {getStatsTask.Error.message}");
            yield break;
        }

        // iterate stats
        // FIXME should remove LokaRtcStatsReportPanelBase.cs
        foreach(var stat in getStatsTask.Value.Stats)
        {
            RTCStatsType rtcStatType = stat.Value.Type;
            long timeStamp = stat.Value.Timestamp;            
            
            // Get report of that player
            if(!StatsReports.ContainsKey(player))
            {
                StatsReports[player] = new LokaRtcStatsReport();
            }
            LokaRtcStatsReport report = StatsReports[player];

            // check dulplicate data
            long lastTimestamp = 0;  
            if(report.Metrics.ContainsKey($"{transceiverTag}.timestamp"))
                lastTimestamp = (long)report.Metrics[$"{transceiverTag}.timestamp"];
            if(timeStamp <= lastTimestamp)
                continue;

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
                report.HighlightedMetrics[$"{transceiverTag}.bitrate (Mbps)"] =  bitrate/1_000_000d;
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
                report.HighlightedMetrics[$"{transceiverTag}.bitrate (Mbps)"] = bitrate/1_000_000d;

                report.HighlightedMetrics[$"{transceiverTag}.targetBitrate (Mbps)"] = outRtpStat.targetBitrate/1_000_000d;
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
            report.Metrics[$"{transceiverTag}.timestamp"] = stat.Value.Timestamp;
            report.Timestamp = DateTimeOffset.Now.ToString("o");

            OnStatsUpdated?.Invoke(player, report);
        }        
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