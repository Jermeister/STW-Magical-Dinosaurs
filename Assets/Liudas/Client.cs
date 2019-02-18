using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
#pragma warning disable CS0618 // Type or member is obsolete

	public GameObject buildingControllerPrefab; // this is spawned when the client is connected to the server


	private const int MAX_USERS = 2;
	private const int PORT = 26090;
	private const int BYTE_SIZE = 1024;
	public string ipAddress = "127.0.0.1";

	public int hostId;
	private byte reliableChannel;

	public int localConnectionId;


	public bool isHost;
	private bool isStarted;

	byte error;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		ConnectToServer();
	}
	private void Update()
	{
		UpdateMessagePump();

		if (Input.GetMouseButtonDown(1))
			Send("BP", reliableChannel);

	}

	private void UpdateMessagePump()
	{

		if (!isStarted)
			return;

		int recHostId;
		int connectionId;
		int channelId;

		byte[] recBuffer = new byte[BYTE_SIZE];
		int dataSize;

		//print("Error: " + error);

		NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

		switch (type)
		{
			case NetworkEventType.ConnectEvent:
				print("Host Id: " + hostId);
				ConsoleScript.Print("Client", "I am connected.");
				break;

			case NetworkEventType.DataEvent:

				string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
				ConsoleScript.Print("ClientGotMsg", msg);

				string[] splitData = msg.Split('|');

				switch (splitData[0])
				{
					// Client just connected to the network
					case "OBP": OtherButtonPress(); break;

					default: ConsoleScript.Print("Client", "Unknown message: " + splitData[0]); break;
				}

				break;


			case NetworkEventType.DisconnectEvent:
				ConsoleScript.Print("Client", "I am disconnecting.");
				break;
		}

	}

	private void OtherButtonPress()
	{
		ConsoleScript.Print("Client", "Other client pressed the button!");
	}
	public void ButtonPress()
	{
		Send("BP", reliableChannel);
	}

	public void Send(string msg, int channelId)
	{
		ConsoleScript.Print("ClientSendMsg", msg);
		byte[] msgByte = Encoding.Unicode.GetBytes(msg);
		NetworkTransport.Send(hostId, localConnectionId, channelId, msgByte, msg.Length * sizeof(char), out error);
	}

	public void ConnectToServer()
	{
		NetworkTransport.Init();

		ConnectionConfig cc = new ConnectionConfig();
		reliableChannel = cc.AddChannel(QosType.Reliable);

		HostTopology hostTopology = new HostTopology(cc, MAX_USERS);

		hostId = NetworkTransport.AddHost(hostTopology, 0);

		localConnectionId = NetworkTransport.Connect(hostId, ipAddress, PORT, 0, out error);
		print("localConnectionId: " + localConnectionId);

		ConsoleScript.Print("Client", "connected to " + ipAddress + " server");

		isStarted = true;
	}
	public void DisconnectFromServer()
	{
		isStarted = false;
		NetworkTransport.Shutdown();
	}




#pragma warning restore CS0618 // Type or member is obsolete
}
