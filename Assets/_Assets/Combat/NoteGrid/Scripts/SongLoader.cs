using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SongLoader : MonoBehaviour
{
    [Header("Game State")]
    public gameState state;

    [Header("Song Prefab")]
    public GameObject songToLoadPrefab;
    GameObject songToLoad;

    [Header("Objects + Stuff")]
    public GameObject noteParent;
    public GameObject noteEditorCamera;
    public Camera noteEditorPreviewCam;
    public Canvas heroUICanvas;
    public EndBattle endBattle;
    public Enemy_Stats_Combat enemy;
    public Hero_Stats_Combat heroStatsCombat;
    public Animator heroAnim;
    public InputField bpmObj;
    public InputField songNameObj;

    public GameObject[] enemyProjectilesToResume = new GameObject[0];


    [Header("Song Properties (To be loaded, don't change!)")]
    public float songBPM;

    public float secsPerBeat;

    public float songPosition; //current song position, in seconds

    public float songPositionInBeats;

    public float dspSongTime; //How many seconds have passed since the song started

    public AudioSource musicSource; //music source that will play music

    bool songPlaying;


    [Header("Timing + Settings")]
    public float songStartOffset;
    public float beatTravelTime;
    public GameObject songTimingPrefab;
    public GameObject[] thingsToToggleEnableForSettingOffset;
    float startupBeats; //How many beats before should a note be spawned (dependent on beatTravelTime and song BPM)
    public List<noteStruct> noteArray;

    public enum gameState
    { 
        NoteEditor,
        Playing,
        BeatOffset
    }

    public struct noteStruct
    {
        public float beatWithOffset;
        public int attackNum;
    }


    // Start is called before the first frame update
    void Start()
    {
        songPlaying = false;

        switch (state)
        {
            case (gameState.Playing):
                {
                    //Load notes
                    LoadSongPrefab(songToLoadPrefab);

                    //Disable note placer camera, not needed in gameplay
                    noteEditorCamera.SetActive(false);

                    //Start Song
                    Invoke("PlaySongZero", songStartOffset);
                    break;
                }
            case (gameState.NoteEditor):
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
            case (gameState.BeatOffset):
                {
                    //Disable note placer camera, not needed in gameplay
                    noteEditorCamera.SetActive(false);

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

    private void Update()
    {
        if (songPlaying)
        {
            //determine how many seconds since the song started
            songPosition = (float)(AudioSettings.dspTime - dspSongTime);

            //determine how many beats since the song started
            songPositionInBeats = songPosition / secsPerBeat;

            if (noteArray.Count > 0)
            {
                if (songPositionInBeats >= noteArray[0].beatWithOffset)
                {
                    enemy.EnemyStartAttack(noteArray[0].attackNum);
                    noteArray.RemoveAt(0);
                }
            }
            else
            {
                if (songPosition > musicSource.clip.length)
                {
                    songPlaying = false;

                    if (state == gameState.BeatOffset)
                    {
                        //Restart song
                        FillNoteArray(0);
                        PlaySong(0);
                    }
                    else
                    {
                        //End the battle
                        endBattle.EndBattleScene();
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

        songToLoad = Instantiate(songPrefab, transform) as GameObject;
        songToLoad.name = songToLoad.name.Replace("(Clone)", "");

        //Set all notes to be childed to the noteParent GameObject
        int i = 0;
        while (i < songToLoad.transform.childCount)
        {
            Transform child = songToLoad.transform.GetChild(i);
            child.parent = noteParent.transform;
        }

        //Change name of noteparent GO to the name of the song to confirm loading worked properly
        noteParent.name += ("(" + songToLoad.name + ")");

        songBPM = songToLoad.GetComponent<SongProperties>().BPM;
        secsPerBeat = 60f / songBPM;
        
        //Set song to play based on song from loaded prefab
        musicSource.clip = songToLoad.GetComponent<AudioSource>().clip;

        //Calculate beat offset
        startupBeats = (songBPM / 60) * beatTravelTime;

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
            if (beatToPlayOn < currentSongBeat) //note should have alreayd been spawned, skip
                continue;

            int atkNum = noteObj.GetComponent<AttackCube>().attackNum;

            noteStruct newNote = new noteStruct { attackNum = atkNum, beatWithOffset = beatToPlayOn };
            noteArray.Add(newNote);
        }

        noteArray.Sort((a, b) => a.beatWithOffset.CompareTo(b.beatWithOffset));

        //DEBUG PRINTING
        //for (int i = 0; i < noteArray.Count; i++)
            //Debug.Log("Beatoffset: "+noteArray[i].beatWithOffset + ", AtkNum: " + noteArray[i].attackNum);
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
        {
            Destroy(go);
        }
        enemyProjectilesToResume = new GameObject[0];
    }

    public void ChangeBeatOffset(float newoffset)
    {
        beatTravelTime = newoffset;
        StopSong();

        LoadSongPrefab(songTimingPrefab);
        PlaySong(0);
    }
}
