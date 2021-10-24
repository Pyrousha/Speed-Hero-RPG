using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroMenuMove : MonoBehaviour
{
    [SerializeField] private Transform arrowLocation;
    [SerializeField] private Text moveNameText;
    [SerializeField] private Text mpCostText;
    private int mpCost;
    private ComboAbility comboAbility;

    public Transform ArrowLocation => arrowLocation; 

    public void LoadMove(ComboAbility newMove)
    {
        comboAbility = newMove;

        //Set Text
        moveNameText.text = newMove.name;

        //Set Mana Cost
        mpCost = newMove.GetManaCost;

        if (mpCost > 0)
            mpCostText.text = mpCost.ToString();
        else
            mpCostText.text = "";
    }

    public ComboAbility GetComboAbility()
    {
        if (comboAbility != null)
            return comboAbility;

        return null;
    }

    public void UpdateCanUse(int currMp, Color canUseColor, Color cantUseColor)
    {
        if (currMp >= mpCost)
        {
            //Player has enough MP to use
            moveNameText.color = canUseColor;
            mpCostText.color = canUseColor;
        }
        else
        {
            //Player does not have enough MP
            moveNameText.color = cantUseColor;
            mpCostText.color = cantUseColor;
        }

    }
}
