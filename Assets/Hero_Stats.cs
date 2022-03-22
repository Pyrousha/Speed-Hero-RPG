using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Stats : MonoBehaviour
{
    public static Hero_Stats Instance;

    [SerializeField] private int damage;
    public int Damage => damage;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Multiple Hero_Stats found");
    }
}
