using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeShot : MonoBehaviour
{
    public float initialSpeed;
    public float decellerateSpeed;
    public int attackNum;

    public LayerMask m_LayerMask;
    [SerializeField] private Transform hitboxTransform;
    [SerializeField] private Rigidbody animationRB;

    // Start is called before the first frame update
    void Start()
    {
        CheckForCollision();

        if (animationRB != null)
        if (animationRB != null)
            animationRB.velocity = animationRB.transform.right * initialSpeed;
    }

    public void CheckForCollision()
    {
        Collider[] hitColliders = Physics.OverlapBox(hitboxTransform.position, hitboxTransform.localScale / 2, hitboxTransform.rotation, m_LayerMask);

        /*
        Enemy_Attack enemyProjectile = null;

        float currAnimTime = 0;

        //Check when there is a new collider coming into contact with the box
        for (int i = 0;  i < hitColliders.Length; i++)
        {
            Enemy_Attack enemyAttacki = hitColliders[i].GetComponent<Enemy_Attack>();
            if (enemyAttacki.GetAnimPercent() > currAnimTime)
                enemyProjectile = enemyAttacki;
        }

        enemyProjectile.TryDestroy(attackNum);
        */

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Enemy_Attack enemyAttacki = hitColliders[i].GetComponent<Enemy_Attack>();
            enemyAttacki.TryDestroy(attackNum);
        }
    }

    private void FixedUpdate()
    {
        if (animationRB != null)
            animationRB.velocity = animationRB.velocity * decellerateSpeed;
    }

    /*
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EnemyProjectile")
        {
            //Debug.Log("HIT!");
            other.GetComponent<Enemy_Attack>().TryDestroy(attackNum);
        }
    }*/

    public void DestroySelf()
    {
        animationRB.velocity = new Vector3(0, 0, 0);
        Destroy(gameObject);
    }
}
