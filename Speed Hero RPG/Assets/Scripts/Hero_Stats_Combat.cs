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

    public Text healthBarText;
    public Slider healthBarSlider;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger?");
        if (other.tag == "EnemyProjectile")
        {
            Debug.Log("hit by projectile!");
            //Damage for "hit" projectiles is already handled in Enemy_Attack.cs
            if (other.GetComponent<Enemy_Attack>().atkType == Enemy_Attack.attackType.Dodge)
            {
                takeDamage(other.gameObject.GetComponent<Enemy_Attack>().dmg);
                Destroy(other.gameObject);
            }
        }
    }

    public void takeDamage(int damage)
    {
        if (invincible)
            return;

        hp -= damage;
        //Debug.Log("HP: " + hp + "/"+maxHp);
        if (hp<=0)
        {
            hp = 0;
            Die();
        }
        else
        {
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        healthBarText.text = "HP: " + hp + "/" + maxHp;
        healthBarSlider.value = ((float)hp) / ((float)maxHp);
    }

    public void Heal(int healing)
    {
        hp = Mathf.Max(maxHp, hp + healing);
    }

    public void Die()
    {
        SceneManager.LoadScene("DEAD", LoadSceneMode.Single);
    }
}
