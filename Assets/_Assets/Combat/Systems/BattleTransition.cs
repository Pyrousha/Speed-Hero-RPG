using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTransition : MonoBehaviour
{
    [SerializeField] private GameObject overWorldObjParent;

    [SerializeField] private string overworldScene;
    private string battleScene;

    private TriggerBattle triggerBattle;

    public void TransitionToBattle(TriggerBattle.BattleType battleType, TriggerBattle newTriggerBattle)
    {
        if (battleType == TriggerBattle.BattleType.standard)
            battleScene = "Combat-Standard";

        triggerBattle = newTriggerBattle;

        overWorldObjParent.SetActive(false); //disable overworld objects

        SceneManager.LoadScene(battleScene, LoadSceneMode.Additive);
    }

    public void TransitionToSceneAdditive(string newScene)
    {
        battleScene = newScene;
        SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
    }

    public void SetBattleScene(string newBattleScene)
    {
        battleScene = newBattleScene;
    }

    public void CloseBeatCalibration()
    {
        SceneManager.UnloadSceneAsync(battleScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(overworldScene));
    }

    public void TransitionFromBattle(bool wonFight)
    {
        SceneManager.UnloadSceneAsync(battleScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(overworldScene));

        overWorldObjParent.SetActive(true); //re-enable overworld objects

        triggerBattle.EndBattle(wonFight);
    }
}
