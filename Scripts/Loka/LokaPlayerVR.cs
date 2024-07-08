using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Host player. (VR version) AKA. client controlled player in the host machine.
/// </summary>
[RequireComponent(typeof(InputReceiver))]
public class LokaPlayerVR : LokaPlayer
{   
    // RenderStreaming 會複製一個 InputAction mapping, 但是原本關連到的 InputAction 不會更新。因此需要重新指定。
    [Header("XRInputAction Remapping")]
    [SerializeField] ActionBasedSnapTurnProvider _xrOrigin_snapTurnProvider;
    [SerializeField] ActionBasedContinuousTurnProvider _xrOrigin_continuousTurnProvider;
    [SerializeField] ActionBasedContinuousMoveProvider _xrOrigin_moveProvider;
    [SerializeField] ActionBasedControllerManager _leftHandControllerManager;
    [SerializeField] ActionBasedController _leftHandXrController;
    [SerializeField] GrabMoveProvider _leftHandGrabMoveProvider;
    [SerializeField] ActionBasedControllerManager _rightHandControllerManager;
    [SerializeField] ActionBasedController _rightHandXrController;
    [SerializeField] GrabMoveProvider _rightHandGrabMoveProvider;
    [SerializeField] List<TrackedPoseDriver> _trackedPoseDrivers;
    
    /* -------------------------------------------------------------------------- */
    
    [Header("Track Pos (VRIK 綁骨架用，可自由微調這些追蹤點的位置/旋轉)")]
    public Transform HeadTrackTransform;
    public Transform LeftHandTrackTransform;
    public Transform RightHandTrackTransform;
    public Transform LeftControllerTrackTransform;
    public Transform RightControllerTrackTransform;

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// 手部追蹤 (若有)
    /// </summary>
    public LokaVRHand Hands { get; private set; }

    /// <summary>
    /// 眼動追蹤 (若有)
    /// </summary>
    public LokaVREyetrack Eyetrack { get; private set; }

    /// <summary>
    /// 控制器 (若有)
    /// </summary>
    // public LokaVRController Controllers { get; private set; }


    /* -------------------------------------------------------------------------- */

    public override void LateInit()
    {
        base.LateInit();

        _xrOrigin_snapTurnProvider = _xrOrigin_snapTurnProvider ?? GetComponentInChildren<ActionBasedSnapTurnProvider>();
        _xrOrigin_continuousTurnProvider = _xrOrigin_continuousTurnProvider ?? GetComponentInChildren<ActionBasedContinuousTurnProvider>();
        _xrOrigin_moveProvider = _xrOrigin_moveProvider ?? GetComponentInChildren<ActionBasedContinuousMoveProvider>();

        if (_xrOrigin_snapTurnProvider != null)
        {
            _xrOrigin_snapTurnProvider.leftHandSnapTurnAction = RemapInputAction(_xrOrigin_snapTurnProvider.leftHandSnapTurnAction);
            _xrOrigin_snapTurnProvider.rightHandSnapTurnAction = RemapInputAction(_xrOrigin_snapTurnProvider.rightHandSnapTurnAction);
        }
        if (_xrOrigin_continuousTurnProvider != null)
        {
            _xrOrigin_continuousTurnProvider.leftHandTurnAction = RemapInputAction(_xrOrigin_continuousTurnProvider.leftHandTurnAction);
            _xrOrigin_continuousTurnProvider.rightHandTurnAction = RemapInputAction(_xrOrigin_continuousTurnProvider.rightHandTurnAction);
        }
        if (_xrOrigin_moveProvider != null)
        {
            _xrOrigin_moveProvider.leftHandMoveAction = RemapInputAction(_xrOrigin_moveProvider.leftHandMoveAction);
            _xrOrigin_moveProvider.rightHandMoveAction = RemapInputAction(_xrOrigin_moveProvider.rightHandMoveAction);
        }        

        if (_leftHandControllerManager != null)
        {
            // TODO _leftHandControllerManager
        }
        if (_leftHandXrController != null)
        {
            _leftHandXrController.positionAction = RemapInputAction(_leftHandXrController.positionAction);
            _leftHandXrController.rotationAction = RemapInputAction(_leftHandXrController.rotationAction);
            _leftHandXrController.trackingStateAction = RemapInputAction(_leftHandXrController.trackingStateAction);
            _leftHandXrController.selectAction = RemapInputAction(_leftHandXrController.selectAction);
            _leftHandXrController.selectActionValue = RemapInputAction(_leftHandXrController.selectActionValue);
            _leftHandXrController.activateAction = RemapInputAction(_leftHandXrController.activateAction);
            _leftHandXrController.activateActionValue = RemapInputAction(_leftHandXrController.activateActionValue);
            _leftHandXrController.uiPressAction = RemapInputAction(_leftHandXrController.uiPressAction);
            _leftHandXrController.uiPressActionValue = RemapInputAction(_leftHandXrController.uiPressActionValue);
            _leftHandXrController.hapticDeviceAction = RemapInputAction(_leftHandXrController.hapticDeviceAction);
            _leftHandXrController.rotateAnchorAction = RemapInputAction(_leftHandXrController.rotateAnchorAction);
            _leftHandXrController.directionalAnchorRotationAction = RemapInputAction(_leftHandXrController.directionalAnchorRotationAction);
            _leftHandXrController.translateAnchorAction = RemapInputAction(_leftHandXrController.translateAnchorAction);
        }
        if (_leftHandGrabMoveProvider != null)
        {
            _leftHandGrabMoveProvider.grabMoveAction = RemapInputAction(_leftHandGrabMoveProvider.grabMoveAction);
        }

        if (_rightHandControllerManager != null)
        {
            // TODO _rightHandControllerManager
        }
        if (_rightHandXrController != null)
        {
            _rightHandXrController.positionAction = RemapInputAction(_rightHandXrController.positionAction);
            _rightHandXrController.rotationAction = RemapInputAction(_rightHandXrController.rotationAction);
            _rightHandXrController.trackingStateAction = RemapInputAction(_rightHandXrController.trackingStateAction);
            _rightHandXrController.selectAction = RemapInputAction(_rightHandXrController.selectAction);
            _rightHandXrController.selectActionValue = RemapInputAction(_rightHandXrController.selectActionValue);
            _rightHandXrController.activateAction = RemapInputAction(_rightHandXrController.activateAction);
            _rightHandXrController.activateActionValue = RemapInputAction(_rightHandXrController.activateActionValue);
            _rightHandXrController.uiPressAction = RemapInputAction(_rightHandXrController.uiPressAction);
            _rightHandXrController.uiPressActionValue = RemapInputAction(_rightHandXrController.uiPressActionValue);
            _rightHandXrController.hapticDeviceAction = RemapInputAction(_rightHandXrController.hapticDeviceAction);
            _rightHandXrController.rotateAnchorAction = RemapInputAction(_rightHandXrController.rotateAnchorAction);
            _rightHandXrController.directionalAnchorRotationAction = RemapInputAction(_rightHandXrController.directionalAnchorRotationAction);
            _rightHandXrController.translateAnchorAction = RemapInputAction(_rightHandXrController.translateAnchorAction);
        }
        if (_rightHandGrabMoveProvider != null)
        {
            _rightHandGrabMoveProvider.grabMoveAction = RemapInputAction(_rightHandGrabMoveProvider.grabMoveAction);
        }

        foreach(var driver in _trackedPoseDrivers)
        {
            if(!driver)
                continue;
            driver.positionInput = RemapInputAction(driver.positionInput);
            driver.rotationInput = RemapInputAction(driver.rotationInput);
            driver.trackingStateInput = RemapInputAction(driver.trackingStateInput);
        }

        Hands = GetComponent<LokaVRHand>();
        Eyetrack = GetComponent<LokaVREyetrack>();
        // Controllers = GetComponent<LokaVRController>();
    }

    /// <summary>
    /// Find specified action in old input action, and find the corresponding one in new input action.
    /// </summary>
    /// <param name="old"></param>
    /// <returns></returns>
    public InputActionProperty RemapInputAction(InputActionProperty old)
    {
        if(old.reference == null)
        {
            if(old.action != null)
                return old;
            throw new System.Exception("[LokaPlayerVR] InputActionProperty both action and reference is null.");
        }
        var newAction = RemapInputAction(old.reference);
        var newProp = new InputActionProperty(newAction);
        return newProp;
    }

    /// <summary>
    /// Find specified action in old input action, and find the corresponding one in new input action.
    /// </summary>
    /// <param name="old"></param>
    /// <returns></returns>
    public InputAction RemapInputAction(InputAction old)
    {
        var newAction = InputReceiver.actions.FindAction(old.id);
        if (newAction == null)
        {
            Debug.LogError($"[LokaPlayerVR] Cannot map InputAsset {old.name} ({old.id}) to new. Will use the origin one, which could have no effect or everyone share the same input!!");
            return old;
        }
        Debug.Log($"[LokaPlayerVR] Map InputAsset {old.name} ({old.id}) to {newAction.name} ({newAction.id}). ");
        return newAction;
    }
}