using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//todo save destroyed obstacles

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    private float timeSinceLastCollision = float.MaxValue;

    private const float collisionCooldown = 1.0f;

    private SpriteRenderer sr;

    private bool on = false;

    private GameManager gameManager;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(on)
        {
            return;
        }

        if(collision.transform.CompareTag("Player") && timeSinceLastCollision > collisionCooldown)
        {
            var p = collision.gameObject.GetComponent<Player>();
            p.SetCheckPoint();
            sr.sprite = onSprite;
            on = true;
            gameManager.Checkpoint.Play();
        }
    }
}
