using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatOffsetSlider : MonoBehaviour
{
    public Slider slider;
    public SongLoader songLoader;

    private void Start()
    {
        slider.value = songLoader.beatTravelTime;
    }

    public void ChangeBeatOffset()
    {
        songLoader.ChangeBeatOffset(slider.value);
    }
}
