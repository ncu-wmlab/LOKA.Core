using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// SERVER SIDE (HOST)
// 對於 HOst 來說，每個 player 都有一個 LabDeviceChannel
public partial class LabDeviceChannel : LokaChannel
{
    void HostStart()
    {

    }

    private void HostReceiveMessage(int tag, object msg)
    {
        _datas[(LabDeviceControl)tag] = msg;
    }

    /* -------------------------------------------------------------------------- */

    public void SendRequest(LabDeviceCommand op, object data)
    {
        Send((int)op, data);
    }
}