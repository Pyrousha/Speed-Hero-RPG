using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndBattle : MonoBehaviour
{
    public BattleTransition persistObj;
    public GameObject battleObjParent;
    [SerializeField] private Image blackOverlay;

    [SerializeField] private AudioSource songAudio;

    //fadeout stuff
    [SerializeField] private float maxFadeoutTime;
    private float fadeoutTimer;
    private bool doFadeout;

    private bool wonFight;

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

    private void Update()
    {
        if (!doFadeout)
            return;

        fadeoutTimer = Mathf.Max(0, fadeoutTimer -= Time.deltaTime);
        float alpha = 1f - (fadeoutTimer / maxFadeoutTime);

        blackOverlay.color = new Color(0, 0, 0, alpha);

        songAudio.volume = 1 - alpha;

        if (alpha >= 1)
        {
            EndBattleScene(wonFight);
            doFadeout = false;
        }
    }

    public void StartFadeOut(bool didWinFight)
    {
        wonFight = didWinFight;

        fadeoutTimer = maxFadeoutTime;

        doFadeout = true;
    }

    public void EndBattleScene(bool didWinFight)
    {
        if (persistObj != null)
            persistObj.TransitionFromBattle(didWinFight);
    }
}
