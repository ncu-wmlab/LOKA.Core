using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.UI;
using Unity.WebRTC;
using System.Text;
using System.Linq;

public class DemoStreamStatsUI : MonoBehaviour
{
    Text _text;
    [SerializeField] StreamSenderBase _sender;
    [SerializeField] StreamReceiverBase _receiver;

    RTCStatsReport _lastReport;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _text = GetComponent<Text>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        RTCStatsReport report;
        if(_sender != null)
            report = _sender.Transceivers.First().Value.Sender.GetStats().Value;
        else 
            report = _receiver.Transceiver.Receiver.GetStats().Value;
        _text.text = CreateDisplayString(report, _lastReport);
        _lastReport = report;
    }



    string CreateDisplayString(RTCStatsReport report, RTCStatsReport lastReport)
    {
        var builder = new StringBuilder();

        foreach (var stats in report.Stats.Values)
        {
            if (stats is RTCInboundRTPStreamStats inboundStats)
            {
                builder.AppendLine($"{inboundStats.kind} receiving stream stats");
                if (inboundStats.codecId != null && report.Get(inboundStats.codecId) is RTCCodecStats codecStats)
                {
                    builder.AppendLine($"Codec: {codecStats.mimeType}");
                    if (!string.IsNullOrEmpty(codecStats.sdpFmtpLine))
                    {
                        foreach (var fmtp in codecStats.sdpFmtpLine.Split(';'))
                        {
                            builder.AppendLine($" - {fmtp}");
                        }
                    }

                    if (codecStats.payloadType > 0)
                    {
                        builder.AppendLine($" - {nameof(codecStats.payloadType)}={codecStats.payloadType}");
                    }

                    if (codecStats.clockRate > 0)
                    {
                        builder.AppendLine($" - {nameof(codecStats.clockRate)}={codecStats.clockRate}");
                    }

                    if (codecStats.channels > 0)
                    {
                        builder.AppendLine($" - {nameof(codecStats.channels)}={codecStats.channels}");
                    }
                }

                if (inboundStats.kind == "video")
                {
                    builder.AppendLine($"Decoder: {inboundStats.decoderImplementation}");
                    builder.AppendLine($"Resolution: {inboundStats.frameWidth}x{inboundStats.frameHeight}");
                    builder.AppendLine($"Framerate: {inboundStats.framesPerSecond}");
                }

                if (lastReport.TryGetValue(inboundStats.Id, out var lastStats) &&
                    lastStats is RTCInboundRTPStreamStats lastInboundStats)
                {
                    var duration = (double)(inboundStats.Timestamp - lastInboundStats.Timestamp) / 1000000;
                    var bitrate = (8 * (inboundStats.bytesReceived - lastInboundStats.bytesReceived) / duration) / 1000;
                    builder.AppendLine($"Bitrate: {bitrate:F2} kbit/sec");
                }
            }
            else if (stats is RTCOutboundRTPStreamStats outboundStats)
            {
                builder.AppendLine($"{outboundStats.kind} sending stream stats");
                if (outboundStats.codecId != null && report.Get(outboundStats.codecId) is RTCCodecStats codecStats)
                {
                    builder.AppendLine($"Codec: {codecStats.mimeType}");
                    if (!string.IsNullOrEmpty(codecStats.sdpFmtpLine))
                    {
                        foreach (var fmtp in codecStats.sdpFmtpLine.Split(';'))
                        {
                            builder.AppendLine($" - {fmtp}");
                        }
                    }

                    if (codecStats.payloadType > 0)
                    {
                        builder.AppendLine($" - {nameof(codecStats.payloadType)}={codecStats.payloadType}");
                    }

                    if (codecStats.clockRate > 0)
                    {
                        builder.AppendLine($" - {nameof(codecStats.clockRate)}={codecStats.clockRate}");
                    }

                    if (codecStats.channels > 0)
                    {
                        builder.AppendLine($" - {nameof(codecStats.channels)}={codecStats.channels}");
                    }
                }

                if (outboundStats.kind == "video")
                {
                    builder.AppendLine($"Encoder: {outboundStats.encoderImplementation}");
                    builder.AppendLine($"Resolution: {outboundStats.frameWidth}x{outboundStats.frameHeight}");
                    builder.AppendLine($"Framerate: {outboundStats.framesPerSecond}");
                }

                if (lastReport.TryGetValue(outboundStats.Id, out var lastStats) &&
                    lastStats is RTCOutboundRTPStreamStats lastOutboundStats)
                {
                    var duration = (double)(outboundStats.Timestamp - lastOutboundStats.Timestamp) / 1000000;
                    var bitrate = (8 * (outboundStats.bytesSent - lastOutboundStats.bytesSent) / duration) / 1000;
                    builder.AppendLine($"Bitrate: {bitrate:F2} kbit/sec");
                }
            }
        }

        return builder.ToString();
    }

}