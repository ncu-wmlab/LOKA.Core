using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(LokaPlayer))]
public class LokaVREyetrack : MonoBehaviour
{
    /// <summary>
    /// Eyes position (in front of face)
    /// </summary>
    [SerializeField] Transform _eyeOrigin;

    /// <summary>
    /// Where eyes look at
    /// </summary>
    public Transform EyeGazeTarget;

    XRHMD _HMD;
    LokaPlayer _player;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Is data valid/available?
    /// </summary>
    public bool IsAvailable { get; private set;}
    /// <summary>
    /// Is left eye data valid?
    /// </summary>
    public bool IsLeftEyeTracked { get; private set;}
    /// <summary>
    /// Is right eye data valid?
    /// </summary>
    public bool IsRightEyeTracked { get; private set;}
    /// <summary>
    /// Left eye openness value
    /// </summary>
    public float LeftEyeOpenness { get; private set;}
    /// <summary>
    /// Right eye openness value
    /// </summary>
    public float RightEyeOpenness { get; private set;}

    /* -------------------------------------------------------------------------- */


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _player = GetComponent<LokaPlayer>();
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        // find HMD device
        if(_HMD == null)
        {
            var device = _player.InputReceiver.devices.FirstOrDefault(x => x is XRHMD);
            if(device == null)
                return;
            _HMD = (XRHMD)device;
        }
        
        // Retrieve data of device type
        if (_HMD is PXR_HMD picoHmd)
        {
            // PXR
            IsAvailable = true;
            IsLeftEyeTracked = picoHmd.combinedEyeGazeVector.value != Vector3.zero;
            IsRightEyeTracked = picoHmd.combinedEyeGazeVector.value != Vector3.zero;
            LeftEyeOpenness = picoHmd.leftEyeOpenness.value;
            RightEyeOpenness = picoHmd.rightEyeOpenness.value;
            EyeGazeTarget.position = _eyeOrigin.position + picoHmd.combinedEyeGazeVector.value;
        }
        // TODO orther VR devices eyetrack      
        else
        {
            IsAvailable = false;  
            IsLeftEyeTracked = false;
            IsRightEyeTracked = false; 
        }
    }
}
