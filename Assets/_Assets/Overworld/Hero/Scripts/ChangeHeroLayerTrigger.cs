using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHeroLayerTrigger : MonoBehaviour
{
    public int newLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "Hero Hitbox")
        {
            collision.transform.Find("Hero Sprite").GetComponent<SpriteRenderer>().sortingOrder = newLayer;
        }

        RespawnHero2D respawnHero = GetComponent<RespawnHero2D>();
        if (respawnHero != null)
            respawnHero.Respawn();
    }
}
