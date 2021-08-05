using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransition : MonoBehaviour
{
    public GameObject overWorldObjParent;
    public GameObject enemyToDestroyAfterFight;
    public Transform postBattleHeroPos;
    public GameObject heroObj;


    public string overworldScene;
    string battleScene;

    public void TransitionToBattle(string sceneToLoad, bool disableOverworld)
    {
        battleScene = sceneToLoad;
        if (disableOverworld)
            overWorldObjParent.SetActive(false); //disable overworld objects
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
    }

    public void SetBattleScene(string newBattleScene)
    {
        battleScene = newBattleScene;
    }

    public void TransitionFromBattle(bool destroyEnemy)
    {
        if (postBattleHeroPos != null)
        {
            heroObj.transform.position = postBattleHeroPos.position;
            postBattleHeroPos = null;
        }

        SceneManager.UnloadSceneAsync(battleScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(overworldScene));
        if (destroyEnemy)
            Destroy(enemyToDestroyAfterFight);

        overWorldObjParent.SetActive(true); //re-enable overworld objects
    }
}
