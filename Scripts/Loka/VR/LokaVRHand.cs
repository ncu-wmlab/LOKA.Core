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
    [Header("Hands Root Transform (Palm)")]
    public Transform LeftHandTransform;
    /// <summary>
    /// Right hand tracked transform
    /// </summary>
    public Transform RightHandTransform;

    /// <summary>
    /// 調整手的大小。
    /// 換言之就是手指與手腕的距離比值。
    /// </summary>
    [Header("Hand Joints (Fingers)")]
    [Range(0.1f, 1.5f)]
    public float HandSizeOffset = 1.0f;
    // 手部追蹤是以 Z 朝前。 TPose 模型左手是-X朝前，右手是+X朝前。
    public Vector3 LeftHandJointRotationOffset = new Vector3(0, 90, 0);
    public Vector3 RightHandJointRotationOffset = new Vector3(0, -90, 0);

    /// <summary>
    /// 是否顯示手指追蹤位置的立方體
    /// </summary>
    public bool DisplayHandJointCube = true;
    
    /* -------------------------------------------------------------------------- */

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
    public Dictionary<XRHandJointID, Transform> LeftHandJointTrackPos => _leftHandJointTrackPos;
    public Dictionary<XRHandJointID, Transform> RightHandJointTrackPos => _rightHandJointTrackPos;

    /* -------------------------------------------------------------------------- */

    LokaPlayer _lokaPlayer;
    XRHandDevice _leftHandDevice;
    XRHandDevice _rightHandDevice;
    Dictionary<XRHandJointID, Transform> _leftHandJointTrackPos = new Dictionary<XRHandJointID, Transform>();
    Dictionary<XRHandJointID, Transform> _rightHandJointTrackPos = new Dictionary<XRHandJointID, Transform>(26);

    /* -------------------------------------------------------------------------- */


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        _lokaPlayer = GetComponent<LokaPlayer>();

        // Create hand joint track objects
        for (int handness = 0; handness < 2; handness++)
        {
            string handName = handness == 0 ? "Left" : "Right";
            for (int i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)
            {
                var go = DisplayHandJointCube ? GameObject.CreatePrimitive(PrimitiveType.Cube) : new GameObject();
                go.transform.localScale = Vector3.one * 0.015f;
                go.SetActive(false);
                go.transform.SetParent(handness == 0 ? LeftHandTransform : RightHandTransform);
                go.name = $"{handName}HandJointTrackPos {i} ({XRHandJointIDUtility.FromIndex(i)})";
                (handness == 0 ? _leftHandJointTrackPos : _rightHandJointTrackPos).Add(XRHandJointIDUtility.FromIndex(i), go.transform);
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
        for (int handness = 0; handness < 2; handness++)
        {
            var handJoints = handness == 0 ? lHandJoints : rHandJoints;
            var handTrackPos = handness == 0 ? _leftHandJointTrackPos : _rightHandJointTrackPos;
            var handTransform = handness == 0 ? LeftHandTransform : RightHandTransform;
            var rotationOffset = handness == 0 ? LeftHandJointRotationOffset : RightHandJointRotationOffset;
            // handTransform.localPosition = handJoints[0].Value.position;  // handJoints[0].Value.position 就已經是 handTransform.localPosition 了
            // Debug.Log($"{handTransform.localPosition} = {handJoints[0].Value.position} Dist={Vector3.Distance(handTransform.localPosition, handJoints[0].Value.position)}");
            foreach(var joint in handTrackPos)
            {
                if(handJoints == null)
                    continue;
                var jointPose = handJoints[joint.Key.ToIndex()];
                if (jointPose.HasValue)
                {
                    joint.Value.gameObject.SetActive(true);
                    joint.Value.position = handTransform.position + (jointPose.Value.position-handTransform.localPosition) * HandSizeOffset;
                    joint.Value.rotation = jointPose.Value.rotation * Quaternion.Euler(rotationOffset);
                }
                else
                {
                    joint.Value.gameObject.SetActive(false);
                    // joint.Value.localPosition = Vector3.zero;
                    joint.Value.localRotation = Quaternion.identity;
                }
            }
        }
    }
}
