using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinosaur : MonoBehaviour
{
    public int id;
    public int tileX, tileZ;
    public string[] infoText;
    [Space]

    public int health;
    public GameObject[] hearts;
    public Sprite halfHeart, fullHeart, halfHeart_p2, fullHeart_p2;
    public int playerID;

    [Header("Sounds")]
    public AudioClip Move;
    public AudioClip Roar;
    public AudioClip Attack;
    public AudioClip Death;

    [Space]
    public ParticleSystem BloodParticles;

    [Space]
    public pos[] whereCanMove;

    void Start()
    {
       // UpdateHealth();
    }

    void Update()
    {
        
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < 5; i++)
        {
            hearts[i].SetActive(false);
        }
        for (int i = 0; i < health/2; i++)
        {
            hearts[i].SetActive(true);
            if(playerID == 1) hearts[i].GetComponent<SpriteRenderer>().sprite = fullHeart;
            else hearts[i].GetComponent<SpriteRenderer>().sprite = fullHeart_p2;

        }
        if (health % 2 == 1)
        {
            hearts[(health / 2)].SetActive(true);
            if (playerID == 1) hearts[(health / 2)].GetComponent<SpriteRenderer>().sprite = halfHeart;
            else hearts[(health / 2)].GetComponent<SpriteRenderer>().sprite = halfHeart_p2;
        }
    }

    public void LoseHealth(int hp)
    {
        health -= hp;
        UpdateHealth();

        // Set the correct color and play the particle
        var main = BloodParticles.main;
        if (playerID == 1)
            main.startColor = Color.blue;
        else
            main.startColor = Color.red;
        BloodParticles.Play();
        // and play a sound
        LowPolyAnimalPack.AudioManager.PlaySound(Roar, transform.position);
    }


}
