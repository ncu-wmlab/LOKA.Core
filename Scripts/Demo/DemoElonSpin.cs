using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoElonSpin : MonoBehaviour
{
    public float  _spinSpeed = 30;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up, _spinSpeed * Time.deltaTime);
    }
}
