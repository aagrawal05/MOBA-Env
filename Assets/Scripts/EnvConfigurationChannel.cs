using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using System.Text;
using System;
using System.Collections.Generic;

public class EnvConfigurationChannel : SideChannel
{
    public MOBASettings m_MOBASettings;
    public MOBAEnvController m_MOBAEnvController;

    public EnvConfigurationChannel()
    {
        ChannelId = new Guid("3f07928c-2b0e-494a-810b-5f0bbb7aaeca");
    }

    protected override void OnMessageReceived(IncomingMessage msg)
    {   
        // DO_NOTHING
    }

    public void SendDebugStatementToPython(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            var stringToSend = type.ToString() + ": " + logString + "\n" + stackTrace;
            using (var msgOut = new OutgoingMessage())
            {
                msgOut.WriteString(stringToSend);
                QueueMessageToSend(msgOut);
            }
        }
    }
}
