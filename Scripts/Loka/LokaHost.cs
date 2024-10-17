using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using System.Linq;
using UnityEngine.Events;
using System;

/// <summary>
/// LOKA Host Signal Handler
/// (Receives Signals from signal server)
/// </summary>
[RequireComponent(typeof(SignalingManager))]
public class LokaHost : SignalingHandlerBase, 
    IOfferHandler, 
    IAddReceiverHandler, IAddChannelHandler, 
    IDisconnectHandler, IDeletedConnectionHandler 
{
    protected static LokaHost _instance = null;
    public static LokaHost Instance => _instance;

    /* -------------------------------------------------------------------------- */

    [Header("Prefabs / GameObjects")]
    [SerializeField] 
    [Tooltip("The prefab to instantiate for each player")]
    LokaPlayer _playerPrefab;

    [SerializeField]
    [Tooltip("The spawn point for each player")]
    Transform _spawnPoint;



    [Header("Configs")]
    [SerializeField] int _targetFrameRate = 120;
    
    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 連線 ID 列表
    /// </summary>
    List<string> _connectionIds = new List<string>();
    
    /// <summary>
    /// 連線 ID 對應的 GameObject (LokaPlayer)
    /// </summary>
    Dictionary<string, LokaPlayer> _connectedPlayers = new Dictionary<string, LokaPlayer>();

    
    /// <summary>
    /// 連線 ID 列表
    /// </summary>
    public List<string> ConnectionIds => _connectionIds;

    /// <summary>
    /// 連線 ID 對應的 GameObject (LokaPlayer)
    /// </summary>
    public Dictionary<string, LokaPlayer> ConnectedPlayers => _connectedPlayers;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 玩家加入事件。會在玩家物件建立完成後觸發
    /// </summary>
    public event UnityAction<LokaPlayer> OnPlayerJoin;
    /// <summary>
    /// 玩家離開事件。會在玩家物件刪除前觸發
    /// </summary>
    public event UnityAction<LokaPlayer> OnPlayerExit;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(_instance) 
        {
            Debug.LogWarningFormat("Only one instance of LokaHost is allowed! Remove dulpicated instance: {0}", gameObject.name);
            Destroy(gameObject);
            return;    
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);        
        GetComponent<SignalingManager>().Run(null, new SignalingHandlerBase[]{this});

        Application.targetFrameRate = _targetFrameRate;        
    }

    /* -------------------------------------------------------------------------- */

    public void OnOffer(SignalingEventData eventData)
    {
        SendAnswer(eventData.connectionId);
        if(_connectionIds.Contains(eventData.connectionId))
        {
            Debug.LogWarning("[LokaHost] Connection already exists: " + eventData.connectionId);
            return;
        }
        print($"[LokaHost] Accepted Offer {eventData.connectionId}");
        _connectionIds.Add(eventData.connectionId);

        // Add Player to the scene
        var newPlayer = Instantiate(_playerPrefab);
        newPlayer.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
        newPlayer.Init(eventData.connectionId);  // Init player
        _connectedPlayers.Add(eventData.connectionId, newPlayer);

        // Register Senders/Channels
        var sender = newPlayer.GetComponentsInChildren<StreamSenderBase>();  // webrtc tracks
        foreach(var s in sender)
        {
            AddSender(eventData.connectionId, s);
        }
        var channels = newPlayer.GetComponentsInChildren<IDataChannel>();
        foreach(var channel in channels)
        {
            AddChannel(eventData.connectionId, channel);            
        }

        newPlayer.LateInit();
        // SendAnswer(eventData.connectionId);
        OnPlayerJoin?.Invoke(newPlayer);
    }

    public void OnAddReceiver(SignalingEventData eventData)
    {
        // Register Receivers
        var player = _connectedPlayers[eventData.connectionId];
        var track = eventData.transceiver.Receiver.Track;
        StreamReceiverBase receiver;
        if(track.Kind == Unity.WebRTC.TrackKind.Video)
        {
            receiver = player.GetComponentsInChildren<VideoStreamReceiver>().FirstOrDefault();
        }
        else if(track.Kind == Unity.WebRTC.TrackKind.Audio)
        {
            receiver = player.GetComponentsInChildren<AudioStreamReceiver>().FirstOrDefault();
        }
        else 
            throw new NotSupportedException("[LokaHost] Unsupported track kind: " + track.Kind);
        
        Debug.Log($"[LokaHost] Add Track Kind={track.Kind} Id={eventData.connectionId} ");
        SetReceiver(eventData.connectionId, receiver, eventData.transceiver);       
        if(track.Kind == Unity.WebRTC.TrackKind.Audio)
        {
            ((AudioStreamReceiver)receiver).targetAudioSource.loop = true;
            ((AudioStreamReceiver)receiver).targetAudioSource.Play();
        } 
    }

    public void OnAddChannel(SignalingEventData eventData)
    {
        var player = _connectedPlayers[eventData.connectionId];
        var channel = player.GetComponentsInChildren<IDataChannel>().FirstOrDefault(c => !c.IsLocal && !c.IsConnected);
        channel?.SetChannel(eventData);
        print($"[LokaHost] Add channel Label={channel?.Label} Id={eventData.connectionId}");
    }

    public void OnDeletedConnection(SignalingEventData eventData)
    {
        RemoveConnection(eventData.connectionId);
    }

    public void OnDisconnect(SignalingEventData eventData)
    {
        RemoveConnection(eventData.connectionId);
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// When a Client Disconnect (Gracefully or Unexpectedly)
    /// </summary>
    /// <param name="connectionId"></param>
    void RemoveConnection(string connectionId) 
    {
        if (!_connectionIds.Contains(connectionId))
        {
           Debug.LogWarning("[LokaSignalHandler] Unable to remove connection that is not exist: " + connectionId);
           return;
        }
        _connectionIds.Remove(connectionId);        

        // Remove Player from the scene
        Debug.Log("[LokaSignalHandler] Remove Connection: " + connectionId);

        var player = _connectedPlayers[connectionId];      
        _connectedPlayers.Remove(connectionId);  
        try
        {
            OnPlayerExit?.Invoke(player);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        // foreach(var sender in player.GetComponentsInChildren<StreamSenderBase>())
        // {
        //     RemoveSender(connectionId, sender);
        // }
        // foreach(var receiver in player.GetComponentsInChildren<StreamReceiverBase>())
        // {
        //     RemoveReceiver(connectionId, receiver);
        // }
        // foreach(var channel in player.GetComponentsInChildren<IDataChannel>())
        // {
        //     RemoveChannel(connectionId, channel);
        // }      

        Destroy(player.gameObject);
    }    
}