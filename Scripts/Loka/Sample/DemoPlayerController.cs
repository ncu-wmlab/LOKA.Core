using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using System;

public class DemoPlayerController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Transform _cameraTransform;
    [SerializeField] InputReceiver _inputReceiver;
    [SerializeField] Text _label;
    [SerializeField] InputField _chatInput;
    [SerializeField] GameObject _chatBubble;

    [SerializeField] float moveSpeed = 100f;
    [SerializeField] float rotateSpeed = 10f;


    Vector2 inputMovement;
    Vector2 inputLook;
    Vector3 initialPosition;

    protected void Awake()
    {
        _inputReceiver.onDeviceChange += OnDeviceChange;
        initialPosition = transform.position;        
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                {
                    _inputReceiver.PerformPairingWithDevice(device);
                    CheckPairedDevices();
                    return;
                }
            case InputDeviceChange.Removed:
                {
                    _inputReceiver.UnpairDevices(device);
                    CheckPairedDevices();
                    return;
                }
        }
    }

    public void CheckPairedDevices()
    {
        if (!_inputReceiver.user.valid)
            return;

        bool hasTouchscreenDevice =
            _inputReceiver.user.pairedDevices.Count(_ => _.path.Contains("Touchscreen")) > 0;

        print("IsTouchDevice: " + hasTouchscreenDevice);
    }

    private void Update()
    {
        _label.text = "player "+player.gameObject.name.Substring(12, 4); // FIXME
        
        var forwardDirection = Quaternion.Euler(0, _cameraTransform.transform.eulerAngles.y, 0);
        var moveForward = forwardDirection * new Vector3(inputMovement.x, 0, inputMovement.y);
        player.transform.Translate(moveForward * Time.deltaTime * moveSpeed);

        var moveAngles = new Vector3(-inputLook.y, inputLook.x);
        var newAngles = _cameraTransform.transform.localEulerAngles + moveAngles * Time.deltaTime * rotateSpeed;
        _cameraTransform.transform.localEulerAngles = new Vector3(newAngles.x, newAngles.y, 0);
        _chatBubble.transform.localEulerAngles = new Vector3(0, -newAngles.y, 0);
        _label.transform.localEulerAngles = new Vector3(0, -newAngles.y, 0);

        // reset if the ball fall down from the floor
        if (player.transform.position.y < -5)
        {
            player.transform.position = initialPosition;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    // public void SetLabel(string text)
    // {
    //     _label.text = text;
    // }

    public void OnControlsChanged()
    {
    }

    public void OnDeviceLost()
    {
    }

    public void OnDeviceRegained()
    {
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        inputMovement = value.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        inputLook = value.ReadValue<Vector2>();
    }

    public void OnChatButtonClicked()
    {        
        print("send by "+player.name);
        _chatBubble.GetComponentInChildren<Text>().text = _chatInput.text;
        _chatInput.text = "";
        StopAllCoroutines();
        StartCoroutine(HideChatBubble());
    }

    private IEnumerator HideChatBubble()
    {
        _chatBubble.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        _chatBubble.gameObject.SetActive(false);
    }
}