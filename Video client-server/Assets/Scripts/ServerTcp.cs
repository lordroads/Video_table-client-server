using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ServerTcp : MonoBehaviour
{
    #region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient; 	
	
	private  CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    #endregion

    public static event Action<string> ActionReceivedMessage;

    public string ipAddress = "127.0.0.1";
	public int port = 5000;

    private static ServerTcp _instance;

    void Awake()
    {
        _instance = this;
    }

    public static ServerTcp GetInstance()
    {
        return _instance;
    }

    void Start () {
		ipAddress = SettingController.Settings.IpAddress;
		port = SettingController.Settings.Port;

		tcpListenerThread = new Thread (() => ListenForIncommingRequests(_cancellationTokenSource.Token)); 		
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Name = "SERVER_TCP_APP";
		tcpListenerThread.Start(); 	
	}  	
	
	void Update () { 		
		if (Input.GetKeyDown(KeyCode.Space)) {   
			Send("This is a message from your server.");         
		}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }  	
	
	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	private void ListenForIncommingRequests (CancellationToken cancellationToken) { 		
		try { 						
			tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");
			Byte[] bytes = new Byte[1024];  	
			while (!cancellationToken.IsCancellationRequested) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 										
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							string clientMessage = Encoding.ASCII.GetString(incommingData);
							//Debug.Log("client message received as: " + clientMessage); 

							ActionReceivedMessage?.Invoke(clientMessage);

                            if (clientMessage == "off")
                            {
                                connectedTcpClient = null;
                                break;
                            }
                        } 					
					} 				
				} 
				Debug.Log("Disconnect client!");
            }
            Debug.Log("Server - not listened!");
        } 		
		catch (SocketException socketException) {
            Debug.Log("SocketException " + socketException.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"[LISTNER]: {e.Message}");
        }
    }  	
	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	public void Send(string message) { 		
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {                 			
				// Convert string message to byte array.                 
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message); 				
				// Write byte array to socketConnection stream.               
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);               
				Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
		catch(Exception e) {
			Debug.LogError($"[SEND]: {e.Message}");
			connectedTcpClient = null;
		}
	}

    void OnApplicationFocus(bool hasFocus)
    {
        Debug.Log($"Focus: {hasFocus}");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log($"Pause: {pauseStatus}");
    }

    void OnApplicationQuit()
	{
		Debug.Log("Quit");
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
