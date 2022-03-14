using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatOffsetTracker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool autoUpdateAverage;
    [SerializeField] private bool printAvg;
    [SerializeField] private float maxOffsetToCountAsHit;

    [Header("Values")]
    [SerializeField] private List<float> offsets;
    [SerializeField] private float avgOffset;

    // Start is called before the first frame update
    void Start()
    {
        offsets = new List<float>();
    }

    public void AddOffsetNote(float newOffset)
    {
        offsets.Add(newOffset);
        if (autoUpdateAverage)
            avgOffset = GetAverageOffset();
    }

    public float GetMaxOffset()
    {
        return maxOffsetToCountAsHit;
    }

    public float GetAverageOffset()
    {
        float avg = 0;
        foreach (float offset in offsets)
            avg += offset;

        avg /= offsets.Count;

        if (printAvg)
            Debug.Log("AVG: " + avg);

        return avg;
    }

    public void MultiplyOffsetWindow(float multiplier)
    {
        maxOffsetToCountAsHit *= multiplier;
    }
}
