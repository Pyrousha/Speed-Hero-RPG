using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongLoader : MonoBehaviour
{
    public GameObject songToLoadPrefab;
    GameObject songToLoad;

    public gameState state;

    public GameObject noteParent;
    public GameObject noteEditorCamera;

    public AudioSource mainCameraAudioSource;

    public Enemy_Stats_Combat enemy;

    float secsPerEightNote;

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
            //Add every note in the song pattern to be played
            for(int i = 0; i < noteParent.transform.childCount; i++)
            {
                AttackCube atkCube = noteParent.transform.GetChild(i).gameObject.GetComponent<AttackCube>();
                atkCube.AddToEnemyPattern(enemy, secsPerEightNote);
            }

            //Disable note placer camera, not needed in gameplay
            noteEditorCamera.SetActive(false);

            //Start Song
            mainCameraAudioSource.PlayDelayed(1);
        }

        else //Start editor mode
        {
            noteEditorCamera.GetComponent<CubePlaceCam>().DisableGameComponents();
            noteEditorCamera.GetComponent<CubePlaceCam>().songLoadedPrefab = songToLoad;
        }
    }

    /// <summary> Spawn note objects from the specified prefab, and load song properties </summary>
    void LoadNotes()
    {
        songToLoad = Instantiate(songToLoadPrefab, transform) as GameObject;

        //Set all notes to be childed to the noteParent GameObject
        int i = 0;
        while (i < songToLoad.transform.childCount)
        {
            Transform child = songToLoad.transform.GetChild(i);
            child.parent = noteParent.transform;
        }

        //Change name of noteparent GO to the name of the song to confirm loading worked properly
        noteParent.name += ("(" + songToLoad.name + ")");

        //Calculate how fast to play notes based on song's BPM
        secsPerEightNote = (30 / songToLoad.GetComponent<SongProperties>().BPM);
        
        //Set song to play based on song from loaded prefab
        mainCameraAudioSource.clip = songToLoad.GetComponent<AudioSource>().clip;
    }
}
