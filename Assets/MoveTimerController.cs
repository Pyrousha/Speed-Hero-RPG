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
    private Image[] arrowImages;

    private void Start()
    {
        arrowImages = new Image[arrowLocationsParent.childCount];

        for (int i = 0; i < arrowLocationsParent.childCount; i++)
            arrowImages[i] = arrowLocationsParent.GetChild(i).GetComponent<Image>();
    }

    public void TryPerformMove(ComboAbility move)
    {
        int currMana = heroStatsCombat.GetMP;

        int manaCost = move.GetManaCost;
        if (currMana >= manaCost)
        {
            heroStatsCombat.SpendMana(manaCost);
        }
    }

    public void LoadMoveIntoSheetDisplay(ComboAbility move)
    {
        if (arrowImages == null)
            Start();

        ComboAbility.ComboInput[] comboInputs = move.GetComboInputs;
        int totalENotes = comboInputs[comboInputs.Length - 1].eigthNotesFromStart;

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
}
