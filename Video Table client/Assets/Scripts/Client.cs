﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    #endregion

    public static event Action<string> ActionReceivedMessage;

    public string ipAddress = "127.0.0.1";
    public int port = 5000;

    private static Client _instance;


    // Use this for initialization 	
    void Start()
    {
        _instance = this;

        var setting = SettingController.Settings;

        ipAddress = setting.IpAddress; 
        port = setting.Port;

        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {
    }

    public static Client GetInstance()
    {
        return _instance;
    }

    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(() => ListenForData(_cancellationTokenSource.Token));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData(CancellationToken cancellationToken)
    {
        try
        {
            socketConnection = new TcpClient(ipAddress, port);
            Byte[] bytes = new Byte[1024];
            while (!cancellationToken.IsCancellationRequested)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);

                        ActionReceivedMessage?.Invoke(serverMessage);
                    }
                }
            }

            socketConnection = null;
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    public void Send(string message)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SEND ERROR]: {e.Message}");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        Debug.Log($"Focus: {hasFocus}");
        if (hasFocus)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                Send("off");
                Debug.Log("NOT Focus!");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                socketConnection = null;
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log($"Pause: {pauseStatus}");
        if (pauseStatus)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                Send("off");
                Debug.Log("Quit Pause");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                socketConnection = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            Send("off");
            Debug.Log("Quit");
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            socketConnection = null;
        }
    }

    ~Client(){
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            Send("off");
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            socketConnection = null;
        }
    }
}
