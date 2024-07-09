using UnityEngine;

public interface ILokaHostUISubpanel {
    // Methods
    void OnShow(LokaPlayer player);
    void OnHide();
    GameObject gameObject { get; }
}