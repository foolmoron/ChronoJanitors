using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Manager<TimeManager> {

    public float VirtualTime = 0;

    void FixedUpdate() {
        VirtualTime += Time.deltaTime;
    }
    
    public void SkipOneSecond() {
        SkipTime(1);
    }
    
    public void ReverseOneSecond() {
        ReverseTime(1);
    }

    public void ReverseTime(float secondsToReverse, Action afterReverse = null) {
        MemoManager.Inst.RevertToTime(VirtualTime - secondsToReverse);
        if (afterReverse != null) {
            afterReverse();
        }
    }

    public void SkipTime(float secondsToSkip, Action afterSkip = null) {
        StartCoroutine(SkipTimeEnumerator(secondsToSkip, afterSkip));
    }

    public IEnumerator SkipTimeEnumerator(float secondsToSkip, Action afterSkip) {
        var timestep = Time.fixedDeltaTime;
        Time.timeScale = Math.Min(secondsToSkip / timestep, 100); // timescale can only go to 100 :[
        var t = 0f;
        while (t <= secondsToSkip) { // fixed update spinlock is much more accurate than WaitForSeconds()
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1;
        if (afterSkip != null) {
            afterSkip();
        }
    }
}
