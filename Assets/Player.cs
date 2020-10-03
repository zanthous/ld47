using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float groundedDistance =.075f;
    [SerializeField] private AnimationCurve jumpCurve;
    private Rigidbody2D rb;

    private bool jumping = false;
    [SerializeField] private float jumpForce = 1.0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Debug.Log(IsGrounded());

        if(!jumping && Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            //todo jump 
            //StartCoroutine(Jump());
            rb.AddForce((GameManager.flipped ? Vector2.down : Vector2.up) * jumpForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator Jump()
    {
        jumping = true;
        float timer = 0.0f;
        while(timer < jumpCurve.length)
        {
            rb.AddForce(Vector2.up * jumpCurve.Evaluate(timer) * jumpForce);
            timer += Time.deltaTime;
            yield return null;
        }
        jumping = false;
    }

    private bool IsGrounded()
    {
        var hits = Physics2D.RaycastAll(transform.position, GameManager.flipped ? Vector2.up : Vector2.down, groundedDistance);
        Debug.DrawRay(transform.position, (GameManager.flipped ? Vector2.up : Vector2.down) * groundedDistance, Color.blue);

        foreach(var hit in hits)
        {
            if(hit.collider != null && hit.collider.transform.tag == "Walkable")
            {
                return true;
            }
        }
        return false;
    }
}
