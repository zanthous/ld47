using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //private bool direction = false;
    //private bool flipped = false;

    public bool Direction { get; set; }
    public bool Flipped { get; set; }

    [SerializeField] private GameObject levelObj;
    [SerializeField] private GameObject playerObj;
    private Player player;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float rotationSpeed = 5.0f;

    [SerializeField] private float timeScale;

    private float scoreTimer = 0.0f;
    [SerializeField] private TextMeshProUGUI UITimer;

    private List<float> rotationSpeeds = new List<float>()
    {
        3,
        5,
        7
    };

    private bool won = false;

    [SerializeField] public AudioSource Checkpoint;
    [SerializeField] public AudioSource Die;
    [SerializeField] public AudioSource GravityChange;
    [SerializeField] public AudioSource Jump;
    [SerializeField] public AudioSource Reverse;
    [SerializeField] public AudioSource Rewind;



    public void WinGame()
    {
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;

        won = true;
    }

    public float LevelRotation()
    {
        return levelObj.transform.eulerAngles.z;
    }

    public void SetLevelRotation(float rotation) => levelObj.transform.rotation = Quaternion.Euler(0, 0, rotation);

    void Start()
    {
        Application.targetFrameRate = 144;
        player = FindObjectOfType<Player>();
        
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Direction = !Direction;
        }
#endif
        if(!won)
        { 
            scoreTimer += Time.deltaTime;
            UITimer.text = "Time: " + scoreTimer.ToString();
        }
        Time.timeScale = timeScale;

        SetRotationSpeed();

        //rotate level
        if(player.Recording)
        {
            var rotationAmount = Direction ? Vector3.back : Vector3.forward;
            rotationAmount = rotationAmount * Time.deltaTime * rotationSpeed;
            levelObj.transform.Rotate(rotationAmount, Space.World);
        }
    }

    private void SetRotationSpeed()
    {
        
        rotationSpeed = 7*3.3f;

        if(Math.Abs(player.transform.position.y) > 2.9f)
        {
            //i used 5 as an input value instead of 16.5 on accident
            rotationSpeed = 5 * 3.3f;
        }
        if(Math.Abs(player.transform.position.y) > 4.2f)
        {
            rotationSpeed = 3*3.3f;
        }
    }

    public void ResetLevel()
    {
        //ResetStaticVariables();
        //SceneManager.LoadScene(0);
        player.Respawn();
    }

    //private static void ResetStaticVariables()
    //{
    //    direction = false;
    //    flipped = false;
    //}
}
