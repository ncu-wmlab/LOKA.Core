using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.WebRTC;
using System.Linq;

public class LokaRtcStatsReportPanel : LokaRtcStatsReportPanelBase, ILokaHostUISubpanel
{
    RTCRtpSender _videoSender;
    RTCRtpSender _audioSender;


    public void OnShow(LokaPlayer player)
    {        
        try
        {
            _videoSender = player.VideoSender.Transceivers.First().Value.Sender;
            _audioSender = player.AudioSender.Transceivers.First().Value.Sender;
        }
        catch
        {
            Debug.LogError("[LokaRtcStatsReportPanel] Fail to get Video or Audio Sender Transceiver");
        }
        StartCoroutine(UpdateCoroutine());
    }

    public void OnHide()
    {
        _videoSender = null;
        _audioSender = null;
        StopAllCoroutines();
    }    


    IEnumerator UpdateCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            if(!enabled)
                continue;
            
            if(_videoSender != null)
                yield return GetStatsAsync(_videoSender.GetStats(), "Video Out");
            
            if(_audioSender != null)
                yield return GetStatsAsync(_audioSender.GetStats(), "Audio Out");
            
            _SetMetric("Video Out", "IsReady", _videoSender != null);
            _SetMetric("Audio Out", "IsReady", _audioSender != null);
            _SetMetric("Timestamp",  "Test", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
        }
    }
}