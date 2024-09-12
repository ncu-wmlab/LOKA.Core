using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// SERVER SIDE (HOST)
// 對於 Host 來說，每個 player 都有一個 LabDeviceChannel
public partial class LabDeviceChannel : LokaChannel
{
    void HostStart()
    {

    }

    /// <summary>
    /// HOST on receive: 儲存下來
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="msg"></param>
    private void HostReceiveMessage(int tag, object msg)
    {
        _datas[(LabDeviceSignal)tag] = msg;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 傳 Command 給 Client
    /// </summary>
    /// <param name="op"></param>
    /// <param name="data"></param>
    public void SendRequest(LabDeviceCommand op, object data)
    {
        Send((int)op, data);
    }
}