using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneControlChannel : LokaChannel
{
    // public bool IsHost = false; // use "local" instead
    [SerializeField] Camera _camera;

    protected override void MessageReceived(string tag, object msg)
    {
        if(tag == "FOV")
        {
            _camera.fieldOfView = (float)(double)msg;
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    /* -------------------------------------------------------------------------- */

    public void SendFov(float fov)
    {
        SendMessage("FOV", fov);
    }
}