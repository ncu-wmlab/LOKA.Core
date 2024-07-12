using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Serialization;
using System.Linq;

/// <summary>
/// Sending Chenneled LOKA messages. <br /><br />
/// </summary>
/// <remarks>
/// Override <c>OnMessageReceive</c> method to receive message. <br />
/// Use <c>Send</c> method to send message. <br />
/// <c>IsLocal</c> is a flag to determine if the channel is the client or the host. <br />
/// </remarks>
public abstract class LokaChannel : DataChannelBase
{
    protected override void OnOpen(string connectionId)
    {
        // print("LokaChannel OnOpen "+ConnectionId);
        base.OnOpen(connectionId);
    }

    protected override void OnClose(string connectionId)
    {
        // print("LokaChannel OnClose "+ConnectionId);
        base.OnClose(connectionId);
    }

    protected override void OnMessage(byte[] bytes)
    {
        base.OnMessage(bytes);        
        string msgStr = System.Text.Encoding.UTF8.GetString(bytes);
        // Debug.Log($"[{GetType()}] <color=#1abc9c>RECVRAW</color>  (Len={bytes.Length}) ({msgStr.Length}) {msgStr} ");  
        Message msg = JsonConvert.DeserializeObject<Message>(msgStr);
        // if(msg == null || msg.T == null) 
        //     return;
        // Debug.Log($"[{GetType()}] <color=#1abc9c>RECV</color>  {msg.T}: {msg.M} ({bytes.Length} Bytes)");  // FIXME Logging should be removed in production
        OnMessageReceive(msg.T, msg.M);
    }

    /// <summary>
    /// Message Received from the other side 
    /// </summary>
    /// <param name="tag">The key of the message (e.g. HP=1, SP=2) <i>(推薦使用 enum 確保不出現 magic number)</i></param>
    /// <param name="msg">The body of the message (e.g. 10000, "3952") <b>注意 float 會被轉換為 double；並且若你傳的是物件，要可以被序列化</b></param>
    protected abstract void OnMessageReceive(int tag, object msg);

    /// <summary>
    /// Send Message to the other side (host / client)
    /// </summary>
    /// <param name="tag">The key of the message (e.g. HP=1, SP=2) <i>(推薦使用 enum 確保不出現 magic number)</i></param>
    /// <param name="msg">The body of the message (e.g. 10000, "3952") <b>注意 float 會被轉換為 double；並且若你傳的是物件，要可以被序列化</b></param>
    public void Send(int tag, object msg)
    {
        // FIXME 或許我們不該使用 json 來做這些事，之後再來改 先求有再求好
        // 考慮過使用 MemoryPack，但會有 Unity 版本問題
        var m = JsonConvert.SerializeObject(
            new Message{ T = tag, M = msg }, 
            new JsonSerializerSettings { 
                NullValueHandling = NullValueHandling.Ignore,  
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                FloatParseHandling = FloatParseHandling.Decimal,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new WritablePropertiesOnlyResolver(),
            });
        Debug.Log($"[{GetType()}] <color=#e67e22>SEND</color> {tag}: {msg}");  // FIXME Logging should be removed in production
        // Debug.Log($"[{GetType()}] <color=#e67e22>SENDRAW</color> (Len={m.Length}) {m}");  
        Send(m);
    }

    [Serializable]
    public class Message
    {
        public int T; // tag/event of message
        public object M; // data/payload of message
    }
}


// json.net ignore readonly properties (only getter)
class WritablePropertiesOnlyResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
        return props.Where(p => p.Writable).ToList();
    }
}