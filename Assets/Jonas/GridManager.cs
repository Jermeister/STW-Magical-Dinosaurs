using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GridManager : MonoBehaviour
{
	[Header("Player Settings")]
	public int playerId = 0;

	[Header("Grid Settings")]
	public int gridSize = 10;
	public GameObject tile;
	public float ySpawnValue = 0.3f;
	public GameObject selectionItem;
	public GameObject standItem; // Item that spawns when you select something

	[Header("Game Settings")]
	public List<GameObject> dinosaurPrefabs;
	public List<GameObject> obstaclePrefabs;
	public bool inGame = false;
	public bool inSetup = false;
	[Range(2,14)]
	public int maxObstaclesCount = 2;
	public float obstaclesSpawnY = 0.55f;
	[Header("Misc")]
	public bool inTiles = false;
	public int obstacleId = 10;
    public int monsterId = 1;

    //Dovydo
    public int selectedIndex;
    public Dinosaur selectedDino;
    public bool canBuild;

    [HideInInspector]
    public UIController uiController;


    //Private stuff
    private GameObject[,] Tiles;
	private TileScript[,] tileScripts;
	private int[,] TilePlayerMap;
	private int[,] TileTypeMap;
	private Transform parent;
	private GameObject selectionInstance;
	private GameObject standInstance;
	public List<GameObject> SpawnedObjects;
	private List<GameObject> SpawnedObstacles;

    public MultiplayerController mc;

    // Start is called before the first frame update
    public void GirdManagerSetUp()
    {
        #region Instantiating objects
        // Instantiating objects
        Tiles = new GameObject[gridSize,gridSize];
		tileScripts = new TileScript[gridSize, gridSize];
        TileTypeMap = new int[gridSize,gridSize];
		TilePlayerMap = new int[gridSize, gridSize];
		SpawnedObjects = new List<GameObject>();
		SpawnedObstacles = new List<GameObject>();

		parent = this.transform;
		#endregion

		#region Instantiating tiles
		for (int x = 0;x < gridSize; x++)
		{
			for(int z = 0;z<gridSize; z++)
			{
				Tiles[x, z] = Instantiate(tile, new Vector3(x,ySpawnValue,z),Quaternion.identity,parent);

				TileTypeMap[x, z] = 0;

				tileScripts[x, z] = Tiles[x, z].GetComponent<TileScript>();
				if (inSetup)
				{
					playerId = GameObject.Find("Client(Clone)").GetComponent<Client>().isHost ? 1 : 2;

					if (playerId == 1 && x < 3)
					{
						TilePlayerMap[x, z] = playerId;
						tileScripts[x, z].playerType = playerId;
					}
					else if (playerId == 2 &&  x > 6)
					{
						TilePlayerMap[x, z] = playerId;
						tileScripts[x, z].playerType = playerId;
					}
					else
					{
						TilePlayerMap[x, z] = 0;
						tileScripts[x, z].playerType = 0;
					}
					tileScripts[x, z].updateTile();
				}
			}
		}
        #endregion

        //Dovydo
        uiController = GameObject.FindObjectOfType<UIController>();
        mc = GameObject.FindObjectOfType<MultiplayerController>();
        //if (inSetup)
        //SpawnRandomObstaclesOnGrid();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            pos[] posses = new pos[12];
            posses[0] = new pos(5, 5);
            posses[1] = new pos(5, 6);
            posses[2] = new pos(6, 5);
            posses[3] = new pos(4, 5);
            posses[4] = new pos(5, 4);
            posses[5] = new pos(6, 6);
            posses[6] = new pos(4, 4);
            posses[7] = new pos(6, 4);
            posses[8] = new pos(4, 6);
            posses[9] = new pos(3, 5);
            posses[10] = new pos(5, 3);
            posses[11] = new pos(7, 5);

           //ShowPossibleMoves(posses);
        }
         
       



		UpdatePressObject();
		UpdateObjectSpawn();
		UpdateSelectionSquare();
    }

	public void StartGame()
	{
		inSetup = false;
		inGame = true;
	}

	public void StartSetup()
	{
		inGame = false;
		inSetup = true;
		//SpawnRandomObstaclesOnGrid();
	}

	public void SpawnDinoButton(int id)
	{
		monsterId = id;
	}

    public row[] canAttackObjects, canMoveObjects;
    public List<pos> nowTargetable;
    public void ShowPossibleMoves(pos[] positions, pos origin)
    {
        nowTargetable.Clear();
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++)
            {
                canMoveObjects[i].column[j].SetActive(false);
            }
        }
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].x + origin.x >= 0 && positions[i].x + origin.x <= 9 && positions[i].y + origin.y >= 0 && positions[i].y + origin.y <= 9 && TileTypeMap[positions[i].x + origin.x, positions[i].y + origin.y] == 0) {
                if ((1 + origin.x <= 9 && 1 + origin.y <= 9 && positions[i].x == 1 && positions[i].y == 1 && TileTypeMap[1 + origin.x, 0 + origin.y] != 0 && TileTypeMap[0 + origin.x, 1 + origin.y] != 0) ||
                    (-1 + origin.x >= 0 && 1 + origin.y <= 9 && positions[i].x == -1 && positions[i].y == 1 && TileTypeMap[-1 + origin.x, 0 + origin.y] != 0 && TileTypeMap[0 + origin.x, 1 + origin.y] != 0) ||
                    (1 + origin.x <= 9 && -1 + origin.y >= 0 && positions[i].x == 1 && positions[i].y == -1 && TileTypeMap[1 + origin.x, 0 + origin.y] != 0 && TileTypeMap[0 + origin.x, -1 + origin.y] != 0) ||
                    (-1 + origin.x >= 0 && -1 + origin.y >= 0 && positions[i].x == -1 && positions[i].y == -1 && TileTypeMap[-1 + origin.x, 0 + origin.y] != 0 && TileTypeMap[0 + origin.x, -1 + origin.y] != 0) ||

                    (1 + origin.y <= 9 && positions[i].x == 0 && positions[i].y == 2 && TileTypeMap[0 + origin.x, 1 + origin.y] != 0) ||
                    (-1 + origin.y >= 0 && positions[i].x == 0 && positions[i].y == -2 && TileTypeMap[0 + origin.x, -1 + origin.y] != 0) ||
                    (1 + origin.x <= 9 && positions[i].x == 2 && positions[i].y == 0 && TileTypeMap[1 + origin.x, 0 + origin.y] != 0) ||
                    (-1 + origin.x >= 0 && positions[i].x == -2 && positions[i].y == 0 && TileTypeMap[-1 + origin.x, 0 + origin.y] != 0) ||

                    (1 + origin.y <= 9 && positions[i].x == 0 && positions[i].y == 3 && TileTypeMap[0 + origin.x, 1 + origin.y] != 0) ||
                    (-1 + origin.y >= 0 && positions[i].x == 0 && positions[i].y == -3 && TileTypeMap[0 + origin.x, -1 + origin.y] != 0) ||
                    (1 + origin.x <= 9 && positions[i].x == 3 && positions[i].y == 0 && TileTypeMap[1 + origin.x, 0 + origin.y] != 0) ||
                    (-1 + origin.x >= 0 && positions[i].x == -3 && positions[i].y == 0 && TileTypeMap[-1 + origin.x, 0 + origin.y] != 0)||

                    (2 + origin.y <= 9 && positions[i].x == 0 && positions[i].y == 3 && TileTypeMap[0 + origin.x, 2 + origin.y] != 0) ||
                    (-2 + origin.y >= 0 && positions[i].x == 0 && positions[i].y == -3 && TileTypeMap[0 + origin.x, -2 + origin.y] != 0) ||
                    (2 + origin.x <= 9 && positions[i].x == 3 && positions[i].y == 0 && TileTypeMap[2 + origin.x, 0 + origin.y] != 0) ||
                    (-2 + origin.x >= 0 && positions[i].x == -3 && positions[i].y == 0 && TileTypeMap[-2 + origin.x, 0 + origin.y] != 0) ||

                    (1 + origin.y <= 9 && positions[i].x == 0 && positions[i].y == 4 && TileTypeMap[0 + origin.x, 1 + origin.y] != 0) ||
                    (-1 + origin.y >= 0 && positions[i].x == 0 && positions[i].y == -4 && TileTypeMap[0 + origin.x, -1 + origin.y] != 0) ||
                    (1 + origin.x <= 9 && positions[i].x == 4 && positions[i].y == 0 && TileTypeMap[1 + origin.x, 0 + origin.y] != 0) ||
                    (-1 + origin.x >= 0 && positions[i].x == -4 && positions[i].y == 0 && TileTypeMap[-1 + origin.x, 0 + origin.y] != 0) ||

                    (2 + origin.y <= 9 && positions[i].x == 0 && positions[i].y == 4 && TileTypeMap[0 + origin.x, 2 + origin.y] != 0) ||
                    (-2 + origin.y >= 0 && positions[i].x == 0 && positions[i].y == -4 && TileTypeMap[0 + origin.x, -2 + origin.y] != 0) ||
                    (2 + origin.x <= 9 && positions[i].x == 4 && positions[i].y == 0 && TileTypeMap[2 + origin.x, 0 + origin.y] != 0) ||
                    (-2 + origin.x >= 0 && positions[i].x == -4 && positions[i].y == 0 && TileTypeMap[-2 + origin.x, 0 + origin.y] != 0) ||

                    (3 + origin.y <= 9 && positions[i].x == 0 && positions[i].y == 4 && TileTypeMap[0 + origin.x, 3 + origin.y] != 0) ||
                    (-3 + origin.y >= 0 && positions[i].x == 0 && positions[i].y == -4 && TileTypeMap[0 + origin.x, -3 + origin.y] != 0) ||
                    (3 + origin.x <= 9 && positions[i].x == 4 && positions[i].y == 0 && TileTypeMap[3 + origin.x, 0 + origin.y] != 0) ||
                    (-3 + origin.x >= 0 && positions[i].x == -4 && positions[i].y == 0 && TileTypeMap[-3 + origin.x, 0 + origin.y] != 0))
                {

                }
                else
                {
                    canMoveObjects[positions[i].x + origin.x].column[positions[i].y + origin.y].SetActive(true);
                    nowTargetable.Add(new pos(positions[i].x + origin.x, positions[i].y + origin.y));
                }
            }
        }
    }
    public void ShowPossibleAttacks(pos[] positions, pos origin)
    {
        nowTargetable.Clear();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                canAttackObjects[i].column[j].SetActive(false);
            }
        }
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].x + origin.x >= 0 && positions[i].x + origin.x <= 9 && positions[i].y + origin.y >= 0 && positions[i].y + origin.y <= 9)
            {
                canAttackObjects[positions[i].x + origin.x].column[positions[i].y + origin.y].SetActive(true);
				nowTargetable.Add(new pos(positions[i].x + origin.x, positions[i].y + origin.y));
            }
        }
    }

    void HidePossibleActions()
    {
        nowTargetable.Clear();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                canAttackObjects[i].column[j].SetActive(false);
                canMoveObjects[i].column[j].SetActive(false);
            }
        }
        
    }

    /// <summary>
    /// Placing dinosaur in a grid
    /// </summary>
    void PlaceObjectNear(Vector3 clickPoint)
	{
		var finalPosition = GetNearestPointOnGrid(clickPoint);

		int xCount = Mathf.RoundToInt(finalPosition.x);
		int zCount = Mathf.RoundToInt(finalPosition.z);

		Quaternion rot;
		if (playerId == 1)
			rot = Quaternion.Euler(0, 90, 0);
		else
			rot = Quaternion.Euler(0, -90, 0);

        if (canBuild && inTiles && TileTypeMap[xCount, zCount] == 0 && TilePlayerMap[xCount, zCount] == playerId && uiController.dinosAre[monsterId - 1] < uiController.maxAmount_Dino[monsterId - 1] && uiController.dinoButtons[monsterId - 1].cost <= uiController.money)
		{
			SpawnedObjects.Add(Instantiate(dinosaurPrefabs[monsterId - 1], new Vector3(xCount, 0.75f, zCount), rot));
            if(monsterId == 1)
            {
                uiController.EnableStartButton();
            }
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().tileX = xCount;
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().tileZ = zCount;
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().id = monsterId;
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().playerID = playerId;
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().UpdateHealth();
            uiController.Bought(monsterId - 1);
            uiController.dinosAre[monsterId - 1]++;
            HidePossibleActions();
			Destroy(selectionInstance);
			TileTypeMap[xCount, zCount] = monsterId;
			TilePlayerMap[xCount, zCount] = playerId;
		}
	}
    // comment for no reason
    public void DecodeMovementCommand(string text)
    {
		string[] split = text.Split('*');
		pos tempPos = new pos(int.Parse(split[2]), int.Parse(split[3]));
		MultiplayerDinoMove(int.Parse(split[0]), int.Parse(split[1]), tempPos);
    }
    public string BuildMovementCommand(int tileX, int tileZ, pos targetPos)
    { 
        return tileX + "*" + tileZ + "*" + targetPos.x + "*" + targetPos.y;
    }

    public void MultiplayerDinoMove(int tileX, int tileZ, pos targetPos)
    {
        int index = -1;

        for (int i = 0; i < SpawnedObjects.Count; i++)
        {
			if (SpawnedObjects[i].GetComponent<Dinosaur>().tileX == tileX && SpawnedObjects[i].GetComponent<Dinosaur>().tileZ == tileZ)
            {
                index = i;
            }
        }

		if (index == -1)
		{
			ConsoleScript.Print("DinoMoveTEst", "cannot move here.");
			selectedDino = null;
			RemoveSelectionInstance();
			return;
		}


        Vector3 targetMoveLocation = canMoveObjects[targetPos.x].column[targetPos.y].transform.position + new Vector3(0f, 0.17f, 0f);
        SpawnedObjects[index].GetComponent<Dinosaur>().SetTargetMovement(targetMoveLocation);

        SpawnedObjects[index].GetComponent<Dinosaur>().tileX = targetPos.x;
        SpawnedObjects[index].GetComponent<Dinosaur>().tileZ = targetPos.y;

        TileTypeMap[tileX, tileZ] = 0;
        TileTypeMap[targetPos.x, targetPos.y] = SpawnedObjects[index].GetComponent<Dinosaur>().id;

        TilePlayerMap[targetPos.x, targetPos.y] = TilePlayerMap[tileX, tileZ];
        TilePlayerMap[tileX, tileZ] = 0;

        LowPolyAnimalPack.AudioManager.PlaySound(SpawnedObjects[index].GetComponent<Dinosaur>().Move, transform.position);

    }
    public void MultiplayerDinoAttack(int tileX, int tileZ, pos targetPos, int damage)
    {
        
        
        int index = -1;
        for (int i = 0; i < SpawnedObjects.Count; i++)
        {
            if (SpawnedObjects[i].GetComponent<Dinosaur>().tileX == targetPos.x && SpawnedObjects[i].GetComponent<Dinosaur>().tileZ == targetPos.y)
            {
                index = i;
                break;
            }
        }

		// Check if we found a hitable dino
		if (index == -1)
		{
			ConsoleScript.Print("DinoMoveTEst", "cannot attack nothing, removing selectionInstance");
			selectedDino = null;
			RemoveSelectionInstance();
			return;
		}

		ConsoleScript.Print("TEST", "INDEX: " + index);
        SpawnedObjects[index].GetComponent<Dinosaur>().LoseHealth(damage);


        LowPolyAnimalPack.AudioManager.PlaySound(SpawnedObjects[index].GetComponent<Dinosaur>().Attack, transform.position);
        
    }

    public string BuildObstaclesString()
    {
        List<Obstacle> toBeSpawned = new List<Obstacle>();

        int count = Random.Range(2, maxObstaclesCount);
		int value = (count / 2) * 2;
		for (int i = 0;i<=value;i+=2)
		{
			int x = Random.Range(0, gridSize - 1);
			int z = Random.Range(0, gridSize - 1);
			int x2 = gridSize - 1 - x;
			int z2 = gridSize - 1 - z;
			if (TileTypeMap[x,z] == 0 && TileTypeMap[x2,z2] == 0)
			{
				int rndObstacle = Random.Range(0, obstaclePrefabs.Count);
				int rndRotation = Random.Range(0, 360);
				int rndRotation2 = Random.Range(0, 360);
				TileTypeMap[x, z] = obstacleId+rndObstacle;
				TileTypeMap[x2, z2] = obstacleId+rndObstacle;
                toBeSpawned.Add(new Obstacle(rndObstacle, x, z, new Vector3(x, obstaclesSpawnY, z), Quaternion.Euler(new Vector3(0, rndRotation, 0))));
                toBeSpawned.Add(new Obstacle(rndObstacle, x2, z2, new Vector3(x2, obstaclesSpawnY, z2), Quaternion.Euler(new Vector3(0, rndRotation2, 0))));
			}
		}

        string result = "";
        for (int i = 0; i < toBeSpawned.Count; i++)
        {
            result += toBeSpawned[i].tileX + "*" + toBeSpawned[i].tileZ + "*" + toBeSpawned[i].id + "!";

        }
        return result;
    }
    public void DecodeObstaclesString(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int tileX = 0, tileZ = 0, identification = 0;
            int index = i;
            string temp = "";
            while (text[index] != '*')
            {
                temp += text[index];
                index++;
            }
            for (int a = 0; a < temp.Length; a++)
            {
                tileX += (int)temp[a] - 48;
            }
            temp = "";
            index++;

            while (text[index] != '*')
            {
                temp += text[index];
                index++;
            }
            for (int a = 0; a < temp.Length; a++)
            {
                tileZ += (int)temp[a] - 48;
            }
            temp = "";
            index++;

            while (text[index] != '!')
            {
                temp += text[index];
                index++;
            }
            i = index;
            for (int a = 0; a < temp.Length; a++)
            {
                identification += (int)temp[a] - 48;
            }

            Quaternion rot;
            if (playerId == 1)
                rot = Quaternion.Euler(0, 90, 0);
            else
                rot = Quaternion.Euler(0, -90, 0);

            SpawnedObstacles.Add(Instantiate(obstaclePrefabs[identification], new Vector3(tileX, 0.75f, tileZ), rot));
            TileTypeMap[tileX, tileZ] = 11;
        }
    }

    public string BuildDinosString()
    {
        string result = "";
        for (int i = 0; i < SpawnedObjects.Count; i++)
        {
            result += SpawnedObjects[i].GetComponent<Dinosaur>().tileX + "*" + SpawnedObjects[i].GetComponent<Dinosaur>().tileZ + "*" + SpawnedObjects[i].GetComponent<Dinosaur>().id + "!";
            
        }
        return result;
    }
    public void DecodeDinosString(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int tileX = 0, tileZ = 0, identification = 0;
            int index = i;
            string temp = "";
            while (text[index] != '*')
            {
                temp += text[index];
                index++;
            }
            for (int a = 0; a < temp.Length; a++)
            {
                tileX += (int)temp[a] - 48;
            }
            temp = "";
            index++;

            while (text[index] != '*')
            {
                temp += text[index];
                index++;
            }
            for (int a = 0; a < temp.Length; a++)
            {
                tileZ += (int)temp[a] - 48;
            }
            temp = "";
            index++;

            while (text[index] != '!')
            {
                temp += text[index];
                index++;
            }
            i = index;
            for (int a = 0; a < temp.Length; a++)
            {
                identification += (int)temp[a] - 48;
            }

            Quaternion rot;
            if (playerId != 1)
                rot = Quaternion.Euler(0, 90, 0);
            else
                rot = Quaternion.Euler(0, -90, 0);


            SpawnedObjects.Add(Instantiate(dinosaurPrefabs[identification - 1], new Vector3(tileX, 0.75f, tileZ), rot));
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().playerID = (playerId == 1 ? 2 : 1);
			SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().tileX = tileX;
			SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().tileZ = tileZ;
			TileTypeMap[tileX, tileZ] = SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().id + 1;
            TilePlayerMap[tileX, tileZ] = SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().playerID;
            SpawnedObjects[SpawnedObjects.Count - 1].GetComponent<Dinosaur>().UpdateHealth();
        }
    }
	
    void ClickOnPossibleAction(Vector3 clickPoint)
    {
        var finalPosition = GetNearestPointOnGrid(clickPoint);

        int xCount = Mathf.RoundToInt(finalPosition.x);
        int zCount = Mathf.RoundToInt(finalPosition.z);

        Quaternion rot;
        if (playerId == 1)
            rot = Quaternion.Euler(0, 90, 0);
        else
            rot = Quaternion.Euler(0, -90, 0);

        bool inThose = false;
        for (int i = 0; i < nowTargetable.Count; i++)
        {
            if(nowTargetable[i].x == xCount && nowTargetable[i].y == zCount)
            {
                inThose = true;
            }
        }

        if (inTiles && inThose)
        {
            for (int a = 0; a < SpawnedObjects.Count; a++)
            {
				int currentPosX = SpawnedObjects[a].GetComponent<Dinosaur>().tileX;
				int currentPosY = SpawnedObjects[a].GetComponent<Dinosaur>().tileZ;
				if (SpawnedObjects[a].GetComponent<Dinosaur>() == selectedDino && TilePlayerMap[currentPosX, currentPosY] == mc.GetThisClientId())
                {
                    HidePossibleActions();

                    if (uiController.actionID == 3)
                    {
                        mc.DinoMove(BuildMovementCommand(currentPosX, currentPosY, new pos(xCount, zCount)));
                        MultiplayerDinoMove(SpawnedObjects[a].GetComponent<Dinosaur>().tileX, SpawnedObjects[a].GetComponent<Dinosaur>().tileZ, new pos(xCount, zCount));
                    }
                    else if (uiController.actionID == 1)
                    {
                        mc.DinoAttack(BuildMovementCommand(currentPosX, currentPosY, new pos(xCount, zCount)));
                        MultiplayerDinoAttack(SpawnedObjects[a].GetComponent<Dinosaur>().tileX, SpawnedObjects[a].GetComponent<Dinosaur>().tileZ, new pos(xCount, zCount), SpawnedObjects[a].GetComponent<Dinosaur>().damage);
                    }

                    break;
                }
            }
            TilePlayerMap[xCount, zCount] = playerId;
        }
    }

	/// <summary>
	/// Deleting dinosaur from a grid
	/// </summary>
	void HandlePressOnObjectNear(Vector3 clickPoint)
	{
		var finalPosition = GetNearestPointOnGrid(clickPoint);

		int xCount = Mathf.RoundToInt(finalPosition.x);
		int zCount = Mathf.RoundToInt(finalPosition.z);

        if (inSetup && TileTypeMap[xCount, zCount] > 0 && TileTypeMap[xCount, zCount] < 10)
		{
			for(int i = 0;i<SpawnedObjects.Count;i++)
			{
				if (canBuild && (int)SpawnedObjects[i].transform.position.x == xCount && (int)SpawnedObjects[i].transform.position.z == zCount)
				{
                    if (SpawnedObjects[i].GetComponent<Dinosaur>().id == 0 || SpawnedObjects[i].GetComponent<Dinosaur>().id == 1)
                    {
                        uiController.DisableStartButton();
                    }
                    Destroy(SpawnedObjects[i]);
                    uiController.dinosAre[SpawnedObjects[i].GetComponent<Dinosaur>().id - 1]--;
                    uiController.Sold(SpawnedObjects[i].GetComponent<Dinosaur>().id - 1);
                    SpawnedObjects.RemoveAt(i);
					TileTypeMap[xCount, zCount] = 0;
					TilePlayerMap[xCount, zCount] = playerId;
				}
			}
		}

		if (inGame && TileTypeMap[xCount, zCount] > 0 && TileTypeMap[xCount, zCount] < 10)
		{
			if (!standInstance && !uiController.unitIsSelected)
			{
           
                standInstance = Instantiate(standItem, new Vector3(finalPosition.x, 0.60f, finalPosition.z), Quaternion.identity);
                monsterId = TileTypeMap[xCount, zCount];
                for (int i = 0; i < SpawnedObjects.Count; i++)
                {
                    if(SpawnedObjects[i].GetComponent<Dinosaur>() != null && SpawnedObjects[i].GetComponent<Dinosaur>().tileX == xCount && SpawnedObjects[i].GetComponent<Dinosaur>().tileZ == zCount)
                    {
                        selectedIndex = i;
                        selectedDino = SpawnedObjects[i].GetComponent<Dinosaur>();
                    }
                }
                uiController.unitIsSelected = true;
                UpdateUIManaCost(xCount, zCount);
                return;
			}

            if (standInstance && !uiController.unitIsSelected)
            {
                //HidePossibleActions();
                standInstance.transform.position = new Vector3(finalPosition.x, 0.60f, finalPosition.z);
                monsterId = TileTypeMap[xCount, zCount];
                for (int i = 0; i < SpawnedObjects.Count; i++)
                {
                    if (SpawnedObjects[i].GetComponent<Dinosaur>() != null && SpawnedObjects[i].GetComponent<Dinosaur>().tileX == xCount && SpawnedObjects[i].GetComponent<Dinosaur>().tileZ == zCount)
                    {
                        selectedIndex = i;
                        selectedDino = SpawnedObjects[i].GetComponent<Dinosaur>();

                    }
                }
                uiController.unitIsSelected = true;
                UpdateUIManaCost(xCount, zCount);

            }
        }
        
		else
		{

			Destroy(standInstance);
		}

        if (TileTypeMap[xCount, zCount] == 0 || TileTypeMap[xCount, zCount] > 9)
        {
            //HidePossibleActions();
            uiController.unitIsSelected = false;
        }
    }

    void UpdateUIManaCost(int xCount, int zCount)
    {
        var num = TileTypeMap[xCount, zCount] - 1;
        var values = dinosaurPrefabs[num].GetComponent<Dinosaur>();
        uiController.UpdateUIManaCost(values.MoveCost, values.AttackCost, values.SpecialCost);
    }

	/// <summary>
	/// Spawning selection item so that player sees where he can spawn a dinosaur
	/// </summary>
	/// <param name="mousePoint"></param>
	void PlaceSelectionNear(Vector3 mousePoint)
	{
		var finalPosition = GetNearestPointOnGrid(mousePoint);
		int xCount = Mathf.RoundToInt(finalPosition.x);
		int zCount = Mathf.RoundToInt(finalPosition.z);

		// Checking if we are holding mouse on the same tile, so selection does not despawn and we don't have to spawn it again
		if (selectionInstance && (int)selectionInstance.transform.position.x == xCount && (int)selectionInstance.transform.position.z == zCount)
			return;

		if ((inGame || inSetup) && finalPosition.x < 0 || finalPosition.x > gridSize-1 || finalPosition.z < 0 || finalPosition.z > gridSize-1)
		{
			RemoveSelectionInstance();
			return;
		}

        if (inSetup && (TileTypeMap[xCount, zCount] != 0 || TilePlayerMap[xCount, zCount] != playerId))
		{
			RemoveSelectionInstance();
			return;
		}

		inTiles = true;
		if (!selectionInstance)
		{
			HidePossibleActions();
			uiController.unitIsSelected = true;
           
            selectionInstance = Instantiate(selectionItem, new Vector3(finalPosition.x, 0.60f, finalPosition.z), Quaternion.identity);
            return;
		}

		if (selectionInstance)
        {
            //HidePossibleActions();
            selectionInstance.transform.position = new Vector3(finalPosition.x, 0.60f, finalPosition.z);
        }
    }

	void RemoveSelectionInstance()
	{
		HidePossibleActions();
		uiController.unitIsSelected = false;
		HidePossibleActions();
		Destroy(selectionInstance);
		inTiles = false;
	}

	Vector3 GetNearestPointOnGrid(Vector3 position)
	{
		position -= transform.position;

		int xCount = Mathf.RoundToInt(position.x);
		int yCount = Mathf.RoundToInt(position.y);
		int zCount = Mathf.RoundToInt(position.z);

		Vector3 result = new Vector3(
			(float)xCount,
			(float)yCount,
			(float)zCount);

		result += transform.position;

		return result;
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = 0; x < gridSize; x += 1)
        {
            for (float z = 0; z < gridSize; z += 1)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0.75f, z));
                Gizmos.DrawSphere(point, 0.05f);
            }

        }

    }

    void UpdateObjectSpawn()
	{
		if (Input.GetMouseButtonDown(0) && inSetup)
		{
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                if (Physics.Raycast(ray, out hitInfo))
                {
                    PlaceObjectNear(hitInfo.point);

                }
            }
		}

        if (Input.GetMouseButtonDown(0) && inGame)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hitInfo))
                {
                    ClickOnPossibleAction(hitInfo.point);

                }
            }
        }
        
    }
	void UpdateSelectionSquare()
	{
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (inGame || inSetup)
            {
                RaycastHit hitInfo;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hitInfo))
                {
                    PlaceSelectionNear(hitInfo.point);
                }
            }
        }
	}
	void UpdatePressObject()
	{
		if (Input.GetMouseButtonDown(0) && (inSetup || inGame))
		{
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hitInfo))
                {
                    HandlePressOnObjectNear(hitInfo.point);
                }
            }
		}
	}

	/// <summary>
	/// TileType map'e, langelio ID su obsticles prasideda nuo 10. Dabar yra 9 obstacles, tai jų IDs bus 10-18
	/// Jei spawninsi multiplayeryje, imk objektą iš listo pagal tiletype id atimti 10 (18-10 = 8) - spawninti 9 objektą iš listo
	/// </summary>

}
