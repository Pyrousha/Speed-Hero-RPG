using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Stats : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float hp;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Enemy_AI enemyAI;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        UpdateHPBar();
    }

    public void TakeDamage(float dmg)
    {
        //enemyAI.HitByHero();

        hp -= dmg;

        if(hp <= 0)
        {
            hp = 0;
            Die();
        }

        UpdateHPBar();
    }

    public void UpdateHPBar()
    {
        hpSlider.value = hp / maxHp;
    }
        

    private void Die()
    {
        Destroy(gameObject);
    }
}
