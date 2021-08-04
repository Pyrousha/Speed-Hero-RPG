using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransition : MonoBehaviour
{
    public GameObject enemyToDestroyAfterFight;
    public GameObject overWorldObjParent;

    public string overworldScene;
    string battleScene;

    public void TransitionToBattle(string sceneToLoad)
    {
        battleScene = sceneToLoad;
        overWorldObjParent.SetActive(false); //disable overworld objects
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
    }

    public void TransitionFromBattle()
    {
        SceneManager.UnloadSceneAsync(battleScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(overworldScene));
        Destroy(enemyToDestroyAfterFight);

        overWorldObjParent.SetActive(true); //re-enable overworld objects
    }
}
