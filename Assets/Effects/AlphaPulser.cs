using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaPulser : MonoBehaviour {

    [Range(0, 10f)]
    public float PulseTime = 0.5f;
    [Range(0, 2f)]
    public float PulseOffset = 0f;
    [Range(0, 1)]
    public float MinAlpha = 0.4f;
    [Range(0, 1)]
    public float MaxAlpha = 1f;

    private Image image;

    void Start() {
        image = GetComponent<Image>();
    }

    void Update() {
        var offsetPulseTime = (Time.realtimeSinceStartup + PulseOffset) % PulseTime;
        var interp = (offsetPulseTime / PulseTime) * 2 - 1;
        image.color = image.color.withAlpha(Mathf.Lerp(MinAlpha, MaxAlpha, Mathf.Abs(interp)));
    }
}
