using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class LokaInputActionsPanel : BaseDictPanel, ILokaHostUISubpanel
{
    InputActionAsset _focusingInputActions;

    public void OnShow(LokaPlayer player)
    {
        _focusingInputActions = player.InputReceiver.actions;
    }

    public void OnHide()
    {
        _focusingInputActions = null;
    }    

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected override void Update()
    {
        if(!_focusingInputActions)
            return;

        foreach (var actionMap in _focusingInputActions.actionMaps)
        {
            foreach(var action in actionMap.actions)
            {
                _SetMetric(actionMap.name, action.name, action.ReadValueAsObject());
            }
        }

        base.Update();   
    }
}