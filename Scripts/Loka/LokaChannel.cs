using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.Events;

/// <summary>
/// Additional channel sending LOKA messages
/// </summary>
public class LokaChannel : DataChannelBase
{
    // TODO

    /// <summary>
    /// Event when message received (TAG, MESSAGE)
    /// </summary>
    public UnityEvent<string, string> OnMessageReceived = new UnityEvent<string, string>();

    protected override void OnOpen(string connectionId)
    {
        base.OnOpen(connectionId);
    }

    protected override void OnClose(string connectionId)
    {
        base.OnClose(connectionId);
    }

    protected override void OnMessage(byte[] bytes)
    {
        base.OnMessage(bytes);
        string msgStr = System.Text.Encoding.UTF8.GetString(bytes);
        Message msg = JsonUtility.FromJson<Message>(msgStr);
        if(msg == null || msg.T == null) 
            return;
        Debug.Log("LokaChannel [" + msg.T + "] " + msg.M);
        OnMessageReceived?.Invoke(msg.T, msg.M.ToString());
    }    

    /// <summary>
    /// Send Message to Host 
    /// </summary>
    /// <param name="msg"></param>
    public new void SendMessage(string tag, object msg)
    {
        Send(JsonUtility.ToJson(new Message{ T = tag, M = msg }));
    }

    public class Message
    {
        public string T; // tag/event of message
        public object M; // data/payload of message
    }
}