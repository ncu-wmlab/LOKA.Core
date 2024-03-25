using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.Events;
using Newtonsoft.Json;

/// <summary>
/// Sending Chenneled LOKA messages. <br /><br />
/// Host can override <c>MessageReceived</c> method to receive message from client. <br />
/// Client can use <c>SendMessage</c> method to send message to host.
/// </summary>
public abstract class LokaChannel : DataChannelBase
{
    protected override void OnOpen(string connectionId)
    {
        print("LokaChannel OnOpen "+ConnectionId);
        base.OnOpen(connectionId);
    }

    protected override void OnClose(string connectionId)
    {
        print("LokaChannel OnClose "+ConnectionId);
        base.OnClose(connectionId);
    }

    protected override void OnMessage(byte[] bytes)
    {
        base.OnMessage(bytes);
        string msgStr = System.Text.Encoding.UTF8.GetString(bytes);
        Message msg = JsonConvert.DeserializeObject<Message>(msgStr);
        if(msg == null || msg.T == null) 
            return;
        Debug.Log($"LokaChannel RECV [{msg.T}] {msg.M}");
        MessageReceived(msg.T, msg.M);
    }

    /// <summary>
    /// Message Received from Client
    /// </summary>
    /// <param name="tag">The name of the message (e.g. HP, CONTROL_CODE) <i>(盡量規範為大寫)</i></param>
    /// <param name="msg">The body of the message (e.g. 10000, "3952") <b>注意 float 會被轉換為 double</b></param>
    protected abstract void MessageReceived(string tag, object msg);

    /// <summary>
    /// Send Message to Host 
    /// </summary>
    /// <param name="msg"></param>
    public new void SendMessage(string tag, object msg)
    {
        var m = JsonConvert.SerializeObject(new Message{ T = tag, M = msg });
        Debug.Log($"LokaChannel SEND [{tag}] {msg}");
        Send(m);
    }

    public class Message
    {
        public string T; // tag/event of message
        public object M; // data/payload of message
    }
}