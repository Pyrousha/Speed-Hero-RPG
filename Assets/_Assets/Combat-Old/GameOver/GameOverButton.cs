using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButton : MonoBehaviour
{
    BattleTransition persistObj;

    // Start is called before the first frame update
    void Start()
    {
        persistObj = GameObject.Find("PersistentGameInfo")?.GetComponent<BattleTransition>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Retry();
        if (Input.GetKeyDown(KeyCode.E))
            ExitCombat();
    }

    public void Retry()
    {
        if (SceneManager.GetSceneByName("Zone1").isLoaded)
        {
            SceneManager.LoadScene("Combat-Standard", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("DEAD");
        }
        else
        {
            SceneManager.LoadScene("Combat-Standard", LoadSceneMode.Single);
        }

    }

    public void ExitCombat()
    {
        if (persistObj == null)
            Retry();
        else
        {
            persistObj.SetBattleScene("DEAD");
            persistObj.TransitionFromBattle(false);
        }
    }
}
