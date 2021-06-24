using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hero_Stats_Combat : MonoBehaviour
{

    public int hp;
    public int maxHp;
    public int dmg;
    public bool invincible;

    int healCount = 0;
    int maxhealCount = 8;
    int hpHealed = 1;

    public Text healthBarText;
    public Slider healthBarSlider;
    public Slider healBarSlider;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthBar();
        UpdateHealBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyProjectile")
        {
            //Damage for "hit" projectiles is already handled in Enemy_Attack.cs
            //if (other.GetComponent<Enemy_Attack>().atkType == Enemy_Attack.attackType.Dodge)
            {
                takeDamage(other.gameObject.GetComponent<Enemy_Attack>().dmg);
                Destroy(other.gameObject);
            }
        }
    }

    public void DestroyEnemyAttack()
    {
        if (healCount+1 > maxhealCount) //heal bar full
        {
            if (hp < maxHp) //not full hp, use heal
            {
                healCount = 0;
                Heal(hpHealed);
            }
        }
        else
        {
            healCount = Mathf.Min(healCount+1, maxhealCount);
        }

        UpdateHealBar();
    }

    public void takeDamage(int damage)
    {
        if (invincible)
            return;

        hp -= damage;
        if (hp<=0)
        {
            hp = 0;
            Die();
        }
        else
        {
            if (healCount == maxhealCount)
                healCount = healCount / 2;
            else
                healCount = healCount / 4;

            UpdateHealBar();

            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        healthBarText.text = "HP: " + hp + "/" + maxHp;
        healthBarSlider.value = ((float)hp) / ((float)maxHp);
    }

    void UpdateHealBar()
    {
        Debug.Log("healCount: " + healCount);
       
        float newVal = ((float)healCount / (float)maxhealCount);
        Debug.Log("newVal: " + newVal);

        healBarSlider.value = newVal;
    }

    public void Heal(int healing)
    {
        hp = Mathf.Min(maxHp, hp + healing);
        UpdateHealthBar();
    }

    public void Die()
    {
        SceneManager.LoadScene("DEAD", LoadSceneMode.Single);
    }
}
