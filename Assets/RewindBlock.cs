using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindBlock : MonoBehaviour
{
    private const float collisionCooldown = .3f;

    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;

    //current max length is probably 1?
    [SerializeField] private float rewindDuration = 0.3f;
    [SerializeField] private float rewindCount = 3;

    [SerializeField] private bool stopOnGrounded;

    private float timeSinceLastCollision = float.MaxValue;

    private SpriteRenderer spriteRenderer;

    private GameManager gameManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite();
        Player.RespawnEvent += OnPlayerRespawned;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnDestroy()
    {
        Player.RespawnEvent -= OnPlayerRespawned;
    }

    private void OnPlayerRespawned()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        timeSinceLastCollision += Time.deltaTime;
    }

    private void SetSprite()
    {
        if(rewindCount == 3)
        {
            spriteRenderer.sprite = sprite3;
        }
        else if(rewindCount == 2)
        {
            spriteRenderer.sprite = sprite2;
        }
        else if(rewindCount == 1)
        {
            spriteRenderer.sprite = sprite1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player") && timeSinceLastCollision > collisionCooldown)
        {
            timeSinceLastCollision = 0.0f;
            var p = collision.gameObject.GetComponent<Player>();
            p.StartCoroutine(p.Rewind(rewindDuration, stopOnGrounded));
            rewindCount--;
            gameManager.Rewind.Play();
            if(rewindCount <= 0)
            {
                //todo animation sfx etc
                //cant destroy or it stops the coroutine
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
            else
            {
                SetSprite();
            }
        }
    }
}
