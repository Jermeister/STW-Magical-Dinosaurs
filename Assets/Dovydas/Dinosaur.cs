using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinosaur : MonoBehaviour
{
    public int id;
    public int tileX, tileZ;
    public string[] infoText;

    [Header("Sounds")]
    public AudioClip Move;
    public AudioClip Roar;
    public AudioClip Attack;
    public AudioClip Death;

    public pos[] whereCanMove;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
