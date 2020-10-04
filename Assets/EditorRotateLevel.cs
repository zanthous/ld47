//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[ExecuteInEditMode]
//public class EditorRotateLevel : Editor
//{
//    [SerializeField] private float rotationSpeed = 5.0f;
//    private GameObject level;

//    void OnSceneGUI()
//    {
//        if(level==null) level = GameObject.Find("SimpleSVG");

//        if(Event.current.keyCode == KeyCode.J)
//        {
//            var rotationAmount = Vector3.back;
//            rotationAmount = rotationAmount * rotationSpeed;
//            level.transform.Rotate(rotationAmount, Space.World);
//        }

//        if(Event.current.keyCode == KeyCode.L)
//        {
//            var rotationAmount = Vector3.forward;
//            rotationAmount = rotationAmount * rotationSpeed;
//            level.transform.Rotate(rotationAmount, Space.World);
//        }
//    }

//}
