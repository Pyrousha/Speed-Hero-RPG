using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongFXManager : MonoBehaviour
{
    [SerializeField] private GameObject hitNoteParticleEffect;
    [SerializeField] private AudioClip attackHitSFX;
    [SerializeField] private bool playHitSFX;
    [SerializeField] private bool screenShake;

    [Header("colors")]
    [SerializeField] private Color earlyColor;
    [SerializeField] private Color perfectColor;
    [SerializeField] private Color lateColor;
    [SerializeField] private float alpha;
    private float earlyHue;
    private float perfectHue;
    private float lateHue;
    private float idk;

    private SongLoader songLoader;
    private AudioSource audioSource;

    [Header("Camera Shake Stuff")]
    [SerializeField] private Animator cameraAnim;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        songLoader = FindObjectOfType<SongLoader>();
        audioSource.clip = attackHitSFX;

        Color.RGBToHSV(earlyColor, out earlyHue, out idk, out idk);
        Color.RGBToHSV(perfectColor, out perfectHue, out idk, out idk);
        Color.RGBToHSV(lateColor, out lateHue, out idk, out idk);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyProjectileHit(Transform parentTransform, float aheadOffset, int attackNum)
    {
        if (playHitSFX)
        {
            PlayHitSound();
        }

        //Spawn particles
        GameObject newParticle = Instantiate(hitNoteParticleEffect, parentTransform);
        newParticle.transform.localPosition = Vector3.zero;
        newParticle.transform.localEulerAngles = Vector3.zero;
        newParticle.transform.parent = gameObject.transform;

        ParticleSystem.MainModule main = newParticle.GetComponent<ParticleSystem>().main;  
        main.startColor = new ParticleSystem.MinMaxGradient(GetColor(aheadOffset));

        if (screenShake)
        {
            //Reset all Triggers
            cameraAnim.ResetTrigger("Left");
            cameraAnim.ResetTrigger("DiagLeft");
            cameraAnim.ResetTrigger("Up");
            cameraAnim.ResetTrigger("DiagRight");
            cameraAnim.ResetTrigger("Right");

            //Trigger Camera Shake
            switch (attackNum)
            {
                case 2:
                    {
                        cameraAnim.SetTrigger("Left");
                        break;
                    }
                case 3:
                    {
                        cameraAnim.SetTrigger("DiagLeft");
                        break;
                    }
                case 5:
                    {
                        cameraAnim.SetTrigger("Up");
                        break;
                    }
                case 7:
                    {
                        cameraAnim.SetTrigger("DiagRight");
                        break;
                    }
                case 9:
                    {
                        cameraAnim.SetTrigger("Right");
                        break;
                    }
            }
        }
    }

    private Color GetColor(float aheadOffset)
    {
        /*
        if (aheadOffset < 0)
        {
            //early
            aheadOffset = -aheadOffset; //make it positive for easy math

            return (earlyColor * aheadOffset + perfectColor * (1 - aheadOffset));
        }
        else
        {
            //late 

            return (perfectColor * (1 - aheadOffset) + lateColor * aheadOffset);
        }*/

        if (aheadOffset < 0)
        {
            //early
            aheadOffset = -aheadOffset; //make it positive for easy math

            float hue = earlyHue * aheadOffset + perfectHue * (1 - aheadOffset);
            Color newColor = Color.HSVToRGB(hue, 1, 1);
            newColor.a = alpha;
            return (newColor);
        }
        else
        {
            //late 

            float hue = (perfectHue * (1 - aheadOffset) + lateHue * aheadOffset);
            Color newColor = Color.HSVToRGB(hue, 1, 1);
            newColor.a = alpha;
            return (newColor);
        }
    }

    public void PlayHitSound()
    {
        audioSource.Play(0);
        Debug.Log("Beat num: " + songLoader.SongPositionInBeats);
    }
}
