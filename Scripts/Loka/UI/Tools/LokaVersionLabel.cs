using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class LokaVersionLabel : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        var label =  GetComponent<TMP_Text>();
        if(label)
        {
            label.text = ReplaceLabel(label.text);
            return;
        }

        var labelUgui = GetComponent<Text>();
        if(labelUgui)
        {
            labelUgui.text = ReplaceLabel(labelUgui.text);
        }
    }

    protected virtual string ReplaceLabel(string label)
    {
        return label
            .Replace("{APP_NAME}", Application.productName)
            .Replace("{APP_VERSION}", Application.version)
            // .Replace("{APP_BUILD}", Application.buildGUID)
            .Replace("{LOKA_VERSION}", LokaConstants.LOKA_VERSION)
        ;
    }
}