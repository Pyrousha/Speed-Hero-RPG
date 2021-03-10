using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBattle : MonoBehaviour
{
    public BattleTransition persistObj;
    public GameObject battleObjParent;

    // Start is called before the first frame update
    void Start()
    {
        persistObj = GameObject.Find("PersistentGameInfo").GetComponent<BattleTransition>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndBattleScene();
        }      
    }

    public void EndBattleScene()
    {
        persistObj.battleObjParent = battleObjParent;
        persistObj.TransitionFromBattle();
    }
}
