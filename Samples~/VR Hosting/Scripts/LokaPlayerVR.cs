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
    [Header("XRInputAction re-mapping")]
    [SerializeField] ActionBasedSnapTurnProvider _xrOrigin_snapTurnProvider;
    [SerializeField] ActionBasedContinuousTurnProvider _xrOrigin_continuousTurnProvider;
    [SerializeField] ActionBasedContinuousMoveProvider _xrOrigin_moveProvider;
    [SerializeField] TrackedPoseDriver _mainCamera_PoseDriver;
    [SerializeField] ActionBasedControllerManager _leftHandControllerManager;
    [SerializeField] ActionBasedController _leftHandXrController;
    [SerializeField] GrabMoveProvider _leftHandGrabMoveProvider;
    [SerializeField] ActionBasedControllerManager _rightHandControllerManager;
    [SerializeField] ActionBasedController _rightHandXrController;
    [SerializeField] GrabMoveProvider _rightHandGrabMoveProvider;

    [Header("Runtime")]    
    public InputActionAsset NewInputActionAsset = null; 


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        InitStream();
    }

    void InitStream()
    {
        // TODO
        NewInputActionAsset = GetComponent<InputReceiver>().actions;

        _xrOrigin_snapTurnProvider.leftHandSnapTurnAction = MigrateInputAction(_xrOrigin_snapTurnProvider.leftHandSnapTurnAction);
        _xrOrigin_snapTurnProvider.rightHandSnapTurnAction = MigrateInputAction(_xrOrigin_snapTurnProvider.rightHandSnapTurnAction);
        _xrOrigin_continuousTurnProvider.leftHandTurnAction = MigrateInputAction(_xrOrigin_continuousTurnProvider.leftHandTurnAction);
        _xrOrigin_continuousTurnProvider.rightHandTurnAction = MigrateInputAction(_xrOrigin_continuousTurnProvider.rightHandTurnAction);
        _xrOrigin_moveProvider.leftHandMoveAction = MigrateInputAction(_xrOrigin_moveProvider.leftHandMoveAction);
        _xrOrigin_moveProvider.rightHandMoveAction = MigrateInputAction(_xrOrigin_moveProvider.rightHandMoveAction);
        _mainCamera_PoseDriver.positionAction = MigrateInputAction(_mainCamera_PoseDriver.positionAction);
        _mainCamera_PoseDriver.rotationAction = MigrateInputAction(_mainCamera_PoseDriver.rotationAction);
        // TODO _leftHandControllerManager
        _leftHandXrController.positionAction = MigrateInputAction(_leftHandXrController.positionAction);
        _leftHandXrController.rotationAction = MigrateInputAction(_leftHandXrController.rotationAction);
        _leftHandXrController.trackingStateAction = MigrateInputAction(_leftHandXrController.trackingStateAction);
        _leftHandXrController.selectAction = MigrateInputAction(_leftHandXrController.selectAction);
        _leftHandXrController.selectActionValue = MigrateInputAction(_leftHandXrController.selectActionValue);
        _leftHandXrController.activateAction = MigrateInputAction(_leftHandXrController.activateAction);
        _leftHandXrController.activateActionValue = MigrateInputAction(_leftHandXrController.activateActionValue);
        _leftHandXrController.uiPressAction = MigrateInputAction(_leftHandXrController.uiPressAction);
        _leftHandXrController.uiPressActionValue = MigrateInputAction(_leftHandXrController.uiPressActionValue);
        _leftHandXrController.hapticDeviceAction = MigrateInputAction(_leftHandXrController.hapticDeviceAction);
        _leftHandXrController.rotateAnchorAction = MigrateInputAction(_leftHandXrController.rotateAnchorAction);
        _leftHandXrController.directionalAnchorRotationAction = MigrateInputAction(_leftHandXrController.directionalAnchorRotationAction);
        _leftHandXrController.translateAnchorAction = MigrateInputAction(_leftHandXrController.translateAnchorAction);
        _leftHandGrabMoveProvider.grabMoveAction = MigrateInputAction(_leftHandGrabMoveProvider.grabMoveAction);
        // TODO _rightHandControllerManager
        _rightHandXrController.positionAction = MigrateInputAction(_rightHandXrController.positionAction);
        _rightHandXrController.rotationAction = MigrateInputAction(_rightHandXrController.rotationAction);
        _rightHandXrController.trackingStateAction = MigrateInputAction(_rightHandXrController.trackingStateAction);
        _rightHandXrController.selectAction = MigrateInputAction(_rightHandXrController.selectAction);
        _rightHandXrController.selectActionValue = MigrateInputAction(_rightHandXrController.selectActionValue);
        _rightHandXrController.activateAction = MigrateInputAction(_rightHandXrController.activateAction);
        _rightHandXrController.activateActionValue = MigrateInputAction(_rightHandXrController.activateActionValue);
        _rightHandXrController.uiPressAction = MigrateInputAction(_rightHandXrController.uiPressAction);
        _rightHandXrController.uiPressActionValue = MigrateInputAction(_rightHandXrController.uiPressActionValue);
        _rightHandXrController.hapticDeviceAction = MigrateInputAction(_rightHandXrController.hapticDeviceAction);
        _rightHandXrController.rotateAnchorAction = MigrateInputAction(_rightHandXrController.rotateAnchorAction);
        _rightHandXrController.directionalAnchorRotationAction = MigrateInputAction(_rightHandXrController.directionalAnchorRotationAction);
        _rightHandXrController.translateAnchorAction = MigrateInputAction(_rightHandXrController.translateAnchorAction);
        _rightHandGrabMoveProvider.grabMoveAction = MigrateInputAction(_rightHandGrabMoveProvider.grabMoveAction);
    }

    InputActionProperty MigrateInputAction(InputActionProperty old)
    {
        var newAction = MigrateInputAction(old.action);
        var newProp = new InputActionProperty(newAction);
        return newProp;
    }

    InputAction MigrateInputAction(InputAction old)
    {
        var newAction = NewInputActionAsset.FindAction(old.id);
        // Debug.Log($"[LokaPlayer] Map InputAsset {old.name} ({old.id}) to {newAction.name} ({newAction.id}). ");
        if(newAction == null)
        {
            Debug.LogError($"[LokaPlayer] Cannot map InputAsset {old.name} ({old.id}). Will use the origin one, which could have no effect or everyone share the same input!!");
            return old;
        }
        return newAction;
    }
}