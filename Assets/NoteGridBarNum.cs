using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteGridBarNum : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Bar this obj starts on
        float startingBarNum = (transform.parent.localPosition.z + 0.5f) / 8f;

        //Set children's text to be the bar# + offset
        for(int i = 0; i<transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Text>().text = (startingBarNum + i).ToString();
        }
    }
}
