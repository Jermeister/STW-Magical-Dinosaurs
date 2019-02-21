using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinosaur : MonoBehaviour
{
    public int id;
    public int tileX, tileZ;
    public string[] infoText;

    public int health;
    public GameObject[] hearts;
    public Sprite halfHeart, fullHeart;

    [Header("Sounds")]
    public AudioClip Move;
    public AudioClip Roar;
    public AudioClip Attack;
    public AudioClip Death;

    public pos[] whereCanMove;

    void Start()
    {
        UpdateHealth();
    }

    void Update()
    {
        
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < health/2; i++)
        {
            hearts[i].GetComponent<SpriteRenderer>().sprite = fullHeart;
        }
        if (health % 2 == 1)
        {
            hearts[health / 2].GetComponent<SpriteRenderer>().sprite = halfHeart;
        }
    }

    public void LoseHealth(int hp)
    {
        health -= hp;
        UpdateHealth();
    }

}
