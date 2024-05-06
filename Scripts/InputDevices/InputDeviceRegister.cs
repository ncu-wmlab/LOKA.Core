using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

public class InputDeviceRegister : MonoBehaviour
{
    void Awake()
    {
        // ^: regex start of line

        // Pico
        InputSystem.RegisterLayout<PXR_HMD>(
            matches: new InputDeviceMatcher()
                .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                .WithProduct(@"^(PICO HMD)|^(PICO Neo)|^(PICO G)"));
        InputSystem.RegisterLayout<PXR_Controller>(
            matches: new InputDeviceMatcher()
                .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                .WithProduct(@"^(PICO Controller)"));
    }
}
