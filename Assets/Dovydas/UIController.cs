using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum Action { Passive = 0, Attack = 1, SpecialMove = 2, Move = 3 };

public class UIController : MonoBehaviour
{
    #region Variables
    public int manaLeft, maxMana, manaSavingMax, manaPerTurn, startingMana;

    public bool unitIsSelected;
    public bool needActionAndInfoZone;

    public float[] heights;

    [Range(1,15)]
    public int manaCost;

    public Text[] infoText_Text;
    public string[] infoText_string;

    public GameObject[] ActivateButton;
    public GameObject[] BackgroundButton;
    public GameObject[] ExitInfoButton;
    public GameObject[] EnterInfoButton;
    public GameObject[] infoBackground;

    public GameObject infoAndActionsZone;

    public RectTransform manaBar;
    public float what = 100f;

    public GameObject screen_JoinToServer,
                      screen_MainMenu,
                      screen_setup,
                      screen_inGame,
                      screen_localMultiplayer,
                      screen_inWaitingForPlayer;

    public Image[] mana;
    public Sprite mana_empty, mana_full;
    public int unitSelected;

    public Text iptext;

    public bool myTurn;
    public GameObject notMyTurnText;
    public GameObject myTurnButton;

    /// Liudo
    public MultiplayerController multiplayerControllerScr;

    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Clicked_ToMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            unitIsSelected = true;
        }

        if (needActionAndInfoZone && unitIsSelected)
            infoAndActionsZone.SetActive(true);
        else
            infoAndActionsZone.SetActive(false);

        ManaCostBarUpdate();
    }


    public void Clicked_Activate(int id)
    {
        Debug.Log("Selected action: " + id);
    }

    public void Clicked_UnitSelected(int id)
    {
        unitIsSelected = true;
        unitSelected = id;
        GameObject.Find("Grid").GetComponent<GridManager>().SpawnDinoButton(id + 1);
        // use infoText_Text[id].text = "your text";
    }

    public void InitializeGame()
    {
        manaLeft = startingMana;
        for (int i = 0; i < maxMana; i++)
        {
            if (i < manaLeft)
                mana[i].sprite = mana_full;
            else
                mana[i].sprite = mana_empty;
        }
        GameObject.FindObjectOfType<GridManager>().inGame = true;
        GameObject.FindObjectOfType<FreeMovement>().inGame = true;
    }

    
    public void Clicked_NextTurn()
    {
        if (myTurn)
        {
            NextTurn();
        }

    }

    public void StartTurn()
    {
        notMyTurnText.SetActive(false);
        myTurnButton.SetActive(true);
    }

    public void NextTurn()
    {
        notMyTurnText.SetActive(true);
        myTurnButton.SetActive(false);

        #region Mana
        int manaLeftover = manaLeft;
        if (manaLeft > manaSavingMax)
        {
            manaLeft = manaLeft + manaSavingMax;
        }
        else
        {
            manaLeft += manaPerTurn;
        }

        if (manaLeft > maxMana)
        {
            manaLeft = maxMana;
        }

        for (int i = 0; i < maxMana; i++)
        {
            if (i < manaLeft)
                mana[i].sprite = mana_full;
            else
                mana[i].sprite = mana_empty;
        }
        #endregion
    }

    public void ManaCostBarUpdate()
    {
        if (manaCost > 0 && manaCost < 16)
        {
            manaBar.gameObject.SetActive(true);
            manaBar.sizeDelta = new Vector2(manaBar.sizeDelta.x, heights[manaCost - 1]);
        }
        else
            manaBar.gameObject.SetActive(false);
    }

    #region Main UI functionality
    public void Clicked_Info(int id)
    {
        infoText_Text[id].text = infoText_string[id];
        infoBackground[id].SetActive(true);
    }

    public void Clicked_ExitInfo(int id)
    {
        infoBackground[id].SetActive(false);
    }

    public void Clicked_ToLocalMultiplayed()
    {
        screen_localMultiplayer.SetActive(true);
        screen_JoinToServer.SetActive(false);
        screen_MainMenu.SetActive(false);
        screen_setup.SetActive(false);
        screen_inGame.SetActive(false);
        screen_inWaitingForPlayer.SetActive(false);
        needActionAndInfoZone = false;
        Clicked_ToInGame();

		multiplayerControllerScr.SplitScreen();
	}

    public void Clicked_ToJoinServer()
    {
        screen_localMultiplayer.SetActive(false);
        screen_JoinToServer.SetActive(true);
        screen_MainMenu.SetActive(false);
        screen_setup.SetActive(false);
        screen_inGame.SetActive(false);
        screen_inWaitingForPlayer.SetActive(false);
        needActionAndInfoZone = false;


    }

    public void Clicked_ToMainMenu()
    {
        screen_localMultiplayer.SetActive(false);
        screen_JoinToServer.SetActive(false);
        screen_MainMenu.SetActive(true);
        screen_setup.SetActive(false);
        screen_inGame.SetActive(false);
        infoAndActionsZone.SetActive(false);
        screen_inWaitingForPlayer.SetActive(false);
        needActionAndInfoZone = false;
    }

    public void Clicked_ToInGame()
    {
        unitIsSelected = false;
        screen_localMultiplayer.SetActive(false);
        screen_JoinToServer.SetActive(false);
        screen_MainMenu.SetActive(false);
        screen_setup.SetActive(false);
        screen_inGame.SetActive(true);
        screen_inWaitingForPlayer.SetActive(false);
        needActionAndInfoZone = true;
        InitializeGame();

		multiplayerControllerScr.SetupButtonPressed();

		/// TODO: disable SetupButton, enable text ui
		/// TODO: this client is ready, send to server
	}
	public void ToSetup()
    {
        unitIsSelected = false;
        screen_localMultiplayer.SetActive(false);
        screen_JoinToServer.SetActive(false);
        screen_MainMenu.SetActive(false);
        screen_setup.SetActive(true);
        screen_inGame.SetActive(false);
        screen_inWaitingForPlayer.SetActive(false);
        needActionAndInfoZone = true;
        GameObject.FindObjectOfType<GridManager>().inSetup = true;

    }

    public void ToWaitingForPlayer()
    {
        unitIsSelected = false;
        screen_localMultiplayer.SetActive(false);
        screen_JoinToServer.SetActive(false);
        screen_MainMenu.SetActive(false);
        screen_setup.SetActive(false);
        screen_inGame.SetActive(false);
        screen_inWaitingForPlayer.SetActive(true);
        needActionAndInfoZone = true;
    }

    public void Clicked_Setup_createdServer()
    {

		multiplayerControllerScr.CreateGame();
        iptext.text = "set ip here";

        ToWaitingForPlayer();
        //ToSetup();

    }

    public void Clicked_Setup_joinedServer()
    {
		string ip = GameObject.Find("InJoinGame").transform.Find("IP_InputField").transform.Find("Text").GetComponent<Text>().text;

        multiplayerControllerScr.ipAdrress = ip;

		multiplayerControllerScr.JoinGame();
        // ToSetup is called in Client script 

        //ToSetup();

    }

    public void Clicked_Setup_localMultiplayer()
    {

		multiplayerControllerScr.SplitScreen();


		ToSetup();

    }
    #endregion
}
