using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{
    public enum BattleType
    { 
		standard
	}

	[Header("Battle Properties")]
	[SerializeField] private BattleType battleType;
	[SerializeField] private GameObject songPrefab;
	[SerializeField] private EnemyObject enemyObject;

	[Header("Overworld Properties + Events")]
	[SerializeField] private bool startFightOnTrigger;

	[SerializeField] private UnityEvent combatWinEvent;
	[SerializeField] private UnityEvent combatExitEvent;

	private BattleTransition persistObj;
	private CombatStartingState combatStartingState;
	private Collider combatStartCollider;

	// Start is called before the first frame update
	void Start()
	{
		persistObj = GameObject.Find("PersistentGameInfo").GetComponent<BattleTransition>();
		combatStartingState = persistObj.GetComponent<CombatStartingState>();

		combatStartCollider = GetComponent<Collider>();
	}

	void OnTriggerEnter(Collider other)
	{
		if ((startFightOnTrigger) && (other.gameObject.layer == 16)) //player layer
		{
			StartBattle();
		}
	}

	public void StartBattle()
    {
		combatStartingState.songPrefab = songPrefab;
		combatStartingState.enemyObject = enemyObject;

		persistObj.TransitionToBattle(battleType, this);

		if (combatStartCollider != null)
			combatStartCollider.enabled = false;
	}

	public void EndBattle(bool wonFight)
    {
		if (wonFight)
			combatWinEvent.Invoke();
		else
			combatExitEvent.Invoke();
    }
}
