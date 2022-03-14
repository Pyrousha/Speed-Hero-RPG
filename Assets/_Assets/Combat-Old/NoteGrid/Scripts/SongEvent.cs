using UnityEngine;
using UnityEngine.Events;

public class SongEvent : MonoBehaviour
{
    public int songEventIndex;
    private SongEventHandler songEventHandler = null;
    private SpriteRenderer arrowSprite = null;

    public void Awake()
    {
        if (arrowSprite == null)
            arrowSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        SetIndex(songEventIndex);
    }

    public void SetIndex(int newIndex)
    {
        songEventIndex = newIndex;

        if (songEventHandler == null)
            songEventHandler = FindObjectOfType<SongEventHandler>();

        float hue = (float)songEventIndex / (float)songEventHandler.songEvents.Length;
        arrowSprite.color = Color.HSVToRGB(hue, 1, 1);
    }

    public void IncrementIndex()
    {
        if (songEventHandler == null)
            songEventHandler = FindObjectOfType<SongEventHandler>();
        songEventIndex = (songEventIndex + 1) % songEventHandler.songEvents.Length;

        float hue = (float)songEventIndex / (float)songEventHandler.songEvents.Length;
        arrowSprite.color = Color.HSVToRGB(hue, 1, 1);
    }
}
