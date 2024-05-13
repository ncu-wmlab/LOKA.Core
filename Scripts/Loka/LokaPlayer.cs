using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

/// <summary>
/// Host player. AKA. client controlled player in the host machine.
/// </summary>
[RequireComponent(typeof(InputReceiver))]
public class LokaPlayer : MonoBehaviour
{
    // [Header("Runtime")]    
    public string ConnectionId { get; private set; }
    
    public SceneControlChannel SceneControlChannel { get; private set; }
    public LabDeviceChannel LabDeviceChannel { get; private set; }

    public InputReceiver InputReceiver { get; private set; }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// (Called by LokaHost) On Connection Init, DataChannels not established
    /// </summary>
    /// <param name="connectionId"></param>
    public virtual void Init(string connectionId)
    {
        ConnectionId = connectionId;
        name = "LokaPlayer ID=" + connectionId;
        SceneControlChannel = GetComponentInChildren<SceneControlChannel>();
        LabDeviceChannel = GetComponentInChildren<LabDeviceChannel>();
    }

    /// <summary>
    /// (Called by LokaHost) On Connection Start, DataChannels established
    /// </summary>
    public virtual void LateInit()
    {
        InputReceiver = GetComponent<InputReceiver>();
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
#if UNITY_EDITOR
        // FIXME print all input devices and their controls: Remove on production!
        string s = "";
        InputReceiver?.devices.ToList().ForEach(dev =>
        {
            s += $"\n<b>{dev.name}</b> ";
            if(dev.usages.Count > 0)
            {
                s += $"({dev.usages[0]}) ";
            }
            s +=  $"<i>{dev.GetType()}</i>\n";
            if (dev is XRHandDevice)
            {
                dev.allControls.ToList().ForEach(y => s += $"- {y.path}: {y.ReadValueAsObject()}\n");
            }
        });

        if(s.Length > 1)
            print(s.Remove(0,1));
#endif
    }
}