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

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            EndBattleScene();
        } */     
    }

    public void EndBattleScene()
    {
        persistObj.TransitionFromBattle();
    }
}
