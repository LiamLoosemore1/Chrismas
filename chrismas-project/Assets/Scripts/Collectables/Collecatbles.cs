using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collecatbles : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        ScoringSystem.score += 1;

        Destroy(gameObject);

    }

}
