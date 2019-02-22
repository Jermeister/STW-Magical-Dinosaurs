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

    [Header("Action Costs")]
    public int AttackCost;
    public int MoveCost;
    public int SpecialCost;

    public int damage;

    [Header("Move/Attack pattern images")]
    public Sprite MovePattern;
    public Sprite AttackPattern;

    [Header("Sounds")]
    public AudioClip Move;
    public AudioClip Roar;
    public AudioClip Attack;
    public AudioClip Death;

	// Lerping
	bool isMoving;
	Vector3 targetPos;
	Quaternion targetRot;
	float currentLerpTime = 0;
	public float dinoMoveSpeed = 5f;

    [Space]
    public ParticleSystem BloodParticles;

    [Space]
    public pos[] whereCanMove;

    [Space]
    public pos[] whereCanAttack;

    void Start()
    {
       // UpdateHealth();
    }

    void Update()
    {
        if (isMoving)
		{
			if (Vector3.Distance(transform.position, targetPos) > 0.1f)
			{
				transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * dinoMoveSpeed);
			} else
			{
				transform.position = targetPos;
				isMoving = false;
			}

			if (currentLerpTime < 1)
			{
				currentLerpTime += Time.deltaTime * 2;
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, currentLerpTime);
			} else
			{
				transform.rotation = targetRot;
			}
		}
    }

	public void SetTargetMovement(Vector3 targetPosition)
	{
		isMoving = true;
		targetPos = targetPosition;
		targetRot = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
		currentLerpTime = 0;
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
        ConsoleScript.Print("TEST", "nuime" + ", health: " + health + ", nuims: " + hp);

        health -= hp;
        UpdateHealth();
        ConsoleScript.Print("TEST", "liko: " + health);

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
