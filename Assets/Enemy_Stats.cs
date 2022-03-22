using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Stats : MonoBehaviour
{
    [SerializeField] private int maxHp;
    private int hp;

    [SerializeField] private Slider hpSlider;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        UpdateHPBar();
    }

    public void TakeDamage(int dmg)
    {
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
        hpSlider.value = (float)hp / maxHp;
    }
        

    private void Die()
    {
        Destroy(gameObject);
    }
}
