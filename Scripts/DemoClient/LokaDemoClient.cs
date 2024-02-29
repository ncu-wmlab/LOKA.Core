using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LokaDemoClient : MonoBehaviour
{
    [SerializeField] LokaGuest _lokaGuest;

    [SerializeField] Button _connectButton;
    [SerializeField] Button _disconnectButton;
    

    // Start is called before the first frame update
    void Start()
    {
        _connectButton.onClick.AddListener(() => {
            _lokaGuest.StartStream();
        });

        _disconnectButton.onClick.AddListener(() => {
            _lokaGuest.StopStream();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
