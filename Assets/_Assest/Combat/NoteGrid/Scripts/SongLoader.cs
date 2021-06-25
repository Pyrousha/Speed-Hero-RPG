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
    public Enemy_Stats_Combat enemy;
    public InputField bpmObj;
    public InputField songNameObj;


    [Header("Song Properties (To be loaded, don't change!)")]
    public float songBPM;

    public float secsPerBeat;

    public float songPosition; //current song position, in seconds

    public float songPositionInBeats;

    public float dspSongTime; //How many seconds have passed since the song started

    public AudioSource musicSource; //music source that will play music

    float secsPerEightNote;

    public float startOffset;

    public enum gameState
    { 
        NoteEditor,
        Playing
    }


    // Start is called before the first frame update
    void Start()
    {
        if (songToLoadPrefab != null)
        {
            LoadNotes();
        }

        if (state == gameState.Playing) //Player enters combat
        {
            //Disable note placer camera, not needed in gameplay
            noteEditorCamera.SetActive(false);

            //Start Song
            Invoke("PlaySong", startOffset);
        }

        else //Start editor mode
        {
            noteEditorCamera.GetComponent<CubePlaceCam>().DisableGameComponents();
            noteEditorCamera.GetComponent<CubePlaceCam>().songLoadedPrefab = songToLoad;

            heroUICanvas.worldCamera = noteEditorPreviewCam;

            bpmObj.text = songBPM.ToString();
            songNameObj.text = songToLoad.name;
        }
    }

    private void Update()
    {
        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secsPerBeat;
    }

    /// <summary> Spawn note objects from the specified prefab, and load song properties </summary>
    void LoadNotes()
    {
        songToLoad = Instantiate(songToLoadPrefab, transform) as GameObject;
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

        //Calculate how fast to play notes based on song's BPM
        secsPerEightNote = (30 / songToLoad.GetComponent<SongProperties>().BPM);
        
        //Set song to play based on song from loaded prefab
        musicSource.clip = songToLoad.GetComponent<AudioSource>().clip;
    }

    void PlaySong()
    {
        //Add every note in the song pattern to be played
        for (int i = 0; i < noteParent.transform.childCount; i++)
        {
            AttackCube atkCube = noteParent.transform.GetChild(i).gameObject.GetComponent<AttackCube>();
            atkCube.AddToEnemyPattern(enemy, secsPerEightNote, 0);
        }

        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }
}
