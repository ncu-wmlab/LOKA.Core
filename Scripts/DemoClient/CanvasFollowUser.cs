using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasFollowUser : MonoBehaviour
{
    Camera _camera;
    float _distance;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _camera = Camera.main;
        _distance = Vector3.Distance(transform.position,_camera.transform.position);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        transform.position = _camera.transform.position + _camera.transform.forward * _distance;
        transform.LookAt(_camera.transform.position);
    }
}