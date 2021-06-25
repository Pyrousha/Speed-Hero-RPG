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

    public bool songIsPlaying;

    GameObject cubePlaceCam;

    Vector3 camStartPosition;
    float beatPos;

    // Start is called before the first frame update
    void Start()
    {
        cubePlaceCam = transform.parent.gameObject;
        camStartPosition = transform.parent.position;
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
        transform.parent.position = camStartPosition;
    }


    public void PlayButtonClicked()
    {
        if (isPlaying)
        {
            //Stop the song
            playButtonImage.sprite = playSprite;
            songLoader.StopSong();
            songIsPlaying = false;
        }
        else
        {
            //Start the song
            playButtonImage.sprite = pauseSprite;
            songLoader.PlaySong();
            songIsPlaying = true;

            //Reset Hero Stats
            heroStats.SetHealth(heroStats.maxHp);
            heroStats.SetHealBar(0);

            if (!camEnabled)
                TogglePreviewStuff();
        }

        isPlaying = !isPlaying;
    }

    public void TogglePreviewStuff()
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
