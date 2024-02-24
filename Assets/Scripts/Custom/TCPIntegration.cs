using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class TCPIntegration : MonoBehaviour
{
    public string host = "10.0.1.30";
    public int port = 80;
    private TcpClient tcpClient;
    private StreamReader reader;
    private bool attemptingConnection = false;
    public delegate void MessageReceivedHandler(string message);
    public event MessageReceivedHandler OnMessageReceived;

    async void Start()
    {
        await ConnectToServer();
    }

    async Task ConnectToServer()
    {
        if (attemptingConnection) return;

        attemptingConnection = true;
        try
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(host, port);
            reader = new StreamReader(tcpClient.GetStream());
            Debug.Log("Connected to the TCP server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to the TCP server: " + e.Message);
        }
        finally
        {
            attemptingConnection = false;
        }
    }

    // Modify the Update method to invoke the event
        void Update()
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                if (tcpClient.Available > 0)
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.Log(message);
                        OnMessageReceived?.Invoke(message); // Invoke the event
                    }
                }
            }
            else if (!attemptingConnection)
            {
                Debug.LogWarning("TCP client disconnected. Attempting to reconnect...");
                Start();
            }
        }

    void OnApplicationQuit()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }
}
