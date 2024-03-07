using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EnviromentController : MonoBehaviour
{
    [SerializeField] InputAction _toggleAction;
    [SerializeField] float _uiFadeDuration = 0.39f;
    [SerializeField] CanvasGroup _uiCanvasGroup;
    [SerializeField] Transform _groundTransform;

    [SerializeField] List<GameObject> _objectsToShowHide;

    bool active;
    Vector3 _uiInitialPos;
    Vector3 _groundInitialScale;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        active = true;
        _toggleAction.Enable();
        _uiInitialPos = _uiCanvasGroup.transform.position;
        _groundInitialScale = _groundTransform.localScale;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_toggleAction.triggered)
        {
            Toggle();
        }
    }    

    void Toggle()
    {
        // print("[Stargazer_UI] Toggle "+ active);
        active = !active;
        StopAllCoroutines();
        StartCoroutine(FadeAnimation(active ? 1 : 0));
    }
    

    IEnumerator FadeAnimation(float targetAlpha)
    {
        SetShowHide(true);     
        while (_uiCanvasGroup.alpha != targetAlpha)
        {
            _uiCanvasGroup.alpha = Mathf.MoveTowards(_uiCanvasGroup.alpha, targetAlpha, Time.deltaTime / _uiFadeDuration);
            _groundTransform.localScale = _groundInitialScale*_uiCanvasGroup.alpha;
            yield return null;
        }
        _uiCanvasGroup.alpha = targetAlpha;   
        SetShowHide(targetAlpha==1);     
    }

    void SetShowHide(bool show)
    {
        _uiCanvasGroup.transform.position = show ? _uiInitialPos : Vector3.up * 9999f;
        _objectsToShowHide.ForEach(o => o.SetActive(show));
    }

}