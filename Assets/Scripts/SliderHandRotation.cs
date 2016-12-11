using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHandRotation : MonoBehaviour {
    public TimeSlider Slider;

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

    public Image PlayImage;

    void Awake() {
        if (Slider == null) {
            Slider = GetComponent<TimeSlider>();
        }
    }

    void Update() {
        var bigHandRotation = Slider.value * -360 / BigHandPeriod;
        var smallHandRotation = Slider.value * -360 / SmallHandPeriod;
        BigHandRotation = Mathf.Lerp(BigHandRotation, bigHandRotation, RotationSpeed);
        SmallHandRotation = Mathf.Lerp(SmallHandRotation, smallHandRotation, RotationSpeed);
        BigHand.transform.localRotation = Quaternion.Euler(0, 0, BigHandRotation);
        SmallHand.transform.localRotation = Quaternion.Euler(0, 0, SmallHandRotation);
        // hands vs play
        PlayImage.gameObject.SetActive(!(Slider.AutoPlaying || Slider.Held));
        BigHand.gameObject.SetActive(Slider.AutoPlaying || Slider.Held);
        SmallHand.gameObject.SetActive(Slider.AutoPlaying || Slider.Held);
    }
}
