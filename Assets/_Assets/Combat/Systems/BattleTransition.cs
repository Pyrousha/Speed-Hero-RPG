using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransition : MonoBehaviour
{
    public GameObject enemyToDestroyAfterFight;
    public GameObject overWorldObjParent;
    public Skybox battleSkybox;

    public string battleScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionToBattle(string sceneToLoad)
    {
        battleScene = sceneToLoad;
        overWorldObjParent.SetActive(false); //disable overworld objects
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
    }

    public void TransitionFromBattle()
    {
        SceneManager.UnloadSceneAsync(battleScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Zone1"));
        Destroy(enemyToDestroyAfterFight);

        overWorldObjParent.SetActive(true); //re-enable overworld objects
    }
}
