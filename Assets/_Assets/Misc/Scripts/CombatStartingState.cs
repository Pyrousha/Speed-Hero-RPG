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

    [Header("Combat Properties")]
    [HideInInspector] public GameObject songPrefab;
    [HideInInspector] public EnemyObject enemyObject;
}
