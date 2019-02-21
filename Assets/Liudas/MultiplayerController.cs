using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
	public int currentTurnPlayerId = 1;
	public string ipAdrress;

	public GameObject clientPrefab;
	public GameObject serverPrefab;

	[HideInInspector] public GameObject client;
	private Client clientScr;
	[HideInInspector] public GameObject server;

	private GridManager gridManagerScr;
	UIController uiControllerScr;
	public GameObject consoleTextObject;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		ConsoleScript.SetUp(consoleTextObject);
		ConsoleScript.isConsoleActive = true;
		gridManagerScr = GameObject.Find("Grid").GetComponent<GridManager>();
		uiControllerScr = GameObject.Find("MainCanvas").GetComponent<UIController>();
	}

	public void ResetEverything()
	{
		ipAdrress = "";

		if (client != null)
			Destroy(client);

		if (server != null)
			Destroy(server);

		client = null;
		server = null;

		clientScr = null;
	}

	public void CreateGame()
	{
		//ConsoleScript.Print("Multiplayer", "Create game button was pressed.");
		if (server == null)
		{
			server = Instantiate(serverPrefab);
			client = Instantiate(clientPrefab);
			clientScr = client.GetComponent<Client>();
			clientScr.isHost = true;
			DisableCreateJoinMenus();
		}
		else
			ConsoleScript.Print("Multiplayer", "Server is already created");
	}
	public void JoinGame()
	{
		//ConsoleScript.Print("Multiplayer", "Join game button was pressed.");
		if (client == null)
		{
			client = Instantiate(clientPrefab);
			clientScr = client.GetComponent<Client>();
			DisableCreateJoinMenus();
		}
		else
			ConsoleScript.Print("Multiplayer", "Client is already created");
	}
	public void SplitScreen()
	{
		ConsoleScript.Print("Multiplayer", "This is not yet implemented.");
	}

	public void DecryptMessage(string message)
	{

		string[] splitMsg = message.Split('|');
		ConsoleScript.Print("ClientGotMsg", message);

		switch (splitMsg[0])
		{
			#region ReceivedMessages

			case "O": // encrypted obstacles from server
				ReceivedMsgDecodeObstacles(splitMsg[1]);
				break;

			case "BPR": // Both Players Ready, send server encoded dino
				ReceivedMsgBothPlayersReady();
				break;

			case "ED": // Encoded Dinos from the server (from other player), spawn them in
				gridManagerScr.DecodeDinosString(splitMsg[1]);
				break;

			case "YT": // Your Turn, server told you that now it is your turn to do things in game
				ReceivedMsgYourTurn();
				break;

			case "DM": // Dino Move, sync up dino movement
				ReceivedMsgDinoMove(splitMsg[1]);
				break;

			case "asd": break;
			default: ConsoleScript.Print("Multiplayer", "Unknown message: " + splitMsg[0]); break;

			#endregion
		}
	}

	void ReceivedMsgDecodeObstacles(string encodedObstacleMessage)
	{
		if (!clientScr.isHost)
			gridManagerScr.GirdManagerSetUp();

		uiControllerScr.ToSetup();
		gridManagerScr.DecodeObstaclesString(encodedObstacleMessage);
	}
	void ReceivedMsgBothPlayersReady()
	{
		uiControllerScr.BothPlayersAreReadyScreen();
		clientScr.GenerateEncodedDino();
	}
	void ReceivedMsgYourTurn()
	{
		uiControllerScr.endTurnUI.SetActive(true);
		currentTurnPlayerId = clientScr.GetThisClientId();
	}
	void ReceivedMsgDinoMove(string encodedText)
	{
		gridManagerScr.DecodeMovementCommand(encodedText);
	}

	public void DinoMove(string encodedText)
	{
		clientScr.DinoMove(encodedText);
	}
	public void SetupButtonPressed()
	{
		clientScr.SetupButtonPress();
	}

	public void EndTurnButtonPress()
	{
		clientScr.EndTurnButtonPress();
	}

	public void DisableCreateJoinMenus()
	{
	}
	public void EnableCreateJoinMenus()
	{
	}

	public bool IsMyTurn()
	{
		return currentTurnPlayerId == clientScr.GetThisClientId();
	}

	private void OnApplicationQuit()
	{
		if (client != null)
			client.GetComponent<Client>().DisconnectFromServer();

		if (server != null)
			server.GetComponent<Server>().StopServer();

	}

}
