﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using System;

public class SongLoader : MonoBehaviour
{
    public CombatState state;
    GameObject startState;

    [Header("Song Prefab + Settings")]
    public GameObject songToLoadPrefab;
    [SerializeField] private float attackTravelTimeInBeats;
    private GameObject songToLoad;

    [Header("Objects + Stuff")]
    public GameObject noteParent;
    public GameObject noteEditorCamera;
    [SerializeField] private SongEventHandler songEventHandler;
    [SerializeField] private MoveTimerController moveTimerController;
    public GameObject combatCameraLights;
    public Camera noteEditorPreviewCam;
    public Camera beatOffsetCamera;
    public Canvas heroUICanvas;
    public EndBattle endBattle;
    public Enemy_Stats_Combat enemy;
    public Hero_Stats_Combat heroStatsCombat;
    public Animator heroAnim;
    public InputField bpmObj;
    public InputField songNameObj;

    public GameObject[] enemyProjectilesToResume = new GameObject[0];


    [Header("Song Properties (To be loaded, don't change!)")]
    [SerializeField] private float songBPM;
    public float SongBPM => songBPM;

    public float secsPerBeat;

    public float songPosition; //current song position, in seconds

    private float songPositionInBeats;
    public float SongPositionInBeats => songPositionInBeats;

    public float dspSongTime; //How many seconds have passed since the song started

    public AudioSource musicSource; //music source that will play music

    bool songPlaying;


    [Header("Timing + Settings")]
    public float songStartOffset;

    //public float beatTravelTime;
    public float BeatTravelTime => 0f;
    [SerializeField] private float beatTimingOffset;
    //public float audioLatencySecs;
    public GameObject songTimingPrefab;
    public GameObject[] thingsToToggleEnableForSettingOffset;
    //float startupBeats; //How many beats before should a note be spawned (dependent on beatTravelTime and song BPM)
    private float startupBeats;
    public float attackAnimationSpeedHit;
    private float attackAnimationSpeedDodge;
    public List<noteStruct> noteArray;

    public enum CombatState
    { 
        NoteEditor,
        Playing,
        BeatOffset
    }

    public struct noteStruct
    {
        public int songEventIndex;
        public float beatWithOffset;
        public int attackNum;
    }

    // Start is called before the first frame update
    void Start()
    {
        songPlaying = false;

        startState = GameObject.Find("PersistentGameInfo");

        if (startState != null)
            LoadDataFromPersistentGameInfo();
        else
            enemy.LoadFromEnemyObject();

        enemy.SetSongLoader(this);

        switch (state)
        {
            case (CombatState.Playing):
                {
                    //Load notes
                    LoadSongPrefab(songToLoadPrefab);

                    //Disable note placer camera, not needed in gameplay
                    noteEditorCamera.SetActive(false);

                    //Start Song
                    Invoke("PlaySongZero", songStartOffset);

                    break;
                }
            case (CombatState.NoteEditor):
                {
                    //Load notes
                    LoadSongPrefab(songToLoadPrefab);

                    noteEditorCamera.GetComponent<CubePlaceCam>().DisableGameComponents();
                    noteEditorCamera.GetComponent<CubePlaceCam>().songLoadedPrefab = songToLoad;

                    heroUICanvas.worldCamera = noteEditorPreviewCam;

                    heroStatsCombat.isNoteEditorMode = true;

                    bpmObj.text = songBPM.ToString();
                    songNameObj.text = songToLoad.name;
                    break;
                }
            case (CombatState.BeatOffset):
                {
                    //Disable note placer camera, not needed in gameplay
                    noteEditorCamera.SetActive(false);

                    //Get cameras ready to be shown in overworld
                    combatCameraLights.SetActive(false);
                    beatOffsetCamera.gameObject.SetActive(true);

                    foreach (GameObject go in thingsToToggleEnableForSettingOffset)
                    {
                        go.SetActive(!go.activeSelf);
                    }

                    //Make hero invincible
                    heroStatsCombat.isNoteEditorMode = true;

                    break;
                }
        }
    }

    private void LoadDataFromPersistentGameInfo()
    {
        CombatStartingState combatStartingState = startState.GetComponent<CombatStartingState>();
        state = combatStartingState.combatState;
        //beatTravelTime = combatStartingState.beatOffset;

        songToLoadPrefab = combatStartingState.songPrefab;
        enemy.SetEnemyObject(combatStartingState.enemyObject);
        enemy.LoadFromEnemyObject();
    }

    private void Update()
    {
        if (songPlaying)
        {
            //determine how many seconds since the song started
            songPosition = (float)AudioSettings.dspTime - dspSongTime;

            //determine how many beats since the song started
            songPositionInBeats = songPosition / secsPerBeat;

            if (noteArray.Count > 0)
            {
                noteStruct nextNote = noteArray[0];
                if (songPositionInBeats >= nextNote.beatWithOffset)
                {
                    if (nextNote.attackNum == 10)
                        songEventHandler.songEvents[nextNote.songEventIndex].Invoke(); //Do event
                    else
                        enemy.EnemyStartAttack(nextNote.attackNum); //Spawn attack
                    noteArray.RemoveAt(0);
                }
            }
            else
            {
                if (songPosition > musicSource.clip.length)
                {
                    songPlaying = false;

                    if (state == CombatState.BeatOffset)
                    {
                        //Restart song
                        FillNoteArray(0);
                        PlaySong(0);
                    }
                    else
                    {
                        //End the battle
                        endBattle.StartFadeOut(true);
                    }
                }
            }
        }
    }

    /// <summary> Spawn note objects from the specified prefab, and load song properties </summary>
    void LoadSongPrefab(GameObject songPrefab)
    {
        if (songPrefab == null)
            return;

        songToLoad = Instantiate(songPrefab, transform);
        songToLoad.name = songToLoad.name.Replace("(Clone)", "");

        SongProperties songProperties = songToLoad.GetComponent<SongProperties>();

        //Set all notes to be childed to the noteParent GameObject
        int i = 0;
        while (i < songToLoad.transform.childCount)
        {
            Transform child = songToLoad.transform.GetChild(i);
            child.parent = noteParent.transform;
        }

        //Change name of noteparent GO to the name of the song to confirm loading worked properly
        noteParent.name += ("(" + songToLoad.name + ")");

        songBPM = songProperties.BPM;
        secsPerBeat = 60f / songBPM;
        
        //Set song to play based on song from loaded prefab
        musicSource.clip = songToLoad.GetComponent<AudioSource>().clip;

        //Set BPM for moveTimerController
        moveTimerController.CalculateOffset();

        /*
        #region Old Beat Offset
        //Calculate beat offset
        startupBeats = (songBPM / 60) * beatTravelTime;

        FindObjectOfType<Enemy_Shot_Creator>().SetAnimSpeeds(1, 1);
        #endregion
        */

        #region New Beat Offset
        startupBeats = attackTravelTimeInBeats;

        float totalTravelTime = secsPerBeat * startupBeats;

        float normalTravelTime = (51f / 60f) + beatTimingOffset;

        float animationScale = (normalTravelTime / totalTravelTime);

        attackAnimationSpeedHit = animationScale;
        attackAnimationSpeedDodge = attackAnimationSpeedHit;

        FindObjectOfType<Enemy_Shot_Creator>().SetAnimSpeeds(attackAnimationSpeedHit, attackAnimationSpeedDodge);
        #endregion

        FillNoteArray(0);
    }

    public void FillNoteArray(float currentSongTime)
    {
        noteArray = new List<noteStruct>();

        float currentSongBeat = currentSongTime * (songBPM / 60);

        //Add every note in the song pattern to be played
        for (int i = 0; i < noteParent.transform.childCount; i++)
        {
            GameObject noteObj = noteParent.transform.GetChild(i).gameObject;

            float beatToPlayOn = (noteObj.transform.localPosition.z/ 2) - startupBeats;
            if (beatToPlayOn < 0)
                beatToPlayOn = 0;

            if (beatToPlayOn < currentSongBeat) //note should have already been spawned, skip
            {
                continue;
            }

            int atkNum = noteObj.GetComponent<AttackCube>().attackNum;

            noteStruct newNote;

            if (atkNum == 10)
                newNote = new noteStruct { songEventIndex = noteObj.GetComponent<SongEvent>().songEventIndex, beatWithOffset = beatToPlayOn + startupBeats, attackNum = 10};
            else
                newNote = new noteStruct { attackNum = atkNum, beatWithOffset = beatToPlayOn };

            noteArray.Add(newNote);
        }

        noteArray.Sort((a, b) => a.beatWithOffset.CompareTo(b.beatWithOffset));

        //DEBUG PRINTING
        float howManyToPrint = 0;
        for (int i = 0; i < howManyToPrint; i++)
            Debug.Log("Beatoffset: "+noteArray[i].beatWithOffset + ", AtkNum: " + noteArray[i].attackNum);
    }

    void PlaySongZero()
    {
        PlaySong(0);
    }

    public void PlaySong(float startTime)
    {
        musicSource.Play();
        musicSource.time = startTime;

        songPlaying = true;
        dspSongTime = (float)AudioSettings.dspTime - startTime;

        heroAnim.speed = 1;
        foreach (GameObject go in enemyProjectilesToResume)
        {
            if (go != null)
                go.GetComponent<Animator>().speed = 1;
        }
        enemyProjectilesToResume = new GameObject[0];
    }

    public void PauseSong()
    {
        songPlaying = false;

        musicSource.Stop();

        enemyProjectilesToResume = GameObject.FindGameObjectsWithTag("EnemyProjectile");

        heroAnim.speed = 0;
        foreach(GameObject go in enemyProjectilesToResume)
        {
            go.GetComponent<Animator>().speed = 0;
        }
    }

    public void StopSong()
    {
        songPlaying = false;
        ClearEnemyAttacks();

        musicSource.Stop();
    }

    public void ClearEnemyAttacks()
    {
        foreach (GameObject go in enemyProjectilesToResume)
            Destroy(go);

        enemyProjectilesToResume = new GameObject[0];
    }

    public void ChangeBeatOffset(float newoffset)
    {
        //beatTravelTime = newoffset;
        StopSong();

        LoadSongPrefab(songTimingPrefab);
        PlaySong(0);
    }

    public void EnemyKilled()
    {
        GameObject[] enemyProjectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");

        //Clear noteArray
        noteArray = new List<noteStruct>();

        //Remove all existing attacks
        foreach (GameObject go in enemyProjectiles)
            Destroy(go);

        //End the battle
        endBattle.StartFadeOut(true);
    }
}
