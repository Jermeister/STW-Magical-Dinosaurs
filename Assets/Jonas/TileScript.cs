using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
	[HideInInspector]
	public char type = '0';
	public int playerType = 0;
	[SerializeField]
	private string gridManagerObjectString = "Grid";
	private GridManager gm;
	[SerializeField]
	private List<Material> materials;
	private Renderer rend;

	private Vector3 mousePos;

	void Awake()
	{
		rend = GetComponent<MeshRenderer>();
		gm = GameObject.Find(gridManagerObjectString).GetComponent<GridManager>();
	}

	// Start is called before the first frame update
	void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void updateTile()
	{
		rend.material = materials[playerType];
	}
}
