using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeSlider : Slider {
    public bool UpdateByTime = true;

    protected override void Awake() {
        base.Awake();
        onValueChanged.AddListener(val => {
            if (!UpdateByTime) {
                TimeManager.Inst.GoToTime(val);
            }
        });

    }

    public override void OnPointerDown(PointerEventData e) {
        base.OnPointerDown(e);
        UpdateByTime = false;
    }

    public override void OnPointerUp(PointerEventData e) {
        base.OnPointerUp(e);
        UpdateByTime = true;
    }

    void FixedUpdate() {
        if (UpdateByTime) {
            value = TimeManager.Inst.VirtualTime;
        }
    }
}
