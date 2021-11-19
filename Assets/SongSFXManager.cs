using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip attackHitSFX;
    [SerializeField] private bool active;
    private SongLoader songLoader;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        songLoader = FindObjectOfType<SongLoader>();
        audioSource.clip = attackHitSFX;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyProjectileHit()
    {
        if (!active)
            return;

        audioSource.Play(0);
        Debug.Log("Audio played on: " + songLoader.SongPositionInBeats);
    }

    public void TestHitSound()
    {
        audioSource.Play(0);
        Debug.Log("Beat num: " + songLoader.SongPositionInBeats);
    }
}
