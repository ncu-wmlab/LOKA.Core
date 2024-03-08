using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour
{
    [SerializeField] TMP_Text _resolutionText;
    [SerializeField] TMP_Text _localFovText;


    [SerializeField] TMP_Text _remoteFovText;
    [SerializeField] Slider _remoteFovSlider;

    Camera _camera;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _camera = Camera.main;

        _remoteFovSlider.onValueChanged.AddListener((float value) => {
            _remoteFovText.text = $"{value:0}°";
            FindObjectOfType<SceneControlChannel>().SendFov(value);
        });
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        _resolutionText.text = $"{Screen.width}x{Screen.height}";
        _localFovText.text = $"{_camera.fieldOfView:0}°";
    }
}