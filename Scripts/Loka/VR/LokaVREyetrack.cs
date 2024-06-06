using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    public XRHMD HmdDevice => _HMD;

    /* -------------------------------------------------------------------------- */


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _player = GetComponent<LokaPlayer>();        
    }

    public void RemapInputs()
    {
        
    }

    void LateUpdate()
    {        
        // if HMD not registered, try to find it
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
