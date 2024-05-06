using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;

[RequireComponent(typeof(LokaPlayer))]
public class LokaVRHand : MonoBehaviour
{
    /// <summary>
    /// Left hand tracked transform
    /// </summary>
    public Transform LeftHandTransform;
    /// <summary>
    /// Right hand tracked transform
    /// </summary>
    public Transform RightHandTransform;

    /// <summary>
    /// Is hand data avaliable? (Does hands exist?)
    /// </summary>
    public bool IsAvailable => _leftHandDevice != null || _rightHandDevice != null;

    /// <summary>
    /// Is left hand data valid?
    /// </summary>
    public bool IsLeftHandTracked => _leftHandDevice != null && _leftHandDevice.trackingState.value != 0;

    /// <summary>
    /// Is right hand data valid?
    /// </summary>
    public bool IsRightHandTracked => _rightHandDevice != null && _rightHandDevice.trackingState.value != 0;

    public XRHandDevice LeftHandDevice => _leftHandDevice;
    public XRHandDevice RightHandDevice => _rightHandDevice;

    /* -------------------------------------------------------------------------- */

    LokaPlayer _lokaPlayer;
    XRHandDevice _leftHandDevice;
    XRHandDevice _rightHandDevice;

    List<GameObject> _leftHandJointTrackPos = new List<GameObject>(26);
    List<GameObject> _rightHandJointTrackPos = new List<GameObject>(26);


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _lokaPlayer = GetComponent<LokaPlayer>();        

        // Create hand joint track objects
        for(int handness = 0; handness < 2; handness++)
        {
            string handName = handness == 0 ? "Left" : "Right";
            for(int i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.localScale = Vector3.one * 0.015f;            
                go.SetActive(false);

                go.transform.SetParent(handness == 0 ? LeftHandTransform : RightHandTransform);
                go.name = $"{handName}HandJointTrackPos {i} ({XRHandJointIDUtility.FromIndex(i)})";
                (handness == 0 ? _leftHandJointTrackPos : _rightHandJointTrackPos).Add(go);
            }
        }
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        // Find devices
        if (_leftHandDevice == null)
        {
            var foundHand = _lokaPlayer.InputReceiver.devices.FirstOrDefault(
                x => x is XRHandDevice && x.usages.Any(u => u.ToString().ToLower().Contains("left")));
            if (foundHand != null)
            {
                _leftHandDevice = (XRHandDevice)foundHand;
            }
        }
        if (_rightHandDevice == null)
        {
            var foundHand = _lokaPlayer.InputReceiver.devices.FirstOrDefault(
                x => x is XRHandDevice && x.usages.Any(u => u.ToString().ToLower().Contains("right")));
            if (foundHand != null)
            {
                _rightHandDevice = (XRHandDevice)foundHand;
            }
        }

        // set hand pos
        if (_leftHandDevice != null)
        {
            LeftHandTransform.localPosition = _leftHandDevice.devicePosition.ReadValue();
            LeftHandTransform.rotation = _leftHandDevice.deviceRotation.ReadValue();                        
        }
        if (_rightHandDevice != null)
        {
            RightHandTransform.localPosition = _rightHandDevice.devicePosition.ReadValue();
            RightHandTransform.rotation = _rightHandDevice.deviceRotation.ReadValue();
        }    

        // set joint pos
        var lHandJoints = _lokaPlayer.LabDeviceChannel.GetData<List<Pose?>>(LabDeviceChannel.DataEnum.HAND_LEFT_JOINTS_POSE);
        var rHandJoints = _lokaPlayer.LabDeviceChannel.GetData<List<Pose?>>(LabDeviceChannel.DataEnum.HAND_RIGHT_JOINTS_POSE);
        for(int i = 0; i < lHandJoints.Count; i++)
        {
            if(lHandJoints[i].HasValue)
            {
                _leftHandJointTrackPos[i].SetActive(true);
                _leftHandJointTrackPos[i].transform.position = lHandJoints[i].Value.position - lHandJoints[0].Value.position + LeftHandTransform.position;
                _leftHandJointTrackPos[i].transform.rotation = lHandJoints[i].Value.rotation;
            }
            else
            {
                _leftHandJointTrackPos[i].SetActive(false);
            }
        }        
    }
}
