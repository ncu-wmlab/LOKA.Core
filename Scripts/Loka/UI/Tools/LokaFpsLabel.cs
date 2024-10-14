using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LokaFpsLabel : MonoBehaviour
{
    TMP_Text label;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        label =  GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // calc fps
        float fps = 1.0f / Time.deltaTime;

        if(label)
        {
            label.text = $"{fps:0.0} FPS";
        }
    }
}