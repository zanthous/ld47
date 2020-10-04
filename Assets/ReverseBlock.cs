using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseBlock : MonoBehaviour
{

    private float timeSinceLastCollision = float.MaxValue;

    private const float collisionCooldown = 1.0f;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        timeSinceLastCollision += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player") && timeSinceLastCollision > collisionCooldown)
        {
            timeSinceLastCollision = 0.0f;
            //var p = collision.gameObject.GetComponent<Player>();
            gameManager.Direction = !gameManager.Direction;
            gameManager.Reverse.Play();
        }
    }
}
