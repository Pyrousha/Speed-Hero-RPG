using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hero_Stats_Combat : MonoBehaviour
{

    public int hp;
    public int maxHp;
    public int dmg;
    public bool invincible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyProjectile")
        {
            takeDamage(other.gameObject.GetComponent<Enemy_Attack>().dmg);
            Destroy(other.gameObject);
        }
    }

    public void takeDamage(int damage)
    {
        if (invincible)
            return;

        hp -= damage;
        Debug.Log("HP: " + hp + "/"+maxHp);
        if (hp<=0)
        {
            hp = 0;
            Die();
        }
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
