using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioSource audioSource;

    public void PlayClip(int clipIndex)
    {
        audioSource.clip = clips[clipIndex];
        audioSource.Play();
    }
}
