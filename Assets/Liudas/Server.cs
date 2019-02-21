using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
#pragma warning disable CS0618 // Type or member is obsolete

	private GridManager gridManagerScr;
	private MultiplayerController multiplayerControllerScr;
	private Client clientScr;
	private UIController uiControllerScr;

	private const int MAX_USERS = 2;
	private const int PORT = 26090;
	private const int BYTE_SIZE = 1024;

	public int hostId;
	private byte reliableChannel;

	private bool isStarted;

	byte error;


	public int currentTurnPlayerId = 1;

	public int playersSetupReady = 0;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		gridManagerScr = GameObject.Find("Grid").GetComponent<GridManager>();

		StartServer();
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
			case NetworkEventType.DataEvent:

				string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
				ConsoleScript.Print("ServerGotMsg", msg);

				DecryptMessage(connectionId, msg);

				break;

			case NetworkEventType.ConnectEvent:

				ConsoleScript.Print("Server", "Client " + connectionId + " has connected.");
				//Send("NC", )
				//if (connectionId == 1)
				{
					SendAll("asd", reliableChannel);
				}
				//Send("asd", reliableChannel, 0);
				//Send("asd", reliableChannel, 1);
				//Send("asd", reliableChannel, 2);

				if (connectionId == 2)
				{
					gridManagerScr.GirdManagerSetUp();
					string generatedString = gridManagerScr.BuildObstaclesString();

					SendAll("O|" + generatedString, reliableChannel);

					// Call our own client uiControllerScr.ToSetup
					uiControllerScr.ToSetup();
				}




				break;
			case NetworkEventType.DisconnectEvent:
				ConsoleScript.Print("Server", "Client " + connectionId + " has disconnected.");
				break;
		}
	}

	public void DecryptMessage(int id, string msg)
	{
		string[] splitData = msg.Split('|');

		switch (splitData[0])
		{

			case "SPR": // server player ready (in setup)
				playersSetupReady++;
				ConsoleScript.Print("Server", "playersSetupReady: " + playersSetupReady);

				if (playersSetupReady >= 2)
				{
					ConsoleScript.Print("Server", "Both players have done their setup");

					string msg2 = "BPR";
					SendAll(msg2, reliableChannel);
				}
				break;

			case "SED": // server Encoded Dinos message from client
				Send("ED|" + splitData[1], reliableChannel, OtherConnectionId(id));
				break;

			case "SET": // server Ended Turn (player ended turn, give other player the rights to do things)
				ServerEndedTurn(id);
				break;

			case "SDM": // server Dino Move, other player moved dino, sync it up
				ServerDinoMove(id, splitData[1]);
				break;

			case "asd": break;

			default: ConsoleScript.Print("Server", "Unknown message: " + splitData[0]); break;
		}
	}

	private void ButtonPress(int connId)
	{
		ConsoleScript.Print("Server", connId + " client press the button.");
		ConsoleScript.Print("Server", "sending message to client: " + OtherConnectionId(connId));
		Send("BP", reliableChannel, OtherConnectionId(connId));
		//SendAll("BP", reliableChannel);
	}

	void ServerEndedTurn(int connId)
	{
		currentTurnPlayerId = currentTurnPlayerId == 1 ? 2 : 1;
		Send("YT", reliableChannel, currentTurnPlayerId);
	}
	void ServerDinoMove(int connId, string encodedText)
	{
		string msg = "DM" + encodedText;
		Send(msg, reliableChannel, OtherConnectionId(connId));
	}

	private int OtherConnectionId(int connId)
	{
		return connId == 1 ? 2 : 1;
	}

	private void Send(string message, int channelId, int cnnId)
	{
		// Send a message for one client
		ConsoleScript.Print("ServerSendMsg", message);

		// check this, if I am sending to the same computer
		if (cnnId == 1)
		{
			//ConsoleScript.Print("Server", "Sending to Client via script");
			multiplayerControllerScr.DecryptMessage(message);
		}
		else
		{
			//ConsoleScript.Print("Server", "Sending to Client via networking");
			byte[] msg = Encoding.Unicode.GetBytes(message);
			NetworkTransport.Send(hostId, cnnId, channelId, msg, message.Length * sizeof(char), out error);
		}


	}
	private void SendAll(string message, int channelId)
	{
		//ConsoleScript.Print("ServerSendMsg", message);

		Send(message, channelId, 2);
		Send(message, channelId, 1);
		
	}

	public void StartServer()
	{
		NetworkTransport.Init();

		ConnectionConfig cc = new ConnectionConfig();
		reliableChannel = cc.AddChannel(QosType.Reliable);

		HostTopology hostTopology = new HostTopology(cc, MAX_USERS);
		hostId = NetworkTransport.AddHost(hostTopology, PORT, null);

		multiplayerControllerScr = GameObject.Find("MultiplayerController").GetComponent<MultiplayerController>();
		clientScr = GameObject.Find("Client(Clone)").GetComponent<Client>();
		uiControllerScr = GameObject.Find("MainCanvas").GetComponent<UIController>();

		ConsoleScript.Print("Server", "Server is started.");
		isStarted = true;
	}
	public void StopServer()
	{
		NetworkTransport.Shutdown();
		multiplayerControllerScr = null;
		uiControllerScr = null;
		clientScr = null;
		isStarted = false;
		ConsoleScript.Print("Server", "Server shutdown.");
	}





#pragma warning restore CS0618 // Type or member is obsolete
}
