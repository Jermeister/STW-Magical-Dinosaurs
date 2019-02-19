using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
	public string ipAdrress;

	public GameObject clientPrefab;
	public GameObject serverPrefab;

	[HideInInspector] public GameObject client;
	private Client clientScr;
	[HideInInspector] public GameObject server;

	public GameObject consoleTextObject;

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		ConsoleScript.SetUp(consoleTextObject);
		ConsoleScript.isConsoleActive = true;
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

	public void ButtonPressed()
	{
		if (clientScr != null)
			clientScr.ClientButtonPress();
	}

	public void CreateGame()
	{
		ConsoleScript.Print("Multiplayer", "Create game button was pressed.");
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
		ConsoleScript.Print("Multiplayer", "Join game button was pressed.");
		if (client == null)
		{
			ConsoleScript.Print("Multiplayer", "Ask for IP.");
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

		for (int i = 0; i < splitMsg.Length; i++)
		{
			ConsoleScript.Print("Multiplayer", splitMsg[i]);
		}

		switch (splitMsg[0])
		{
			// Client just connected to the network
			case "OBP": OtherButtonPress(); break;

			case "asd": break;

			default: ConsoleScript.Print("Multiplayer", "Unknown message: " + splitMsg[0]); break;
		}

	}

	private void OtherButtonPress()
	{
		ConsoleScript.Print("Client", "Other client pressed the button!");
	}


	public void DisableCreateJoinMenus()
	{
	}
	public void EnableCreateJoinMenus()
	{
	}

	private void OnApplicationQuit()
	{
		if (client != null)
			client.GetComponent<Client>().DisconnectFromServer();

		if (server != null)
			server.GetComponent<Server>().StopServer();

	}

}
