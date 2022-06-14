using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    private float alpha;
    [SerializeField] private float startAlpha;
    [SerializeField] private float alphaDecreasePerSec;

    private Transform playerTransform;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField] private Color startColor;
    private Color color;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.Find("Hero Sprite").transform;
        playerSpriteRenderer = playerTransform.GetComponent<SpriteRenderer>();

        alpha = startAlpha;
        spriteRenderer.sprite = playerSpriteRenderer.sprite;
        transform.position = playerTransform.position;
        transform.rotation = playerTransform.rotation;

        color = startColor;
    }

    private void Update()
    {
        alpha -= alphaDecreasePerSec*Time.deltaTime;
        color.a = alpha;
        spriteRenderer.color = color;

        if ( alpha <= 0.05f)
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
