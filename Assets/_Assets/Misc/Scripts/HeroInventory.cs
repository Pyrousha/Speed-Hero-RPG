using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInventory : Singleton<HeroInventory>
{
    [Header("Starting Values")]
    [SerializeField] private bool hasSpeedCrystal;
    public bool HasDash => hasSpeedCrystal;

    [SerializeField] private bool hasSword;
    public bool HasSword => hasSword;

    [Header("References")]
    [SerializeField] private HeroDashManager dashManager;
    [SerializeField] private PlayerSwordHandler swordHandler;

    public void OnSwordObtained()
    {
        hasSword = true;
    }

    public void OnDashObtained()
    {
        hasSpeedCrystal = true;
    }
}
