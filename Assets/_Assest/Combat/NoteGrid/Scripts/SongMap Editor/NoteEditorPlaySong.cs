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

    [Header("Camera Button")]
    public Sprite camSprite1;
    public Sprite camSprite0;

    public Image camButtonImage;

    public GameObject[] previewCamStuff;

    bool isPlaying = false;
    bool camEnabled = false;

    Vector3 camStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        camStartPosition = transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        else
        {
            //Start the song
            playButtonImage.sprite = pauseSprite;
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
