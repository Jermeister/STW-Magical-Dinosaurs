﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
#pragma warning disable CS0618 // Type or member is obsolete

	public GameObject buildingControllerPrefab; // this is spawned when the client is connected to the server

	private GridManager gridManagerScr;
	private MultiplayerController multiplayerControllerScr;
	private Server serverScr;
	private UIController uiControllerScr;

	private const int MAX_USERS = 2;
	private const int PORT = 26090;
	private const int BYTE_SIZE = 1024;
	//public string ipAddress = "127.0.0.1";

	public int hostId;
	private byte reliableChannel;

	public int localConnectionId;

	public bool isHost;
	public bool isConnectionSuccessful;
	private bool isStarted;

	byte error;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		gridManagerScr = GameObject.Find("Grid").GetComponent<GridManager>();
		multiplayerControllerScr = GameObject.Find("MultiplayerController").GetComponent<MultiplayerController>();
		uiControllerScr = GameObject.Find("MainCanvas").GetComponent<UIController>();

		ConnectToServer();

		if (isHost)
		{
			serverScr = GameObject.Find("Server(Clone)").GetComponent<Server>();

			if (serverScr == null)
			{
				ConsoleScript.Print("Client", "Server script was not found");
			}
		}


	}
	private void Update()
	{
		UpdateMessagePump();
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

		NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

		switch (type)
		{
			case NetworkEventType.ConnectEvent:
				ConsoleScript.Print("Client", "I am connected.");
				break;

			case NetworkEventType.DataEvent:
				string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
				multiplayerControllerScr.DecryptMessage(msg);
				break;


			case NetworkEventType.DisconnectEvent:
				ConsoleScript.Print("Client", "I am disconnecting.");
				break;
		}

	}

	public void SetupButtonPress()
	{
		// PR = player ready
		int playerId = multiplayerControllerScr.GetThisClientId();
		string msg = "SPR|" + playerId;

		Send(msg, reliableChannel);
	}
	public void EndTurnButtonPress()
	{
		uiControllerScr.endTurnUI.SetActive(false);
		multiplayerControllerScr.currentTurnPlayerId = multiplayerControllerScr.GetOtherClientId();
		string msg = "SET";
		Send(msg, reliableChannel);
	}

	#region DinoActions

	public void DinoMove(string encodedText)
	{
		Send("SDM|" + encodedText, reliableChannel);
	}
	public void DinoAttack(string encodedText)
	{
		Send("SDA|" + encodedText, reliableChannel);
	}
	public void DinoSpecial(string encodedText)
	{
		Send("SDS|" + encodedText, reliableChannel);
	}

	#endregion

	public void GenerateEncodedDino()
	{
		// Get client's encoded dinos
		string dinoEncodedString = gridManagerScr.BuildDinosString();
		ConsoleScript.Print("Client", "dinoEncodedString: " + dinoEncodedString);

		string msg = "SED|" + dinoEncodedString;
		Send(msg, reliableChannel);

	}


	public void Send(string msg, int channelId)
	{
		ConsoleScript.Print("ClientSendMsg", msg);
		// check this, if I am sending to the same computer
		if (isHost)
		{
			//ConsoleScript.Print("Client", "Sending to server via script");
			serverScr.DecryptMessage(1, msg);
		}
		else
		{
			//ConsoleScript.Print("Server", "Sending to server via networking");
			byte[] msgByte = Encoding.Unicode.GetBytes(msg);
			NetworkTransport.Send(hostId, localConnectionId, channelId, msgByte, msg.Length * sizeof(char), out error);
		}



		//byte[] msgByte = Encoding.Unicode.GetBytes(msg);
		//NetworkTransport.Send(hostId, localConnectionId, channelId, msgByte, msg.Length * sizeof(char), out error);
	}

	public void ConnectToServer()
	{
		NetworkTransport.Init();

		ConnectionConfig cc = new ConnectionConfig();
		reliableChannel = cc.AddChannel(QosType.Reliable);

		HostTopology hostTopology = new HostTopology(cc, MAX_USERS);

		hostId = NetworkTransport.AddHost(hostTopology, 0);

		string IP = (isHost == true ? "127.0.0.1" : multiplayerControllerScr.ipAdrress);

		localConnectionId = NetworkTransport.Connect(hostId, IP, PORT, 0, out error);
		print("localConnectionId: " + localConnectionId);

		ConsoleScript.Print("Client", "connected to " + IP + " server");

		//if (!isHost)
		//	uiControllerScr.ToSetup();

		isConnectionSuccessful = true;
		isStarted = true;
	}
	public void DisconnectFromServer()
	{
		isStarted = false;
		NetworkTransport.Shutdown();

		multiplayerControllerScr.ResetEverything();
	}


#pragma warning restore CS0618 // Type or member is obsolete
}
