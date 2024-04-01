using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using System.Linq;

/// <summary>
/// LOKA Host Signal Handler
/// (Receives Signals from signal server)
/// </summary>
[RequireComponent(typeof(SignalingManager))]
public class LokaSignalHandler : SignalingHandlerBase, IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler
{
    [Header("Prefabs")]
    [SerializeField] GameObject _playerPrefab;

    /// <summary>
    /// 連線 ID 列表
    /// </summary>
    List<string> _connectionIds = new List<string>();
    /// <summary>
    /// 連線 ID 對應的 GameObject
    /// </summary>
    Dictionary<string, GameObject> _connectedPlayers = new Dictionary<string, GameObject>();


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
        newPlayer.name = "LokaPlayer "+eventData.connectionId;
        _connectedPlayers.Add(eventData.connectionId, newPlayer);

        // Register Sender
        var sender = newPlayer.GetComponentsInChildren<StreamSenderBase>();
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

        SendAnswer(eventData.connectionId);
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
           Debug.LogWarning("[LokaSignalHandler] Uable to remove connection that is not exist: " + connectionId);
           return;
        }
        _connectionIds.Remove(connectionId);

        // Remove Player from the scene
        var player = _connectedPlayers[connectionId];
        Destroy(player);
        _connectedPlayers.Remove(connectionId);
        _connectionIds.Remove(connectionId);
        Debug.Log("[LokaSignalHandler] Removed Connection: " + connectionId);
    }
}