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
    /// We use this as origin point to track eye gaze
    /// Could be Center Eye position (in front of face)
    /// </summary>
    [Header("TrackPos (Optional)")]
    public Transform Origin;

    /// <summary>
    /// TrackPos: Where eyes look at
    /// </summary>
    public Transform EyeGazeTrackPos;
    /// <summary>
    /// 
    /// </summary>
    public Transform LeftEyeTrackPos;
    /// <summary>
    /// 
    /// </summary>
    public Transform RightEyeTrackPos;

    /* -------------------------------------------------------------------------- */

    [SerializeField] InputActionProperty _combinedEyeGazeVectorAction;
    [SerializeField] InputActionProperty _leftEyePositionAction;
    [SerializeField] InputActionProperty _leftEyeOpennessAction;
    [SerializeField] InputActionProperty _leftEyeRotationAction;
    [SerializeField] InputActionProperty _rightEyePositionAction;    
    [SerializeField] InputActionProperty _rightEyeRotationAction;    
    [SerializeField] InputActionProperty _rightEyeOpennessAction;

    /* -------------------------------------------------------------------------- */

    LokaPlayerVR _player;
    
    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Is eye data valid?
    /// </summary>
    public bool IsEyeTracked => _combinedEyeGazeVectorAction.action.ReadValue<Vector3>() != Vector3.zero;
    /// <summary>
    /// Left eye position
    /// </summary>
    public Vector3 LeftEyePosition => _leftEyePositionAction.action.ReadValue<Vector3>();
    /// <summary>
    /// Left eye rotation
    /// </summary>
    public Quaternion LeftEyeRotation => _leftEyeRotationAction.action.ReadValue<Quaternion>();
    /// <summary>
    /// Left eye openness value
    /// </summary>
    public float LeftEyeOpenness => _leftEyeOpennessAction.action.ReadValue<float>();
    /// <summary>
    /// Right eye position
    /// </summary>
    public Vector3 RightEyePosition => _rightEyePositionAction.action.ReadValue<Vector3>();
    /// <summary>
    /// Right eye rotation
    /// </summary>
    public Quaternion RightEyeRotation => _rightEyeRotationAction.action.ReadValue<Quaternion>();
    /// <summary>
    /// Right eye openness value
    /// </summary>
    public float RightEyeOpenness => _rightEyeOpennessAction.action.ReadValue<float>();
    /// <summary>
    /// Combined eye gaze vector
    /// </summary>
    public Vector3 CombinedEyeGazeVector => _combinedEyeGazeVectorAction.action.ReadValue<Vector3>();

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
        _leftEyePositionAction = _player.RemapInputAction(_leftEyePositionAction);
        _leftEyeRotationAction = _player.RemapInputAction(_leftEyeRotationAction);
        _leftEyeOpennessAction = _player.RemapInputAction(_leftEyeOpennessAction);
        _rightEyePositionAction = _player.RemapInputAction(_rightEyePositionAction);
        _rightEyeRotationAction = _player.RemapInputAction(_rightEyeRotationAction);
        _rightEyeOpennessAction = _player.RemapInputAction(_rightEyeOpennessAction);
        _combinedEyeGazeVectorAction = _player.RemapInputAction(_combinedEyeGazeVectorAction);
    }

    void LateUpdate()
    {        
        if(Origin)
        {
            if(EyeGazeTrackPos)
            {
                EyeGazeTrackPos.position = CombinedEyeGazeVector + Origin.position;
            }
            if(LeftEyeTrackPos)
            {
                LeftEyeTrackPos.position = LeftEyePosition + Origin.position;
                LeftEyeTrackPos.rotation = LeftEyeRotation;
            }
            if(RightEyeTrackPos)
            {
                RightEyeTrackPos.position = RightEyePosition + Origin.position;
                RightEyeTrackPos.rotation = RightEyeRotation;
            }
        }
        

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
