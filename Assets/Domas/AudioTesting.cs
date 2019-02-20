using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTesting : MonoBehaviour
{

    public AudioClip ThemeSong;

    private void Start()
    {
        LowPolyAnimalPack.AudioManager.PlaySound(ThemeSong, Vector3.zero);
    }
}
