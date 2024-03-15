using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AppInfoLabel : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        var label =  GetComponent<TMP_Text>();
        label.text = label.text
            .Replace("{APP_NAME}", Application.productName)
            .Replace("{APP_VERSION}", Application.version)
        ;
    }
}