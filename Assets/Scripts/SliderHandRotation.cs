using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHandRotation : MonoBehaviour {
    public Slider Slider;

    public Image BigHand;
    public float BigHandRotation;
    [Range(0, 30)]
    public float BigHandPeriod = 3;
    public Image SmallHand;
    public float SmallHandRotation;
    [Range(0, 30)]
    public float SmallHandPeriod = 15;
    [Range(0.01f, 0.25f)]
    public float RotationSpeed = 0.1f;

    void Awake() {
        if (Slider == null) {
            Slider = GetComponent<Slider>();
        }
    }

    void FixedUpdate() {
        var bigHandRotation = Slider.value * -360 / BigHandPeriod;
        var smallHandRotation = Slider.value * -360 / SmallHandPeriod;
        BigHandRotation = Mathf.Lerp(BigHandRotation, bigHandRotation, RotationSpeed);
        SmallHandRotation = Mathf.Lerp(SmallHandRotation, smallHandRotation, RotationSpeed);
        BigHand.transform.rotation = Quaternion.Euler(BigHand.transform.rotation.eulerAngles.withZ(BigHandRotation));
        SmallHand.transform.rotation = Quaternion.Euler(SmallHand.transform.rotation.eulerAngles.withZ(SmallHandRotation));
    }
}
