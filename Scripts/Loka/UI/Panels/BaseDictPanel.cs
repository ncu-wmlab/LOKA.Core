using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// set <c>_dict</c> to display metrics. <br />
/// Use <c>_SetMetric</c> and <c>_GetMetric</c> to set and get metrics.
/// </summary>
public class BaseDictPanel : MonoBehaviour
{
    [Header("BaseDictPanel")]
    [SerializeField] RectTransform _containerParent;
    [SerializeField] GameObject _categoryTemplate;
    [SerializeField] GameObject _metricTemplate;


    /// <summary>
    /// _dict[categoryName][metricName] = metric value
    /// </summary>
    protected Dictionary<string, Dictionary<string, object>> _dict = new Dictionary<string, Dictionary<string, object>>()/*{{"", new Dictionary<string, object>()}}*/;
    Dictionary<string, Transform> _uiCategory = new Dictionary<string, Transform>();
    Dictionary<string, TMP_Text> _uiMetric = new Dictionary<string, TMP_Text>();

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        _categoryTemplate.SetActive(false);
        _metricTemplate.SetActive(false);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// Display metrics in UI 
    /// </summary>
    protected virtual void Update()
    {       
        foreach(var categoryName in _dict)
        {
            if(!_uiCategory.ContainsKey(categoryName.Key))
            {
                var container = Instantiate(_categoryTemplate, _containerParent);
                container.SetActive(true);
                container.name = $"{categoryName.Key} Container";
                container.GetComponentInChildren<TMP_Text>().text = categoryName.Key;
                _uiCategory[categoryName.Key] = container.transform;
            }

            foreach(var metricName in categoryName.Value)
            {
                if(string.IsNullOrEmpty(metricName.Key))
                    continue;

                var metricKey = $"{categoryName.Key}.{metricName.Key}";
                if(!_uiMetric.ContainsKey(metricKey))
                {
                    var metric = Instantiate(_metricTemplate, _uiCategory[categoryName.Key]);
                    metric.SetActive(true);
                    metric.name = $"{metricKey} Metric";
                    metric.transform.GetChild(0).GetComponent<TMP_Text>().text = metricName.Key;
                    _uiMetric[metricKey] = metric.transform.GetChild(1).GetComponent<TMP_Text>();
                }

                _uiMetric[metricKey].text = $"{metricName.Value}";
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_containerParent);
    }

    /// <summary>
    /// set metric value
    /// </summary>
    /// <param name="category"></param>
    /// <param name="metric"></param>
    /// <param name="value"></param>
    protected void _SetMetric(string category, string metric, object value)
    {
        if(!_dict.ContainsKey(category))
            _dict[category] = new Dictionary<string, object>();
        _dict[category][metric] = value;
    }

    /// <summary>
    /// get metric value
    /// </summary>
    /// <param name="category"></param>
    /// <param name="metric"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    protected object _GetMetric(string category, string metric, object defaultValue = null)
    {
        if(!_dict.ContainsKey(category))
            return defaultValue;
        if(!_dict[category].ContainsKey(metric))
            return defaultValue;
        return _dict[category][metric];
    }

    /// <summary>
    /// Clear All entries & UI
    /// </summary>
    public void Clear()
    {
        // TODO
        // _dict.Clear();
        // foreach(Transform t in _containerParent)
        // {
        //     if(t == _categoryTemplate || t == _metricTemplate)
        //         continue;
        //     Destroy(t.gameObject);
        // }
        // _uiCategory.Clear();
        // _uiMetric.Clear();
    }
}