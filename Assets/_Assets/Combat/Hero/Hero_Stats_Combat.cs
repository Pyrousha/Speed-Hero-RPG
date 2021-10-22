using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hero_Stats_Combat : MonoBehaviour
{
    [Header("Devhacks")]
    public bool invincible;
    bool dead = false;
    [SerializeField] private bool infiniteMana = false;

    [Header("Stats")]
    public int maxHp;
    private int hp;
    public int dmg;
    private int mp;
    public int GetMP => mp;

    [SerializeField] private int maxMp;

    private int healCount = 0;
    private int maxhealCount = 8;
    private int hpToHeal = 1;

    [Header("Objects")]
    public Text healthBarText;
    public Slider healthBarSlider;
    public Slider healBarSlider;
    [SerializeField] private Text manaBarText;
    [SerializeField] private Slider manaBarSlider;
    [SerializeField] private HeroMenuController heroMenuController;

    public bool isNoteEditorMode;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        mp = 0;

        UpdateHealthBar();
        UpdateHealBar();
        UpdateManaBar();
    }

    private void Update()
    {
        if ((infiniteMana) && (mp < maxMp))
            SetManaToMax();
    }

    public void SetHealth(int newHp)
    {
        hp = newHp;
        UpdateHealthBar();
    }

    public void SetHealBar(int newHeal)
    {
        healCount = newHeal;
        UpdateHealBar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyProjectile")
        {
            //Damage for "hit" projectiles is already handled in Enemy_Attack.cs

            Enemy_Attack enemy_Attack = other.GetComponent<Enemy_Attack>();

            if (enemy_Attack.atkType == Enemy_Attack.attackType.Dodge)
            {
                takeDamage(enemy_Attack.dmg);
                Destroy(other.gameObject);
            }
        }
    }

    public void DestroyEnemyAttack()
    {
        #region mana bar stuff
        mp = Mathf.Min(maxMp, mp + 1);
        UpdateManaBar();
        #endregion

        #region hp bar stuff
        if (healCount+1 > maxhealCount) //heal bar full
        {
            if (hp < maxHp) //not full hp, use heal
            {
                healCount = 0;
                Heal(hpToHeal);
            }
        }
        else
        {
            healCount = Mathf.Min(healCount+1, maxhealCount);
        }

        UpdateHealBar();
        #endregion
    }

    public void takeDamage(int damage)
    {
        if (invincible)
            return;

        hp -= damage;
        if (hp<=0)
        {
            hp = 0;

            if (!isNoteEditorMode)
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
        healthBarText.text = "" + hp + "/" + maxHp;
        healthBarSlider.value = ((float)hp) / ((float)maxHp);
    }

    void UpdateHealBar()
    {
        float newVal = ((float)healCount / (float)maxhealCount);
        healBarSlider.value = newVal;
    }

    void UpdateManaBar()
    {
        heroMenuController.OnHeroManaUpdated(mp);

        manaBarText.text = "" + mp + "/" + maxMp;
        float newVal = ((float)mp / (float)maxMp);
        manaBarSlider.value = newVal;
    }

    public void Heal(int healing)
    {
        hp = Mathf.Min(maxHp, hp + healing);
        UpdateHealthBar();
    }

    public void SpendMana(int manaUsed)
    {
        mp -= manaUsed;
        UpdateManaBar();
    }

    public void SetManaToMax()
    {
        mp = maxMp;
        UpdateManaBar();
    }

    public void Die()
    {
        if (dead == false)
        {
            dead = true;
            if (SceneManager.GetSceneByName("Zone1").isLoaded)
            {
                SceneManager.LoadScene("DEAD", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("Combat-Standard");
            }
            else
            {
                SceneManager.LoadScene("DEAD", LoadSceneMode.Single);
            }
        }
    }
}
