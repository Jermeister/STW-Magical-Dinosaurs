using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
	[Header("Player Settings")]
	public int playerId = 1;

	[Header("Grid Settings")]
	public int gridSize = 10;
	public GameObject tile;
	public float ySpawnValue = 0.3f;
	public GameObject selectionItem;

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

    //Private stuff
    private GameObject[,] Tiles;
	private TileScript[,] tileScripts;
	private int[,] TilePlayerMap;
	private int[,] TileTypeMap;
	private Transform parent;
	private GameObject selectionInstance;
	private List<GameObject> SpawnedObjects;
	private List<GameObject> SpawnedObstacles;
	private int monsterId = 1;

	// Start is called before the first frame update
	void Start()
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

		if (inSetup)
			SpawnRandomObstaclesOnGrid();
	}

    // Update is called once per frame
    void Update()
    {
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
		SpawnRandomObstaclesOnGrid();
	}

	public void SpawnDinoButton(int id)
	{
		monsterId = id;
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

		if (inTiles && TileTypeMap[xCount, zCount] == 0 && TilePlayerMap[xCount, zCount] == playerId)
		{
			SpawnedObjects.Add(Instantiate(dinosaurPrefabs[monsterId - 1], new Vector3(xCount, 0.75f, zCount), rot));
			Destroy(selectionInstance);
			TileTypeMap[xCount, zCount] = monsterId;
			TilePlayerMap[xCount, zCount] = playerId;
		}
	}

	/// <summary>
	/// Deleting dinosaur from a grid
	/// </summary>
	void DeleteObjectNear(Vector3 clickPoint)
	{
		var finalPosition = GetNearestPointOnGrid(clickPoint);

		int xCount = Mathf.RoundToInt(finalPosition.x);
		int zCount = Mathf.RoundToInt(finalPosition.z);

		Quaternion rot;
		if (playerId == 1)
			rot = Quaternion.Euler(0, 90, 0);
		else
			rot = Quaternion.Euler(0, -90, 0);

		// TODO: Check if pressing on the right tiles
		if (inTiles && TileTypeMap[xCount, zCount] == 0 && TilePlayerMap[xCount, zCount] == playerId)
		{
			SpawnedObjects.Add(Instantiate(dinosaurPrefabs[monsterId - 1], new Vector3(xCount, 0.75f, zCount), rot));
			Destroy(selectionInstance);
			TileTypeMap[xCount, zCount] = monsterId;
			TilePlayerMap[xCount, zCount] = playerId;
		}
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

		if (finalPosition.x < 0 || finalPosition.x > gridSize-1 || finalPosition.z < 0 || finalPosition.z > gridSize-1 ||
			TileTypeMap[xCount, zCount] != 0 || TilePlayerMap[xCount, zCount] != playerId)
		{
			Destroy(selectionInstance);
			inTiles = false;
			return;
		}

		inTiles = true;
		if (!selectionInstance)
		{
			selectionInstance = Instantiate(selectionItem, new Vector3(finalPosition.x, 0.75f, finalPosition.z), Quaternion.identity);
			return;
		}

		if(selectionInstance)
			selectionInstance.transform.position = new Vector3(finalPosition.x, 0.75f, finalPosition.z);

		
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
                Gizmos.DrawSphere(point, 0.1f);
            }

        }

    }

    void UpdateObjectSpawn()
	{
		if (Input.GetMouseButtonDown(0) && inGame)
		{
			RaycastHit hitInfo;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hitInfo))
			{
				PlaceObjectNear(hitInfo.point);
                
			}
		}
    }

	void UpdateSelectionSquare()
	{
        if (inGame)
        { 
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                PlaceSelectionNear(hitInfo.point);
            }
        }
	}
	
	void SpawnRandomObstaclesOnGrid()
	{
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
				int rndObstacle = Random.Range(0, obstaclePrefabs.Count-1);
				int rndRotation = Random.Range(0, 360);
				int rndRotation2 = Random.Range(0, 360);
				TileTypeMap[x, z] = obstacleId;
				TileTypeMap[x2, z2] = obstacleId;
				SpawnedObstacles.Add(Instantiate(obstaclePrefabs[rndObstacle], new Vector3(x, obstaclesSpawnY, z), Quaternion.Euler(new Vector3(0, rndRotation, 0))));
				SpawnedObstacles.Add(Instantiate(obstaclePrefabs[rndObstacle], new Vector3(x2, obstaclesSpawnY, z2), Quaternion.Euler(new Vector3(0, rndRotation2, 0))));
			}
		}
	}

}
