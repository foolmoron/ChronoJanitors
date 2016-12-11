﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeSlider : Slider {
    public bool AutoPlaying;
    public bool Held;

    float downTime = float.NegativeInfinity;
    float prevValue;

    protected override void Awake() {
        base.Awake();
        onValueChanged.AddListener(val => {
            downTime = float.NegativeInfinity;
            TimeManager.Inst.GoToTime(val);
        });
    }

    public override void OnPointerDown(PointerEventData e) {
        var veryPrevValue = value;
        base.OnPointerDown(e);
        downTime = Time.realtimeSinceStartup;
        if (veryPrevValue == value && prevValue != value) {
            onValueChanged.Invoke(value);
        } else if (prevValue != value) {
            downTime = float.NegativeInfinity;
        }
        var wasAutoPlaying = AutoPlaying;
        AutoPlaying = false;
        if (wasAutoPlaying) {
            downTime = float.NegativeInfinity;
        }
        Held = true;
    }

    public override void OnPointerUp(PointerEventData e) {
        base.OnPointerUp(e);
        if (Time.realtimeSinceStartup - downTime < 0.3f) {
            // clicked
            AutoPlaying = true;
        }
        Held = false;
    }

    void Update() {
        if (AutoPlaying) {
            value += Time.fixedDeltaTime;
            onValueChanged.Invoke(value);
        }
        prevValue = value;
    }
}
