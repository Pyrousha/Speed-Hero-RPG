using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndBattle : MonoBehaviour
{
    public BattleTransition persistObj;
    public GameObject battleObjParent;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Combat-Standard"));

        if (GameObject.Find("PersistentGameInfo") != null)
        {
            persistObj = GameObject.Find("PersistentGameInfo").GetComponent<BattleTransition>();
        }
        else
        {
            Debug.Log("Unable to find PersistentGameInfo");
            //SceneManager.LoadScene("Zone1");
        }
    }


    public void EndBattleScene(bool wonFight)
    {
        if (persistObj != null)
            persistObj.TransitionFromBattle(wonFight);
    }
}
