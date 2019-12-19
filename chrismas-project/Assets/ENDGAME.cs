using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ENDGAME : MonoBehaviour
{
    void FixedUpdate()
    {

        if(ScoringSystem.score == 20)
        {
            Debug.LogWarning("Testing");

            SceneManager.LoadScene("Credits");
        }
        
    }
}
