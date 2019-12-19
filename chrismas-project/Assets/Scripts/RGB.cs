using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGB : MonoBehaviour
{
    const float speed = 0.5f;
    float t = 0;
    int i = 0;
    Color[] colors = {
        Color.red,
        Color.green,
        Color.cyan,
        Color.red
    };

    void Update()
    {
        t += Time.deltaTime * speed;
        gameObject.GetComponent<Light>().color = Color.Lerp(colors[i], colors[i + 1], t);

        if (t > 1)
        {
            t = 0;
            i++;
            if (i >= colors.Length - 1) i = 0;
        }
    }
}
