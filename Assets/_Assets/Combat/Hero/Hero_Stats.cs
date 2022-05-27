using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero_Stats : Singleton<Hero_Stats>
{
    [Header("Stats")]
    [SerializeField] private float maxHp;
    private float hp;
    [SerializeField] private float damage;
    public float Damage => damage;

    [Header("References")]
    [SerializeField] private Slider heroHpSlider;
    [SerializeField] private SpriteRenderer heroSpriteRend;
    [SerializeField] private Color invincibleColor;
    private bool isInvincible;

    [SerializeField] private float invincibleDuration;
    private float endInvincibleTime;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        UpdateHPbar();
    }

    private void Update()
    {
        if(isInvincible)
        {
            if (Time.time >= endInvincibleTime)
            {
                //End invinciblity
                isInvincible = false;
                heroSpriteRend.color = Color.white;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isInvincible)
            return;

        hp -= dmg;

        if(hp <= 0)
        {
            hp = 0;
            Die();
        }

        UpdateHPbar();

        //Make player invincible
        BecomeInvincible();
        heroSpriteRend.color = invincibleColor;
    }

    public void BecomeInvincible()
    {
        isInvincible = true;
        endInvincibleTime = Time.time + invincibleDuration;
    }

    public void UpdateHPbar()
    {
        heroHpSlider.value = (float)hp / maxHp;
    }
        
    private void Die()
    {

    }
}
