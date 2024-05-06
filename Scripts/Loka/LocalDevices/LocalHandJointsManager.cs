using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Hands;
using UnityEngine.InputSystem;
using System.Linq;

public class LocalHandJointsManager : MonoBehaviour
{
    public static LocalHandJointsManager Instance { get; private set; }

    [SerializeField] Transform xrOriginPos;
    XRHandSubsystem _HandSubsystem;

    void Start()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Get hand subsystem
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        for (var i = 0; i < handSubsystems.Count; ++i)
        {
            var handSubsystem = handSubsystems[i];
            if (handSubsystem.running)
            {
                _HandSubsystem = handSubsystem;
                break;
            }
        }
        
        if(xrOriginPos == null)
            Debug.LogWarning("LocalHandJointsManager: xrOriginPos is not set. Hand joints will be in Real-world space.");
        // if (m_HandSubsystem != null)
        //     m_HandSubsystem.updatedHands += OnUpdatedHands;
    }

    // void OnUpdatedHands(XRHandSubsystem subsystem,
    //     XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
    //     XRHandSubsystem.UpdateType updateType)
    // {
    //     switch (updateType)
    //     {
    //         case XRHandSubsystem.UpdateType.Dynamic:
    //             // Update game logic that uses hand data
    //             break;
    //         case XRHandSubsystem.UpdateType.BeforeRender:
    //             // Update visual objects that use hand data
    //             break;
    //     }
    // }

    public List<Pose?> GetJoints(Handedness hand)
    {
        List<Pose?> poses = new List<Pose?>();
        if(_HandSubsystem == null)
            return null;

        XRHand xrhand = hand==Handedness.Left ? _HandSubsystem.leftHand : _HandSubsystem.rightHand;
        if(xrhand == null)
            return null;


        poses.Add(xrhand.rootPose);        
        for (var i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)  // starts from 1
        {
            var trackingData = xrhand.GetJoint(XRHandJointIDUtility.FromIndex(i));
            if (trackingData.TryGetPose(out Pose pose))
            {
                var poseTransformed = xrOriginPos != null ? 
                    pose.GetTransformedBy(xrOriginPos) :  // convert to world space
                    pose;
                poses.Add(poseTransformed);
            }
            else
            {
                poses.Add(null);
            }
        }

        return poses;
    }
}