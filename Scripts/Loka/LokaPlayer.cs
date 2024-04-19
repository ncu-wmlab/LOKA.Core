using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.InputSystem;
using System.Linq;

/// <summary>
/// Host player. AKA. client controlled player in the host machine.
/// </summary>
[RequireComponent(typeof(InputReceiver))]
public class LokaPlayer : MonoBehaviour
{
    public string ConnectionId { get; private set; }
    public SceneControlChannel SceneControlChannel;
    public LabDeviceChannel LabDeviceChannel;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // GetComponentsInChildren<DataChannelBase>().ToList().ForEach(c => c.local = false);
    }

    /* -------------------------------------------------------------------------- */

    public void Init(string connectionId)
    {
        ConnectionId = connectionId;
        name = "LokaPlayer "+connectionId;
    }
}