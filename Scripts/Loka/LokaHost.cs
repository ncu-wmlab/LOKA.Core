using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;

[RequireComponent(typeof(LokaSignalHandler))]
[RequireComponent(typeof(SignalingManager))]
public class LokaHost : MonoBehaviour
{
    public bool StartSignalServerAtStart = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(StartSignalServerAtStart)
        {
            // FIXME  StartSignalServerAtStart
            Debug.LogWarning("StartSignalServerAtStart is not supported yet. Doing nothing.");
        }        
    }
}