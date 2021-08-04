using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{
	public string sceneToLoad;
	public GameObject thingsToDisable;
	public GameObject enemyToDestroy;
	public BattleTransition persistObj;

	// Start is called before the first frame update
	void Start()
	{
		persistObj = GameObject.Find("PersistentGameInfo").GetComponent<BattleTransition>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			persistObj.enemyToDestroyAfterFight = enemyToDestroy;
			persistObj.overWorldObjParent = thingsToDisable;

			persistObj.TransitionToBattle(sceneToLoad);
		}
	}
}
