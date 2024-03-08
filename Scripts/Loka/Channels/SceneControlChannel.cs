using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneControlChannel : LokaChannel
{
    public bool IsHost = false;

    protected override void MessageReceived(string tag, object msg)
    {
        if(tag == "FOV")
        {
            Camera.main.fieldOfView = (float)msg;
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