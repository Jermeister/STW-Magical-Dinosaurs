﻿using System.Collections;
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
	public bool isBuildingMode = true;
	public List<GameObject> dinosaurPrefabs;

	//Private stuff
	private GameObject[,] Tiles;
	private TileScript[,] tileScripts;
	private int[,] TilePlayerMap;
	private char[,] TileTypeMap;
	private Transform parent;
	private GameObject selectionInstance;
	private List<GameObject> SpawnedObjects;
	private int monsterId = 1;

	// Start is called before the first frame update
	void Start()
    {
		#region Instantiating objects
		// Instantiating objects
		Tiles = new GameObject[gridSize,gridSize];
		tileScripts = new TileScript[gridSize, gridSize];
		TileTypeMap = new char[gridSize,gridSize];
		TilePlayerMap = new int[gridSize, gridSize];
		SpawnedObjects = new List<GameObject>();

		parent = this.transform;
		#endregion

		#region Instantiating tiles
		for (int x = 0;x < gridSize; x++)
		{
			for(int z = 0;z<gridSize; z++)
			{
				Tiles[x, z] = Instantiate(tile, new Vector3(x,ySpawnValue,z),Quaternion.identity,parent);

				TileTypeMap[x, z] = '0';

				tileScripts[x, z] = Tiles[x, z].GetComponent<TileScript>();
				if (isBuildingMode)
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


	}

    // Update is called once per frame
    void Update()
    {
		UpdateObjectSpawn();
		UpdateSelectionSquare();
    }

	void UpdateTileMap(int x, int y, char tileType, char tilePlayer)
	{

	}

	public void SpawnDinoButton(int id)
	{
		monsterId = id;
	}

	void PlaceObjectNear(Vector3 clickPoint)
	{
		var finalPosition = GetNearestPointOnGrid(clickPoint);

		int xCount = Mathf.RoundToInt(finalPosition.x / 1);
		int zCount = Mathf.RoundToInt(finalPosition.z / 1);

		Quaternion rot;
		if (playerId == 1)
			rot = Quaternion.Euler(0, 90, 0);
		else
			rot = Quaternion.Euler(0, -90, 0);

		SpawnedObjects.Add(Instantiate(dinosaurPrefabs[monsterId-1], new Vector3(xCount, 0.75f, zCount), rot));
		TileTypeMap[xCount, zCount] = (char)monsterId;
	}

	void PlaceSelectionNear(Vector3 mousePoint)
	{
		if (selectionInstance)
			Destroy(selectionInstance);
		var finalPosition = GetNearestPointOnGrid(mousePoint);
		if (finalPosition.x < 0 || finalPosition.x > gridSize-1 || finalPosition.z < 0 || finalPosition.z > gridSize-1)
			return;

		int xCount = Mathf.RoundToInt(finalPosition.x / 1);
		int zCount = Mathf.RoundToInt(finalPosition.z / 1);

		if (TileTypeMap[xCount, zCount] == '0' && TilePlayerMap[xCount, zCount] == playerId)
		{

			selectionInstance = Instantiate(selectionItem, new Vector3(finalPosition.x, 0.75f, finalPosition.z), Quaternion.identity);
		}
	}

	Vector3 GetNearestPointOnGrid(Vector3 position)
	{
		position -= transform.position;

		int xCount = Mathf.RoundToInt(position.x / 1);
		int yCount = Mathf.RoundToInt(position.y / 1);
		int zCount = Mathf.RoundToInt(position.z / 1);

		Vector3 result = new Vector3(
			(float)xCount * 1,
			(float)yCount * 1,
			(float)zCount * 1);

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
		if (Input.GetMouseButtonDown(0))
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
		RaycastHit hitInfo;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hitInfo))
		{
			PlaceSelectionNear(hitInfo.point);
		}
	}
}