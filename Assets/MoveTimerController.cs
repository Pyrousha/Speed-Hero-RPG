using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTimerController : MonoBehaviour
{
    [SerializeField] private Hero_Stats_Combat heroStatsCombat;

    public void TryPerformMove(ComboAbility move)
    {
        int currMana = heroStatsCombat.GetMP;

        int manaCost = move.GetManaCost;
        if (currMana >= manaCost)
        {
            heroStatsCombat.SpendMana(manaCost);
        }
    }
}
