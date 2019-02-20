using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioList : MonoBehaviour
{
    private static AudioList instance;

    [Header("Music")]
    public static AudioClip MenuTheme;
    public static AudioClip AmbientTheme;
    public static AudioClip GameTheme;

    [Header("Sound Effects")]
    public static AudioClip Roar1;
    public static AudioClip Roar2;
    public static AudioClip Roar3;
    public static AudioClip Roar4;
    public static AudioClip Roar5;
    public static AudioClip Roar6;
    public static AudioClip Rocks;
    public static AudioClip DinoDead;
    public static AudioClip Fireball;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Audio List is active in the scene.");
            return;
        }
        instance = this;
    }
}
