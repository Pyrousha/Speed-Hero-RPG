using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveTimerController : MonoBehaviour
{
    [SerializeField] private Hero_Stats_Combat heroStatsCombat;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite eighthRestSprite;
    [SerializeField] private Sprite transparentSprite;

    [SerializeField] private Transform arrowLocationsParent;
    [SerializeField] private Image songStartCircle;
    private Image[] arrowImages;

    [Header("Attack Timing")]
    [SerializeField] private bool useSongloaderOffset;
    [SerializeField] private SongLoader songLoader;
    [SerializeField] private float offset;
    private float maxOffset;
    private float closestNote;

    //private ComboAbility currAbility;
    private ComboAbility.ComboInput[] currComboInputs;
    private float[] targetBeats;
    private KeyCode[] targetInputs;
    private KeyCode nextInput;
    private int nextInputIndex;
    private float nextInputTimeEnd;
    private int totalENotes;

    [System.NonSerialized] private bool doneStart = false;

    [System.NonSerialized] private bool isDoingMove = false;
    public bool IsDoingMove => isDoingMove;

    private void Start()
    {
        if (doneStart)
            return;

        arrowImages = new Image[arrowLocationsParent.childCount];

        for (int i = 0; i < arrowLocationsParent.childCount; i++)
            arrowImages[i] = arrowLocationsParent.GetChild(i).GetComponent<Image>();

        doneStart = true;
    }

    public void CalculateOffset()
    {
        float BPM = songLoader.SongBPM;

        if (useSongloaderOffset)
            maxOffset = songLoader.gameObject.GetComponent<BeatOffsetTracker>().GetMaxOffset() * (BPM / 60f);
        else
            maxOffset = offset * (BPM / 60f);
    }

    private void Update()
    {
        if (isDoingMove)
        {
            float songPosBeats = songLoader.SongPositionInBeats;
            float beatsFromStart = songPosBeats - closestNote;
            float xPos = beatsFromStart * -40f;

            arrowLocationsParent.transform.localPosition = new Vector3(xPos, 0, 0);

            if (Input.GetKeyDown(nextInput) && (CheckInputTimeToSongPosition(targetBeats[nextInputIndex])))
            {
                //correct input
                SetArrowColor(nextInputIndex, Color.green);

                nextInputIndex++;
                if (nextInputIndex < targetInputs.Length)
                {
                    nextInput = targetInputs[nextInputIndex];
                    nextInputTimeEnd = targetBeats[nextInputIndex] + maxOffset / 2;
                }
                else
                    MoveDone();
            }

            if (songPosBeats > nextInputTimeEnd)
            {
                //Missed note
                SetArrowColor(nextInputIndex, Color.red);

                nextInputIndex++;

                if (nextInputIndex < targetInputs.Length)
                {
                    nextInput = targetInputs[nextInputIndex];
                    nextInputTimeEnd = targetBeats[nextInputIndex] + maxOffset / 2;
                }
                else
                    MoveDone();
            }
        }
    }

    void SetArrowColor(int index, Color col)
    {
        int numArrowsToSkip = index;
        for(int i = 0; i< arrowImages.Length; i++)
        {
            Image currImage = arrowImages[i];
            if (currImage.sprite == arrowSprite)
            {
                if (numArrowsToSkip <= 0)
                {
                    currImage.color = col;
                    return;
                }
                else
                    numArrowsToSkip--;
            }
        }
    }

    public void TryPerformMove(ComboAbility move)
    {
        ResetArrowColors();

        int currMana = heroStatsCombat.GetMP;

        int manaCost = move.GetManaCost;
        if ((currMana >= manaCost) && (IsInputOnBeat(8)))
        {
            isDoingMove = true;

            songStartCircle.color = Color.green;

            //currAbility = move;
            currComboInputs = move.GetComboInputs;
            targetBeats = new float[currComboInputs.Length];
            targetInputs = new KeyCode[currComboInputs.Length];

            for (int i = 0; i < currComboInputs.Length; i++)
            {
                ComboAbility.ComboInput currInput = currComboInputs[i];
                targetBeats[i] = closestNote + (currInput.eigthNotesFromStart * 0.5f);
                targetInputs[i] = AttackDirToKeycode(currInput.attackDirection);
            }

            nextInput = targetInputs[0];
            nextInputIndex = 0;
            nextInputTimeEnd = targetBeats[0] + maxOffset / 2;

            heroStatsCombat.SpendMana(manaCost);
        }
    }

    private void MoveDone()
    {
        isDoingMove = false;
        currComboInputs = null;

        arrowLocationsParent.transform.localPosition = new Vector3(0, 0, 0);
        //ResetArrowColors();
    }

    private void ResetArrowColors()
    {
        songStartCircle.color = Color.white;
        foreach (Image img in arrowImages)
            img.color = Color.white;
    }
        
    public void LoadMoveIntoSheetDisplay(ComboAbility move)
    {
        if (arrowImages == null)
            Start();

        ComboAbility.ComboInput[] comboInputs = move.GetComboInputs;
        totalENotes = comboInputs[comboInputs.Length - 1].eigthNotesFromStart;

        Sprite[] spritesToUse = new Sprite[totalENotes];
        int[] rotations = new int[totalENotes];

        for (int i=0; i< comboInputs.Length; i++)
        {
            ComboAbility.ComboInput currInput = comboInputs[i];
            int index = currInput.eigthNotesFromStart - 1;

            spritesToUse[index] = arrowSprite;
            rotations[index] = AttackDirToDegrees(currInput.attackDirection);
        }

        for (int i = 0; i < spritesToUse.Length; i++)
        {
            if (spritesToUse[i] == null)
                spritesToUse[i] = eighthRestSprite;
        }

        for (int i = 0; i< arrowImages.Length; i++)
        {
            Image currArrow = arrowImages[i];

            if (i < spritesToUse.Length)
            {
                currArrow.sprite = spritesToUse[i];
                currArrow.transform.eulerAngles = new Vector3(0, 0, rotations[i]);
                if (currArrow.sprite == eighthRestSprite)
                    currArrow.transform.localScale = new Vector3(0.75f, 0.75f, 1);
                else
                    currArrow.transform.localScale = Vector3.one;
            }
            else
            {
                currArrow.sprite = transparentSprite;
                currArrow.transform.eulerAngles = Vector3.zero;
                currArrow.transform.localScale = new Vector3(0.75f, 0.75f, 1);
            }
        }

        ResetArrowColors();
    }

    public int AttackDirToDegrees(ComboAbility.attackDir dir)
    {
        switch (dir)
        {
            case ComboAbility.attackDir.Left:
                return 180;
            case ComboAbility.attackDir.DiagLeft:
                return 135;
            case ComboAbility.attackDir.Up:
                return 90;
            case ComboAbility.attackDir.DiagRight:
                return 45;
            case ComboAbility.attackDir.Right:
                return 0;
        }

        Debug.Log("INVALID ATTACKDIR PASSED IN: " + dir);
        return (-1);
    }

    public KeyCode AttackDirToKeycode(ComboAbility.attackDir dir)
    {
        switch (dir)
        {
            case ComboAbility.attackDir.Left:
                return KeyCode.A;
            case ComboAbility.attackDir.DiagLeft:
                return KeyCode.Q;
            case ComboAbility.attackDir.Up:
                return KeyCode.W;
            case ComboAbility.attackDir.DiagRight:
                return KeyCode.E;
            case ComboAbility.attackDir.Right:
                return KeyCode.D;
        }

        Debug.Log("INVALID ATTACKDIR PASSED IN: " + dir);
        return KeyCode.Escape;
    }

    public bool IsInputOnBeat(int notesPerBeat)
    {
        float currSongPosInBeats = songLoader.SongPositionInBeats;
        float locationInBeat = currSongPosInBeats - Mathf.Floor(currSongPosInBeats);

        if (locationInBeat <= (maxOffset / 2)) //within first quarter note
        {
            closestNote = (int) currSongPosInBeats;
            return true;
        }

        if (locationInBeat >= (1 - (maxOffset / 2))) //within second quarter note
        {
            closestNote = (int)currSongPosInBeats + 1;
            return true;
        }

        if (notesPerBeat == 8)
        {
            locationInBeat = Mathf.Abs(locationInBeat - 0.5f);
            if (locationInBeat <= maxOffset / 2) //within eigth notes
            {
                closestNote = (int)currSongPosInBeats + 0.5f;
                return true;
            }
        }

        closestNote = -1000;
        return false;
    }

    public bool CheckInputTimeToSongPosition(float targetBeat)
    {
        float currSongPosInBeats = songLoader.SongPositionInBeats;
        float offsetAhead = currSongPosInBeats - targetBeat;

        if (Mathf.Abs(offsetAhead) <= maxOffset / 2)
        {
            //Hit within timing
            return true;
        }

        return false;
    }
}
