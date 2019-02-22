using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 posi;
    public Quaternion roti;

    public Obstacle(int id, int x, int z, Vector3 pos, Quaternion rot)
    {
        this.id = id;
        this.posi = pos;
        this.roti = rot;
        this.tileX = x;
        this.tileZ = z;
    }

    public int tileX, tileZ;
    public int id;
    // Start is called before the first frame update
    void Start()
    {
		this.transform.position = new Vector3(transform.position.x, 0.60f, transform.position.z);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
