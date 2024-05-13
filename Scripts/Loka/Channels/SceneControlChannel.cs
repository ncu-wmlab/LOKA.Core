using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneControlChannel : LokaChannel
{
    // public bool IsHost = false; // use "local" instead
    [SerializeField] Camera _camera;

    protected override void OnMessageReceive(int tag, object msg)
    {
        // ignore if not host
        if(IsLocal)
            return;

        if(tag == 26000)
        {
            _camera.fieldOfView = (float)(double)msg;
            print($"FOV: {_camera.fieldOfView}");
        }
    }

    /* -------------------------------------------------------------------------- */

    public void SendFov(float fov)
    {
        Send(26000, fov);
    }

    
}