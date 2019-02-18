using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
	public GameObject clientPrefab;
	public GameObject serverPrefab;

	[HideInInspector] public GameObject client;
	private Client clientScr;
	[HideInInspector] public GameObject server;

	public GameObject consoleTextObject;
	public GameObject createJoinButtonsParent;

	private void Start()
	{
		ConsoleScript.SetUp(consoleTextObject);
		ConsoleScript.isConsoleActive = true;
	}

	public void ButtonPressed()
	{
		if (clientScr != null)
			clientScr.ButtonPress();
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

	public void DisableCreateJoinMenus()
	{
		createJoinButtonsParent.SetActive(false);
	}
	public void EnableCreateJoinMenus()
	{
		createJoinButtonsParent.SetActive(true);
	}

	private void OnApplicationQuit()
	{
		if (client != null)
			client.GetComponent<Client>().DisconnectFromServer();

		if (server != null)
			server.GetComponent<Server>().StopServer();

	}

}
