using UnityEngine;

[CreateAssetMenu(menuName = "Combat/EnemyObject")]
public class EnemyObject : ScriptableObject
{
    [SerializeField] private string enemyName;
    public string EnemyName => enemyName;

    [SerializeField] private Sprite enemySprite;
    public Sprite EnemySprite => enemySprite;

    [SerializeField] private int maxHP;
    public int MaxHP => maxHP;

    [SerializeField] private int dmg;
    public int Dmg => dmg;
}
