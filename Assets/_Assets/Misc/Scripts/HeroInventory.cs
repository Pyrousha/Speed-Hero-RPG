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
    [SerializeField] private Transform keyImageParent;
    private int numKeys;

    public void OnSwordObtained()
    {
        hasSword = true;
    }

    public void OnDashObtained()
    {
        hasSpeedCrystal = true;
    }

    public void GainKey()
    {
        numKeys++;
        UpdateKeyUI();
    }

    public bool TryUseKey()
    {
        if (numKeys <= 0)
            return false;

        numKeys--;
        UpdateKeyUI();

        return true;
    }
    
    private void UpdateKeyUI()
    {
        for (int i = 0; i < keyImageParent.childCount; i++)
        {
            GameObject childObj = keyImageParent.GetChild(i).gameObject;
            childObj.SetActive(i + 1 <= numKeys);
        }
    }
}
