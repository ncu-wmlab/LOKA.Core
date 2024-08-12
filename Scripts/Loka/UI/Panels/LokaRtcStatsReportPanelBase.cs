using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine.UI;
using TMPro;
using System;
using Newtonsoft.Json;
using LabFrame2023;

public class LokaRtcStatsReportPanelBase : BaseDictPanel
{
    /// <summary>
    /// If true, will also save record to file when Stats updated
    /// </summary>
    protected bool _isRecording = false;
    protected const string HIGHLIGHT_METRIC_KEY = "<color=green>Highlight Metrics :)</color>";
    

    /// <summary>
    /// Refresh stats to BaseDictPanel record
    /// </summary>
    /// <param name="getStatsTask">no need to await first</param>
    /// <param name="name">transceiver tag (Display on panel)</param>
    /// <returns></returns>
    protected IEnumerator GetStatsAsync(RTCStatsReportAsyncOperation getStatsTask, string name)
    {
        yield return getStatsTask;
        if(getStatsTask.IsError)
        {
            Debug.LogWarning($"[StatsPanel] Read Stats Error: {getStatsTask.Error.message}");
            yield break;
        }


        foreach(var stat in getStatsTask.Value.Stats)
        {
            RTCStatsType type = stat.Value.Type;
            long timeStamp = stat.Value.Timestamp;
            string categoryName = $"{name}.{type}";

            // Special stats (We set a _stats[HIGHLIGH_METRIC_KEY] for this purpose)
            _SetMetric(HIGHLIGHT_METRIC_KEY, "", "");
            if(type == RTCStatsType.InboundRtp)
            {
                var inboundRtpStat = (RTCInboundRTPStreamStats)stat.Value;
                // bitrate
                ulong bytesReceived = inboundRtpStat.bytesReceived;
                ulong lastBytesReceived = (ulong)_GetMetric(categoryName, "bytesReceived", 0UL);
                long lastTimestamp = (long)_GetMetric(categoryName, "timestamp", 0L);
                double timeDelta = (timeStamp - lastTimestamp)/1_000_000d; // μs to s   
                ulong bytesDelta = bytesReceived - lastBytesReceived;
                var bitrate = bytesDelta * 8 / timeDelta;
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.bitrate (kbps)", bitrate/1_000d);

                //
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.received (MB)", inboundRtpStat.bytesReceived/1_000_000d);
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.jitter (ms)", inboundRtpStat.jitter*1000d);
            }
            else if(type == RTCStatsType.OutboundRtp)
            {
                var outRtpStat = (RTCOutboundRTPStreamStats)stat.Value;

                //
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.targetBitrate (kbps)", outRtpStat.targetBitrate/1_000d);
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.sent (MB)", outRtpStat.bytesSent/1_000_000d);
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.packetSent", outRtpStat.packetsSent);
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.framePerSeconds", outRtpStat.framesPerSecond);
            }
            else if(type == RTCStatsType.RemoteInboundRtp)
            {
                var remoteInRtpStat = (RTCRemoteInboundRtpStreamStats)stat.Value;
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.packetsLost", remoteInRtpStat.packetsLost);
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.roundTripTime (ms)", remoteInRtpStat.roundTripTime*1000d); // s to ms
                _SetMetric(HIGHLIGHT_METRIC_KEY, $"{name}.jitter (ms)", remoteInRtpStat.jitter*1000d); 
            }
            
            // save all stats
            _SetMetric(categoryName, $"timestamp", stat.Value.Timestamp);
            foreach(var pair in stat.Value.Dict)
            {                
                _SetMetric(categoryName, $"{pair.Key}", pair.Value);
                // Debug.Log($"[Stats] {name} {pair.Key}: {pair.Value}");
            }

            // record
            if(_isRecording)
            {
                // print($"[Stats] {name}.{type} {timeStamp} | "+stat.Value.ToJson());
                RecordStats();
            }            
        }        
    }    

    private void RecordStats()
    {
        var data = new LokaStatsData{stats = _dict};
        LabDataManager.Instance.WriteData(data);
    }


    public class LokaStatsData : LabDataBase
    {
        public Dictionary<string, Dictionary<string, object>> stats;
    }
}