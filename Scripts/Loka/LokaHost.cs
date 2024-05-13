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
public class LokaHost : SignalingHandlerBase, IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler
{
    [Header("Prefabs")]
    [SerializeField] LokaPlayer _playerPrefab;

    /// <summary>
    /// 連線 ID 列表
    /// </summary>
    List<string> _connectionIds = new List<string>();
    
    /// <summary>
    /// 連線 ID 對應的 GameObject (LokaPlayer)
    /// </summary>
    Dictionary<string, LokaPlayer> _connectedPlayers = new Dictionary<string, LokaPlayer>();

    public Dictionary<string, LokaPlayer> ConnectedPlayers => _connectedPlayers;

    /* -------------------------------------------------------------------------- */
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
        GetComponent<SignalingManager>().Run(null, new SignalingHandlerBase[]{this});
    }

    public void OnOffer(SignalingEventData eventData)
    {
        if(_connectionIds.Contains(eventData.connectionId))
        {
            Debug.LogWarning("[LokaSignalHandler] Connection already exists: " + eventData.connectionId);
            return;
        }
        _connectionIds.Add(eventData.connectionId);

        // Add Player to the scene
        var newPlayer = Instantiate(_playerPrefab);
        newPlayer.Init(eventData.connectionId);  // Init player
        _connectedPlayers.Add(eventData.connectionId, newPlayer);

        // Register Senders
        var sender = newPlayer.GetComponentsInChildren<StreamSenderBase>();  // webrtc tracks
        foreach(var s in sender)
        {
            AddSender(eventData.connectionId, s);
        }

        // Register Channels
        var channels = newPlayer.GetComponentsInChildren<IDataChannel>();
        foreach(var channel in channels)
        {
            AddChannel(eventData.connectionId, channel);            
        }

        newPlayer.LateInit();

        SendAnswer(eventData.connectionId);
        OnPlayerJoin?.Invoke(newPlayer);
    }

    public void OnAddChannel(SignalingEventData eventData)
    {
        print($"[LokaSignalHandler] Add channel {eventData.connectionId}");
        var go = _connectedPlayers[eventData.connectionId];
        var channels = go.GetComponentsInChildren<IDataChannel>();
        var channel = channels.FirstOrDefault(c => !c.IsLocal && !c.IsConnected);
        channel?.SetChannel(eventData);
    }

    public void OnDeletedConnection(SignalingEventData eventData)
    {
        RemoveConnection(eventData.connectionId);
    }

    public void OnDisconnect(SignalingEventData eventData)
    {
        RemoveConnection(eventData.connectionId);
    }

    /// <summary>
    /// When Guest Disconnect
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
        _connectionIds.Remove(connectionId);
        try
        {
            OnPlayerExit?.Invoke(player);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        Destroy(player.gameObject);
    }
}