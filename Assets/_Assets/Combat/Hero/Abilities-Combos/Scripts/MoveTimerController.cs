using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveTimerController : MonoBehaviour
{
    [Header("Mechanics/Config")]
    [SerializeField] private bool useWASDForAttackInput;
    [SerializeField] private bool useComboAbilityTimings;
    [SerializeField] private bool misinputsBreakCombo;
    [SerializeField] private bool mistimingBreaksCombo;
    [SerializeField] private bool dontInterruptComboWhenInvincible;
    [SerializeField] private bool dealAllDamageAtEnd;
    [SerializeField] private bool pressSpaceAtEndToFinishCombo;
    [SerializeField] private bool hasEndlagOnAttack;
    [SerializeField] private float quarterNotesOfEndlag;
    private int totalDamage;
    private float beatEndlagEnds;

    //Controls
    private KeyCode attackInputA;
    private KeyCode attackInputQ;
    private KeyCode attackInputW;
    private KeyCode attackInputE;
    private KeyCode attackInputD;
    private KeyCode attackInputSpace;

    [Header("Objects/References")]
    [SerializeField] private Hero_Stats_Combat heroStatsCombat;
    [SerializeField] private Hero_Controller_Combat heroControllerCombat;
    [SerializeField] private Enemy_Stats_Combat enemyStatsCombat;

    [SerializeField] private Transform arrowLocationsParent;
    private Image[] arrowImages;
    [SerializeField] private Image songStartCircle;
    [SerializeField] private Transform attackdirHighlightsParent;
    private MeshRenderer[] attackdirHighlights;

    [Header("Materials")]
    [SerializeField] private Material unlitArrowMat;
    [SerializeField] private Material greenArrowMat;
    [SerializeField] private Material redArrowMat;

    [Header("Sprites")]
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite eighthRestSprite;
    [SerializeField] private Sprite transparentSprite;

    [Header("Attack Timing")]
    [SerializeField] private SongLoader songLoader;
    [SerializeField] private float offset;
    [SerializeField] private bool overrideOffsetWithSongloaderValue;
    private float maxOffset;
    private float closestNote;
    private float lastInputNote; //used to make sure player doesn't input twice in same quarter/eigth note

    //private ComboAbility currAbility;
    private ComboAbility.ComboInput[] currComboInputs;
    private float[] targetBeats;
    private KeyCode[] targetInputs;
    private KeyCode nextInput;
    private int nextInputIndex;
    private float nextInputTimeEnd;
    private int totalENotes;

    [System.NonSerialized] private bool doneStart = false;

    public enum attackMoveStateEnum
    {
        idle,
        attacking,
        endlag
    }

    //[System.NonSerialized] private bool isDoingMove = false;
    [System.NonSerialized] private attackMoveStateEnum attackMoveState = attackMoveStateEnum.idle;

    public attackMoveStateEnum AttackMoveState => attackMoveState;

    private void Start()
    {
        if (doneStart)
            return;

        //Set input vars
        if(useWASDForAttackInput)
        {
            attackInputA = KeyCode.A;
            attackInputQ = KeyCode.Q;
            attackInputW = KeyCode.W;
            attackInputE = KeyCode.E;
            attackInputD = KeyCode.D;
            attackInputSpace = KeyCode.Insert;
        }
        else
        {
            attackInputA = KeyCode.J;
            attackInputQ = KeyCode.U;
            attackInputW = KeyCode.I;
            attackInputE = KeyCode.O;
            attackInputD = KeyCode.L;
            attackInputSpace = KeyCode.Space;
        }

        //Fill attackDirHighlights
        attackdirHighlights = new MeshRenderer[attackdirHighlightsParent.childCount];
        for (int i = 0; i < attackdirHighlightsParent.childCount; i++)
            attackdirHighlights[i] = attackdirHighlightsParent.GetChild(i).GetComponent<MeshRenderer>();

        //Fill arrowImages array
        arrowImages = new Image[arrowLocationsParent.childCount];
        for (int i = 0; i < arrowLocationsParent.childCount; i++)
            arrowImages[i] = arrowLocationsParent.GetChild(i).GetComponent<Image>();

        doneStart = true;
    }

    public void CalculateOffset()
    {
        float BPM = songLoader.SongBPM;

        if (overrideOffsetWithSongloaderValue)
            maxOffset = songLoader.gameObject.GetComponent<BeatOffsetTracker>().GetMaxOffset() * (BPM / 60f);
        else
            maxOffset = offset * (BPM / 60f);
    }

    public void TryStartMove(ComboAbility move)
    {
        if (attackMoveState != attackMoveStateEnum.idle)
            return;

        ResetArrowColors();

        int currMana = heroStatsCombat.GetMP;

        int manaCost = move.GetManaCost;
        if ((currMana >= manaCost) && (IsInputOnBeat(8)))
        {
            attackMoveState = attackMoveStateEnum.attacking;

            songStartCircle.color = Color.green;

            ComboAbility.ComboInput[] loadedComboInputs = move.GetComboInputs;

            //Load comboinputs from comboability
            if (pressSpaceAtEndToFinishCombo)
            {
                //Add another input (space) at end of combo
                currComboInputs = new ComboAbility.ComboInput[loadedComboInputs.Length + 1];
                for (int i = 0; i < loadedComboInputs.Length; i++)
                {
                    currComboInputs[i] = loadedComboInputs[i];
                }

                //Press space, deals no damage
                currComboInputs[currComboInputs.Length - 1] = new ComboAbility.ComboInput
                { 
                    attackDirection = ComboAbility.attackDir.Space, 
                    damageAfterPressed = 0, 
                    eigthNotesFromStart = loadedComboInputs[loadedComboInputs.Length - 1].eigthNotesFromStart 
                };
            }
            else
            {
                currComboInputs = loadedComboInputs;
            }

            //Setup input and beats arrays
            targetBeats = new float[currComboInputs.Length];
            targetInputs = new KeyCode[currComboInputs.Length];

            //Load comboability
            for (int i = 0; i < currComboInputs.Length; i++)
            {
                ComboAbility.ComboInput currInput = currComboInputs[i];
                targetBeats[i] = closestNote + (currInput.eigthNotesFromStart * 0.5f);
                targetInputs[i] = AttackDirToKeycode(currInput.attackDirection);
            }
            if (pressSpaceAtEndToFinishCombo)
            {
                //Last move in combo should be pressing space/ins again
                targetInputs[targetInputs.Length - 1] = attackInputSpace;
            }

            //First thing the player needs to do
            nextInput = targetInputs[0];
            nextInputIndex = 0;
            nextInputTimeEnd = targetBeats[0] + maxOffset / 2;
            lastInputNote = closestNote;
            totalDamage = 0;

            if (!useComboAbilityTimings)
                UpdateNextAttackArrowIndicator(nextInput);

            heroStatsCombat.SpendMana(manaCost);
        }
    }

    private void Update()
    {
        if (attackMoveState == attackMoveStateEnum.attacking)
        {
            if (useComboAbilityTimings)
                CheckInputsForTimedCombos();
            else
                CheckInputsForUntimedCombos();

            return;
        }

        if (attackMoveState == attackMoveStateEnum.endlag)
        {
            float songPosBeats = songLoader.SongPositionInBeats;
            if (songPosBeats >= beatEndlagEnds)
            {
                //Endlag is over
                EndEndlag();
            }
        }
    }

    private void CheckInputsForTimedCombos()
    {
        float songPosBeats = songLoader.SongPositionInBeats;
        float beatsFromStart = songPosBeats - closestNote;
        float xPos = beatsFromStart * -40f;

        arrowLocationsParent.transform.localPosition = new Vector3(xPos, 0, 0);

        if (Input.GetKeyDown(nextInput) && (CheckInputTimeToSongPosition(targetBeats[nextInputIndex])))
        {
            //correct input
            SetArrowColor(nextInputIndex, Color.green);
            enemyStatsCombat.TakeDamage(currComboInputs[nextInputIndex].damageAfterPressed);

            nextInputIndex++;
            if (nextInputIndex < targetInputs.Length)
            {
                nextInput = targetInputs[nextInputIndex];
                nextInputTimeEnd = targetBeats[nextInputIndex] + maxOffset / 2;
            }
            else
                MoveDone(true);
        }

        if (songPosBeats > nextInputTimeEnd)
        {
            //Missed note
            SetArrowColor(nextInputIndex, Color.red);
            int dmgToDeal = Mathf.CeilToInt(currComboInputs[nextInputIndex].damageAfterPressed / 4f);
            enemyStatsCombat.TakeDamage(dmgToDeal);

            nextInputIndex++;
            if (nextInputIndex < targetInputs.Length)
            {
                nextInput = targetInputs[nextInputIndex];
                nextInputTimeEnd = targetBeats[nextInputIndex] + maxOffset / 2;
            }
            else
                MoveDone(true);
        }
    }

    private void CheckInputsForUntimedCombos()
    {
        if (Input.GetKeyDown(nextInput))
        {
            if (IsInputOnBeat(8) && (closestNote != lastInputNote))
            {
                //correct input
                lastInputNote = closestNote;
                
                SetArrowColor(nextInputIndex, Color.green);

                int dmgToDeal = currComboInputs[nextInputIndex].damageAfterPressed;
                DealDamageAndIncrementUntimed(dmgToDeal, false);
            }
            else
            {
                //Input was not on-beat
                SetArrowColor(nextInputIndex, Color.red);

                int dmgToDeal = Mathf.CeilToInt(currComboInputs[nextInputIndex].damageAfterPressed / 4f);
                DealDamageAndIncrementUntimed(dmgToDeal, mistimingBreaksCombo);
            }

            return;
        }

        if (IncorrectInputPressed(nextInput))
        {
            //Wrong note pressed
            SetArrowColor(nextInputIndex, Color.red);

            int dmgToDeal = Mathf.CeilToInt(currComboInputs[nextInputIndex].damageAfterPressed / 4f);
            DealDamageAndIncrementUntimed(dmgToDeal, misinputsBreakCombo);
        }

        void DealDamageAndIncrementUntimed(int dmg, bool shouldEndCombo)
        {
            totalDamage += dmg;
            if (!dealAllDamageAtEnd)
                enemyStatsCombat.TakeDamage(dmg);

            if (shouldEndCombo)
            {
                MoveDone(false);
                return;
            }

            nextInputIndex++;
            if (nextInputIndex < targetInputs.Length)
            {
                nextInput = targetInputs[nextInputIndex];
                UpdateNextAttackArrowIndicator(nextInput);
            }
            else //last input in the combo was just performed
            {
                if (dealAllDamageAtEnd)
                    enemyStatsCombat.TakeDamage(totalDamage);
                MoveDone(true);
            }
        }
    }

    private bool IncorrectInputPressed(KeyCode nextAttackInput)
    {
        if (nextAttackInput == attackInputA)
        {
            if (Input.GetKeyDown(attackInputQ) || Input.GetKeyDown(attackInputW)
                || Input.GetKeyDown(attackInputE) || Input.GetKeyDown(attackInputD))
                return true;
        }

        if (nextAttackInput == attackInputQ)
        {
            if (Input.GetKeyDown(attackInputA) || Input.GetKeyDown(attackInputW)
                || Input.GetKeyDown(attackInputE) || Input.GetKeyDown(attackInputD))
                return true;
        }

        if (nextAttackInput == attackInputW)
        {
            if (Input.GetKeyDown(attackInputA) || Input.GetKeyDown(attackInputQ)
                || Input.GetKeyDown(attackInputE) || Input.GetKeyDown(attackInputD))
                return true;
        }

        if (nextAttackInput == attackInputE)
        {
            if (Input.GetKeyDown(attackInputA) || Input.GetKeyDown(attackInputQ)
                || Input.GetKeyDown(attackInputW) || Input.GetKeyDown(attackInputD))
                return true;
        }

        if (nextAttackInput == attackInputD)
        {
            if (Input.GetKeyDown(attackInputA) || Input.GetKeyDown(attackInputQ)
                || Input.GetKeyDown(attackInputW) || Input.GetKeyDown(attackInputE))
                return true;
        }

        return false;
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

    private void UpdateNextAttackArrowIndicator(KeyCode nextAttackInput)
    {
        ResetArrowIndicatorMaterials();

        MeshRenderer nextInputArrow = GetNextInputArrowRenderer(nextAttackInput);
        if (nextInputArrow != null)
            nextInputArrow.material = greenArrowMat;
    }

    private void ResetArrowIndicatorMaterials()
    {
        foreach (MeshRenderer rend in attackdirHighlights)
            rend.material = unlitArrowMat;
    }

    public void HeroTakeDamage(bool isHeroInvincible)
    {
        if (isHeroInvincible && dontInterruptComboWhenInvincible)
            return;

        if (attackMoveState == attackMoveStateEnum.attacking)
            MoveDone(false);
    }

    private void MoveDone(bool comboCompleted)
    {
        ResetArrowIndicatorMaterials();

        if (hasEndlagOnAttack && comboCompleted)
            StartEndlag();
        else
            attackMoveState = attackMoveStateEnum.idle;

        currComboInputs = null;

        arrowLocationsParent.transform.localPosition = new Vector3(0, 0, 0);

        songStartCircle.color *= 0.5f;
        foreach (Image img in arrowImages)
        {
            if (img.sprite == arrowSprite)
                img.color *= 0.5f;
        }

        //ResetArrowColors();
    }

    private void StartEndlag()
    {
        beatEndlagEnds = closestNote + quarterNotesOfEndlag;
        attackMoveState = attackMoveStateEnum.endlag;

        GetNextInputArrowRenderer(attackInputSpace).material = redArrowMat;

        heroControllerCombat.SetCanAttack(false);
    }

    private void EndEndlag()
    {
        attackMoveState = attackMoveStateEnum.idle;

        GetNextInputArrowRenderer(attackInputSpace).material = unlitArrowMat;

        heroControllerCombat.SetCanAttack(true);
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
                return attackInputA;
            case ComboAbility.attackDir.DiagLeft:
                return attackInputQ;
            case ComboAbility.attackDir.Up:
                return attackInputW;
            case ComboAbility.attackDir.DiagRight:
                return attackInputE;
            case ComboAbility.attackDir.Right:
                return attackInputD;
            case ComboAbility.attackDir.Space:
                return attackInputSpace;
        }

        Debug.Log("INVALID ATTACKDIR PASSED IN: " + dir);
        return KeyCode.Escape;
    }

    private MeshRenderer GetNextInputArrowRenderer(KeyCode nextInput)
    {
        if (nextInput == attackInputA)
            return attackdirHighlights[0];
        if (nextInput == attackInputQ)
            return attackdirHighlights[1];
        if (nextInput == attackInputW)
            return attackdirHighlights[2];
        if (nextInput == attackInputE)
            return attackdirHighlights[3];
        if (nextInput == attackInputD)
            return attackdirHighlights[4];
        if (nextInput == attackInputSpace)
            return attackdirHighlights[5];

        return null;
    }

    public bool IsInputOnBeat(int notesPerBeat)
    {
        // 4 = hit on quarter notes
        // 8 = hit on eigth notes

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
