using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject levelObj;
    [SerializeField] private GameObject playerObj;
    private Player player;

    [SerializeField] private AnimationCurve jumpCurve;

    public static bool direction = false;
    public static bool flipped = false;

    [SerializeField] private float rotationSpeed = 5.0f;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        //rotate level
        var rotationAmount = direction ? Vector3.forward : Vector3.back;
        rotationAmount = rotationAmount * Time.deltaTime * rotationSpeed;
        levelObj.transform.Rotate(rotationAmount, Space.World);
    }
}
