using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStartingState : MonoBehaviour
{
    public enum GameState
    {
        Overworld,
        Combat,
        Menu,
        Cutscene
    }

    [Header("Game State")]
    public GameState gameState;
    public SongLoader.CombatState combatState;

    [Header("Song Properties")]
    public float beatOffset;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
