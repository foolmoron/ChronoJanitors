using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Manager<TimeManager> {

    public float VirtualTime = 0;

    void Awake() {
        Time.timeScale = 0.001f;
    }

    void FixedUpdate() {
        VirtualTime += Time.deltaTime;
    }
    
    public void SkipOneSecond() {
        SkipTime(3);
    }
    
    public void ReverseOneSecond() {
        ReverseTime(3);
    }

    public void GoToTime(float time, Action afterDone = null) {
        StopCoroutines();
        if (time < VirtualTime) {
            ReverseTime(VirtualTime - time, afterDone);
        } else if (time > VirtualTime) {
            SkipTime(time - VirtualTime, afterDone);
        } else {
            if (afterDone != null) {
                afterDone();
            }
        }
    }

    public void ReverseTime(float secondsToReverse, Action afterReverse = null) {
        StopCoroutines();
        MemoManager.Inst.RevertToTime(VirtualTime - secondsToReverse);
        if (afterReverse != null) {
            afterReverse();
        }
        AudioManager.Inst.SetAdvanceRewind(false);
    }
    
    public void SkipTime(float secondsToSkip, Action afterSkip = null) {
        StopCoroutines();
        StartCoroutine(SkipTimeCoroutine(secondsToSkip, afterSkip));
        AudioManager.Inst.SetAdvanceRewind(true);
    }

    public void StopCoroutines() {
        StopCoroutine("SkipTimeCoroutine");
    }

    public IEnumerator SkipTimeCoroutine(float secondsToSkip, Action afterSkip) {
        var timestep = Time.fixedDeltaTime;
        Time.timeScale = Math.Min(secondsToSkip / timestep, 100); // timescale can only go to 100 :[
        var t = 0f;
        while (t <= secondsToSkip) { // fixed update spinlock is much more accurate than WaitForSeconds()
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 0.001f;
        if (afterSkip != null) {
            afterSkip();
        }
    }
}
