using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    public GameObject photonObject;
    public GameObject soundManager;

    void Start() 
    {
        DontDestroyOnLoad(photonObject);
        DontDestroyOnLoad(soundManager);
    }

}