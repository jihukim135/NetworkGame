using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MySQLDemo : MonoBehaviour
{
    [SerializeField] private InputField name;
    [SerializeField] private InputField score;
    private byte[] receivedBytes = new byte[1024];

    public void OnClickAddRank()
    {
        if (name.text == string.Empty || score.text == string.Empty)
        {
            return;
        }

        using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            byte[] buffer = Encoding.UTF8.GetBytes($"{name.text},{score.text}");
        
            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Loopback, 10200);
            clientSocket.SendTo(buffer, serverEndPoint);
            
            int numberReceived = clientSocket.ReceiveFrom(receivedBytes, ref serverEndPoint);
            string text = Encoding.UTF8.GetString(receivedBytes, 0, numberReceived);
        
            Debug.Log(text);
        }

        name.text = string.Empty;
        score.text = string.Empty;
    }
}