﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour
{

    public GameObject scoreText;

    public static int score;

    void Update()
    {
        scoreText.GetComponent<Text>().text = "SCORE: " + score;
    }

}
