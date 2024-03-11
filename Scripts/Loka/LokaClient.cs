using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

/// <summary>
/// LOKA Client Signal Handler
/// </summary>
public class LokaClient : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private RawImage _overlayRawImage;
    [SerializeField] private AudioSource _audioSource;

    [Header("Channels")]
    [SerializeField] private VideoStreamReceiver _videoStreamReceiver;
    [SerializeField] private AudioStreamReceiver _audioStreamReceiver;
    [SerializeField] private InputSender _inputSender;
    [Header("Managers")]
    [SerializeField] private SignalingManager _signalingManager;
    [SerializeField] private SingleConnection _singleConnection;

    string _connectionId;
    Vector2 _lastRawImageSize;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        // RenderStreamingSettings ursSettings = ;
        _overlayRawImage.enabled = false;
        _audioStreamReceiver.targetAudioSource = _audioSource;

        // register get signal events
        _videoStreamReceiver.OnUpdateReceiveTexture += (texture) => {            
            _overlayRawImage.texture = texture;
            print("[Video] Receive stream RES: "+texture?.width + "x" + texture?.height);
        };
        _videoStreamReceiver.OnStartedStream += (s) => {
            _overlayRawImage.enabled = true;
            print("[Video] Started Stream "+s);
        };
        _videoStreamReceiver.OnStoppedStream += (s) => {
            _overlayRawImage.enabled = false;
            print("[Video] Stopped Stream "+s);
        };
        _audioStreamReceiver.OnUpdateReceiveAudioSource += (audio) => {
            audio.loop = true;
            audio.Play();
        };
        _inputSender.OnStartedChannel += (id) => {
            Debug.Log("[InputSender] Started Channel: " + id);
        };
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        var size = _overlayRawImage.rectTransform.sizeDelta;
        if (_lastRawImageSize == size)
            return;
        _lastRawImageSize = size;
        CalculateInputRegion();
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Recalculate Input Region
    /// </summary>
    void CalculateInputRegion()
    {
        // check if not ready
        if(_inputSender == null || !_inputSender.IsConnected)
            return;

        var corners_wp = new Vector3[4];
        _overlayRawImage.rectTransform.GetWorldCorners(corners_wp);
        var cornerBL_sp = RectTransformUtility.WorldToScreenPoint(_overlayRawImage.canvas.worldCamera, corners_wp[0]);
        var cornerTR_sp = RectTransformUtility.WorldToScreenPoint(_overlayRawImage.canvas.worldCamera, corners_wp[2]);        
        var w = cornerTR_sp.x - cornerBL_sp.x;
        var h = cornerTR_sp.y - cornerBL_sp.y;

        var region = new Rect(cornerBL_sp.x, cornerBL_sp.y, w, h);
        // var size = new Vector2Int(w, h);
        var size = new Vector2Int(_overlayRawImage.texture.width, _overlayRawImage.texture.height);

        _inputSender.CalculateInputResion(region, size);
        _inputSender.EnableInputPositionCorrection(true);        
    }

    /* -------------------------------------------------------------------------- */

    public void StartStream(string connectionId = null, RenderStreamingSettings settings = null)
    {
        if(string.IsNullOrWhiteSpace(connectionId))
        {
            connectionId = "LokaConnection-"+Guid.NewGuid().ToString();
        }        

        // TODO stream config
        // _videoStreamReceiver.SetCodec()       

        if(settings != null) 
        {
            // GetComponent<SignalingManager>().Run(settings, new SignalingHandlerBase[]{});；
        }
        // GetComponent<SignalingManager>().Run();

        _connectionId = connectionId;
        _singleConnection.CreateConnection(_connectionId);
    }

    public void StopStream()
    {
        _singleConnection.DeleteConnection(_connectionId);
        _connectionId = null;
    }


}