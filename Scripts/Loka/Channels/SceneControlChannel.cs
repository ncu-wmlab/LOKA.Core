using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.tvOS;


public class SceneControlChannel : LokaChannel
{
    // public bool IsHost = false; // use "local" instead
    [SerializeField] LokaPlayer _player;
    [SerializeField] Camera _camera;

    public enum SceneControlChannelTag
    {
        FOV = 26000,
        WIDTH = 26001,
        HEIGHT = 26002,
    }

    protected override void OnMessageReceive(int tag, object msg)
    {
        // ignore if not host
        if(IsLocal)
            return;

        SceneControlChannelTag stag = (SceneControlChannelTag)tag;
        if(stag == SceneControlChannelTag.FOV)
        {
            // print($"FOV: {_camera.fieldOfView}");
            _camera.fieldOfView = (float)(double)msg;
        }
        else  if(stag == SceneControlChannelTag.WIDTH)
        {
            // print($"WIDTH: {Screen.width}");
            // _player.VideoStreamSender.texture.width = (int)msg;
        }
        else  if(stag == SceneControlChannelTag.HEIGHT)
        {
            // print($"HEIGHT: {Screen.height}");
            // _player.VideoStreamSender.texture.height = (int)msg;
        }
    }

    /* -------------------------------------------------------------------------- */

    public void SendFov(float fov)
    {
        Send(26000, fov);
    }

    public void SendWidthAndHeight(int width, int height)
    {
        Send(26001, width);
        Send(26002, height);
    }

}