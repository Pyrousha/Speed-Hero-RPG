using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEditorPlaySong : MonoBehaviour
{
    [Header("Play Button")]
    public Sprite playSprite;
    public Sprite pauseSprite;

    public Image playButtonImage;

    public SongLoader songLoader;

    [Header("Camera Button")]
    public Sprite camSprite1;
    public Sprite camSprite0;

    public Image camButtonImage;

    [Header("More Stuff")]
    public GameObject[] previewCamStuff;
    public Hero_Stats_Combat heroStats;

    bool isPlaying = false;
    bool camEnabled = false;

    float bpm;
    float songPosWhenPaused = 0;

    public bool songIsPlaying;
    float songStartTime;

    GameObject cubePlaceCam;

    Vector3 camStartPosition;
    float beatPos;

    // Start is called before the first frame update
    void Start()
    {
        cubePlaceCam = transform.parent.gameObject;
        camStartPosition = transform.parent.position;

        songStartTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (songIsPlaying)
        {
            //Move camera along with notes
            beatPos = songLoader.songPositionInBeats;

            cubePlaceCam.transform.position = camStartPosition + new Vector3(0, 0, 27.68f * (beatPos/4));
        }
    }

    public void ResetSongButtonClick()
    {
        if (isPlaying)
            PlayButtonClicked();

        songStartTime = 0;

        transform.parent.position = camStartPosition;
    }


    public void PlayButtonClicked()
    {
        if (isPlaying)
        {
            //Get current song time
            songPosWhenPaused = songLoader.songPosition;

            //Stop the song
            playButtonImage.sprite = playSprite;
            songLoader.PauseSong();
            songIsPlaying = false;
        }
        else
        {
            //Set current song time based on camera position
            bpm = songLoader.songBPM;
            songStartTime = (240 * (cubePlaceCam.transform.position.z - camStartPosition.z)) / (bpm * 27.68f);
            songStartTime = Mathf.Max(0, songStartTime - songLoader.beatTravelTime);

            //Reload notes into array
            songLoader.FillNoteArray(songStartTime);

            //Start the song
            playButtonImage.sprite = pauseSprite;
            if (songPosWhenPaused != songStartTime)
                songLoader.ClearEnemyAttacks();
            songLoader.PlaySong(songStartTime);
            songIsPlaying = true;

            //Reset Hero Stats
            heroStats.SetHealth(heroStats.maxHp);
            heroStats.SetHealBar(0);

            if (!camEnabled)
                ToggleCamPreviewStuff();
        }

        isPlaying = !isPlaying;
    }

    public void ToggleCamPreviewStuff()
    {
        if (camEnabled)
        {
            //Disable Cam Preview
            camButtonImage.sprite = camSprite1;
        }
        else
        {
            //Enable Cam Preview
            camButtonImage.sprite = camSprite0;
        }

        camEnabled = !camEnabled;

        foreach (GameObject go in previewCamStuff)
            go.SetActive(camEnabled);
    }

    public bool GetCamEnabled()
    {
        return camEnabled;
    }
}
