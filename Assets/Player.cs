using Nito.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

struct GameState 
{
    public bool direction;
    public bool flipped;

    public Vector2 rbVelocity;
    public float yValue;
    public float levelRotation;

    public float time;
    //public float yScale; infer from flipped
    //public float gravityScale; infer from flipped
}


public class Player : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float groundedDistance =.08f;
    [SerializeField] private AnimationCurve jumpCurve;
    private Rigidbody2D rb;

    [SerializeField] private float jumpForce = 1.0f;

    private float gravityScale = 1.0f;

    //this should have gone in gamemanager 草!!!!
    private bool recording = true;

    private float timeSinceStart = 0.0f;

    //private Vector3 respawnPosition;
    private GameState respawnGameState;

    public static Action RespawnEvent;

    [SerializeField] private Slider slider;

    public bool Recording
    {
        get
        {
            return recording;
        }
        set
        {
            recording = value;
        }
    }

    //amount of time to track in number of fixedupdates
    private float recordLength = 2.0f;
    private Deque<GameState> gameStates = new Deque<GameState>();
    private float timeSinceLastJump;
    private float timeSinceLastFlip;

    private bool groundedLastFrame = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        //respawnPosition = transform.position;
        if(!SaveData.checkpoint)
        {
            SaveData.lastCheckpoint = CurrentGameState();
            SaveData.checkpoint = true;
        }
    }

    public void SetCheckPoint()
    {
        SaveData.lastCheckpoint = CurrentGameState();
        SaveData.checkpoint = true;
    }

    public void Respawn()
    {
        RespawnEvent.Invoke();
        gameManager.Die.Play();
        UpdateGameState(SaveData.lastCheckpoint);
    }

    void Update()
    {

#if UNITY_EDITOR
        if(IsGrounded())
        { 
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
#endif

        //rewinding
        if(!recording)
        {
            return;
        }
        timeSinceStart += Time.deltaTime;
        timeSinceLastJump += Time.deltaTime;
        timeSinceLastFlip += Time.deltaTime;

        if(Input.GetKey(KeyCode.F) && timeSinceLastJump > 0.1f && IsGrounded())
        {
            timeSinceLastJump = 0.0f;
            //todo jump 
            //StartCoroutine(Jump());
            //reset velocity to ensure jump velocity is added from nothing instead of to a velocity going down already
            rb.velocity = new Vector2();
            rb.AddForce((gameManager.Flipped ? Vector2.down : Vector2.up) * jumpForce, ForceMode2D.Impulse);
            gameManager.Jump.Play();
        }

        if(Input.GetKey(KeyCode.J) && timeSinceLastFlip > 0.1f && IsGrounded())
        {
            timeSinceLastFlip = 0.0f;
            //rb.AddForce((GameManager.flipped ? Vector2.down : Vector2.up) * jumpForce, ForceMode2D.Impulse);
            gameManager.Flipped = !gameManager.Flipped;
            rb.gravityScale = -rb.gravityScale;
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
            gameManager.GravityChange.Play();
        }

        UpdateGameStateQueue();

        groundedLastFrame = IsGrounded();

        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            gameManager.ResetLevel();//?
        }
    }

    private void UpdateGameStateQueue()
    {
        var gs = new GameState();
        gs = CurrentGameState();
        //todo add more variables to gamestate

        gameStates.AddToBack(gs);

        //todo debug

        //after rewind time has to be updated? or a different timer has to be used that tracks rewinding
        while(Math.Abs(gameStates[0].time - gameStates[gameStates.Count - 1].time) > recordLength)
        {
            gameStates.RemoveFromFront();
        }
    }

    private GameState CurrentGameState()
    {
        GameState gs;
        gs.yValue = transform.position.y;
        gs.direction = gameManager.Direction;
        gs.levelRotation = gameManager.LevelRotation();
        gs.flipped = gameManager.Flipped;
        gs.time = timeSinceStart;
        gs.rbVelocity = rb.velocity;
        return gs;
    }

    //my data structure reps....
    public IEnumerator Rewind(float duration, bool stopOnGrounded)
    {
        slider.gameObject.SetActive(true);
        slider.value = 1.0f;

        var maxTimePossible = Math.Abs(gameStates[0].time - gameStates[gameStates.Count - 1].time);

        Debug.Log($"Max time: {maxTimePossible}");

        float timer = 0.0f;
        recording = false;
        rb.isKinematic = true;
        //GetComponent<CapsuleCollider2D>().enabled = false;
        //shift is just for testing
        //Input.GetKey(KeyCode.LeftShift) && 
        var previousState = gameStates[gameStates.Count - 1];
        float timeProcessed = 0;
        bool exiting = false;

        float maxValue = maxTimePossible < duration ? maxTimePossible : duration;

        while(gameStates.Count > 0 && timer < duration)
        {
            //process extra frames in case of lag?
            while(timeProcessed <= timer && gameStates.Count > 0)
            {
                var gs = gameStates.RemoveFromBack();
                timeProcessed += Math.Abs(gs.time - previousState.time);

                UpdateGameState(gs);

                previousState = gs;

                if(stopOnGrounded && rb.velocity.y < .1f && IsGrounded())
                {
                    exiting = true;
                    break;
                }

                slider.value =  1 - (timeProcessed / maxValue);
            }

            if(exiting)
            {
                break;
            }

            timer += Time.deltaTime;
            
            // play through the game states at 2x speed what they ran at real time 
            yield return null;
        }

        timeSinceStart = previousState.time;
        recording = true;

        //GetComponent<CapsuleCollider2D>().enabled = true;
        rb.isKinematic = false;

        slider.gameObject.SetActive(false);
    }

    private void UpdateGameState(GameState gs)
    {
        gameManager.SetLevelRotation(gs.levelRotation);
        gameManager.Direction = gs.direction;

        transform.position = new Vector3(transform.position.x, gs.yValue, transform.position.z);
        gameManager.Flipped = gs.flipped;
        rb.velocity = gs.rbVelocity;

        //set sprite to be flipped if necessary
        transform.localScale = new Vector3(transform.localScale.x,
            gs.flipped ? -Math.Abs(transform.localScale.y) : Math.Abs(transform.localScale.y),
            transform.localScale.z);

        rb.gravityScale = gameManager.Flipped ? -gravityScale : gravityScale;
    }

    private void FixedUpdate()
    {
    }

    //private IEnumerator Jump()
    //{
    //    jumping = true;
    //    float timer = 0.0f;
    //    while(timer < jumpCurve.length)
    //    {
    //        rb.AddForce(Vector2.up * jumpCurve.Evaluate(timer) * jumpForce);
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //    jumping = false;
    //}

    private bool IsGrounded()
    {
        var hits = Physics2D.RaycastAll(transform.position, gameManager.Flipped ? Vector2.up : Vector2.down, groundedDistance);
        Debug.DrawRay(transform.position, (gameManager.Flipped ? Vector2.up : Vector2.down) * groundedDistance, Color.blue);

        foreach(var hit in hits)
        {
            if(hit.collider != null && hit.collider.transform.CompareTag("Walkable"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!recording)
            return;

        if(collision.transform.CompareTag("Spike") || collision.transform.CompareTag("Wall"))
        {
            //todo delay
            gameManager.ResetLevel();
        }

        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Goal"))
        {
            gameManager.WinGame();
        }
    }
}
