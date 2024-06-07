using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(LokaPlayer))]
public class LokaVREyetrack : MonoBehaviour, ILokaVRDevice
{
    /// <summary>
    /// Eyes position (in front of face)
    /// </summary>
    [SerializeField] Transform _eyeOrigin;

    /// <summary>
    /// Where eyes look at
    /// </summary>
    public Transform EyeGazeTrackPos;

    /* -------------------------------------------------------------------------- */

    [SerializeField] InputActionProperty _combinedEyeGazeVectorAction;
    [SerializeField] InputActionProperty _leftEyeOpennessAction;
    [SerializeField] InputActionProperty _rightEyeOpennessAction;

    /* -------------------------------------------------------------------------- */

    XRHMD _HMD;
    LokaPlayerVR _player;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Is eye data valid?
    /// </summary>
    public bool IsEyeTracked => _combinedEyeGazeVectorAction.action.ReadValue<Vector3>() != Vector3.zero;
    /// <summary>
    /// Left eye openness value
    /// </summary>
    public float LeftEyeOpenness => _leftEyeOpennessAction.action.ReadValue<float>();
    /// <summary>
    /// Right eye openness value
    /// </summary>
    public float RightEyeOpenness => _rightEyeOpennessAction.action.ReadValue<float>();

    /* -------------------------------------------------------------------------- */


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _player = GetComponent<LokaPlayerVR>();        
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // remap input actions
        _combinedEyeGazeVectorAction = _player.RemapInputAction(_combinedEyeGazeVectorAction);
        _leftEyeOpennessAction = _player.RemapInputAction(_leftEyeOpennessAction);
        _rightEyeOpennessAction = _player.RemapInputAction(_rightEyeOpennessAction);
    }

    void LateUpdate()
    {        
        EyeGazeTrackPos.position = _combinedEyeGazeVectorAction.action.ReadValue<Vector3>() + _eyeOrigin.position;

        // if HMD not registered, try to find it
        // if(_HMD == null)
        // {
        //     var device = _player.InputReceiver.devices.FirstOrDefault(x => x is XRHMD);
        //     if(device == null)
        //         return;
        //     _HMD = (XRHMD)device;
        // }

        // Retrieve data of device type
        // if (_HMD is PXR_HMD picoHmd)
        // {
        //     // PXR
        //     IsAvailable = true;
        //     IsLeftEyeTracked = picoHmd.combinedEyeGazeVector.value != Vector3.zero;
        //     IsRightEyeTracked = picoHmd.combinedEyeGazeVector.value != Vector3.zero;
        //     LeftEyeOpenness = picoHmd.leftEyeOpenness.value;
        //     RightEyeOpenness = picoHmd.rightEyeOpenness.value;
        //     EyeGazeTarget.position = _eyeOrigin.position + picoHmd.combinedEyeGazeVector.value;
        // }
        // else
        // {
        //     IsAvailable = false;  
        //     IsLeftEyeTracked = false;
        //     IsRightEyeTracked = false; 
        // }
    }    
}
