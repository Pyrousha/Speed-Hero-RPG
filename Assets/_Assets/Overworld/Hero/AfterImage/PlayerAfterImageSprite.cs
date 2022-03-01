using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    private float alpha;
    [SerializeField] private float startAlpha;
    [SerializeField] private float duration;

    private float spawnTime;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Color startColor;
    private Color color;
    private bool counting;

    public void SetSprite(Sprite spr)
    {
        spawnTime = Time.time;
        counting = true;

        spriteRenderer.sprite = spr;
        color = startColor;
    }

    private void Update()
    {
        if (counting == false)
            return;

        if (Time.time >= spawnTime + duration)
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
        else
        {
            float ratio = (Time.time - spawnTime) / duration;

            alpha = 1 - ratio;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}
