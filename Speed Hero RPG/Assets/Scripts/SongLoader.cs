using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongLoader : MonoBehaviour
{
    public GameObject noteParent;
    public GameObject songToLoadPrefab;
    public GameObject noteEditorCamera;

    public AudioSource mainCameraAudioSource;

    public Enemy_Stats_Combat enemy;

    public float secsPerEightNote;

    public enum gameState
    { 
        NoteEditor,
        Playing
    }

    public gameState state;


    // Start is called before the first frame update
    void Start()
    {
        if (songToLoadPrefab != null)
        {
            LoadNotes();
        }

        if (state == gameState.Playing) //Player enters combat
        {
            for(int i = 0; i < noteParent.transform.childCount; i++)
            {
                AttackCube atkCube = noteParent.transform.GetChild(i).gameObject.GetComponent<AttackCube>();
                atkCube.AddToEnemyPattern(enemy, secsPerEightNote);
            }

            noteEditorCamera.SetActive(false);

            //Start Song
            mainCameraAudioSource.PlayDelayed(1);
        }

        else //Start editor mode
        {
            noteEditorCamera.GetComponent<CubePlaceCam>().DisableGameComponents();
        }
    }

    void LoadNotes()
    {
        GameObject songToLoad = Instantiate(songToLoadPrefab, transform) as GameObject;
        int i = 0;
        while (i < songToLoad.transform.childCount)
        {
            Transform child = songToLoad.transform.GetChild(i);
            child.parent = noteParent.transform;
        }

        noteParent.name += ("(" + songToLoad.name + ")");

        secsPerEightNote = (30 / songToLoad.GetComponent<SongProperties>().BPM);
        mainCameraAudioSource.clip = songToLoad.GetComponent<AudioSource>().clip;

        Destroy(songToLoad);
    }
}
