using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{
	public string sceneToLoad;
	public GameObject thingsToDisable;
	public BattleTransition persistObj;

	// Start is called before the first frame update
	void Start()
	{
		persistObj = GameObject.Find("PersistentGameInfo").GetComponent<BattleTransition>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy")
		{
			persistObj.enemyToDestroyAfterFight = other.gameObject;
			persistObj.overWorldObjParent = thingsToDisable;

			persistObj.TransitionToBattle(sceneToLoad);
		}
	}
}
