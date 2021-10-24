using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Stats_Combat : MonoBehaviour
{
    [SerializeField] private EnemyObject enemyObject;

    private int hp;
    private int maxHp;
    private int dmg;

    [SerializeField] private Enemy_Shot_Creator EnemyShotCreator;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text nameText;
    [SerializeField] private Text hpValueText;

    private SpriteRenderer spriteRenderer;

    private SongLoader songLoader;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void EnemyStartAttack(int attackdir)
    {
        EnemyShotCreator.SpawnAttack(attackdir, dmg);
    }

    public void SetEnemyObject(EnemyObject newEnemyObject)
    {
        enemyObject = newEnemyObject;
    }

    public void LoadFromEnemyObject()
    {
        if (enemyObject == null)
            return;

        //Load Stats
        maxHp = enemyObject.MaxHP;
        hp = maxHp;
        dmg = enemyObject.Dmg;

        //Load Visuals
        nameText.text = enemyObject.EnemyName;
        spriteRenderer.sprite = enemyObject.EnemySprite;
    }

    public void TakeDamage(int dmgToTake)
    {
        if (hp == 0)
            return;

        hp = Mathf.Max(0, hp - dmgToTake);

        UpdateHealthBar();

        if (hp == 0)
            EnemyKilled();
    }

    public void UpdateHealthBar()
    {
        hpSlider.value = (((float)hp) / ((float)maxHp));
        hpValueText.text = hp.ToString() + " / " + maxHp.ToString();
    }
        
    public void SetSongLoader(SongLoader loader)
    {
        songLoader = loader;
    }

    public void EnemyKilled()
    {
        songLoader.EnemyKilled();
    }
}
