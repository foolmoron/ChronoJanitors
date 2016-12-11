using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Memo {
    [Serializable]
    public struct RigidbodyInfo {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
    }

    public float Time;
    public float BackgroundParticlesTime;
    public RigidbodyInfo[] RigidbodyInfos;
}

public class MemoManager : Manager<MemoManager> {
    public float MemoInterval = 1;
    public float TimeSinceMemo;

    public List<Memo> memos = new List<Memo>();

    void Start() {
        var paths = new List<string>();
        memos.Clear();
        foreach (var child in BreakableManager.Inst.BreakableContainer.transform) {
        }
        RecordMemo();
    }

    public void RecordMemo() {
        memos.Add(new Memo { Time = TimeManager.Inst.VirtualTime });
    }
    
    void FixedUpdate() {
        TimeSinceMemo += Time.deltaTime;
        if (TimeSinceMemo >= MemoInterval) {
            RecordMemo();
            TimeSinceMemo -= MemoInterval;
        }
    }

    public void RevertToTime(float time) {
        time = Mathf.Clamp(time, 0, TimeManager.Inst.VirtualTime);
        if (time >= TimeManager.Inst.VirtualTime) {
            return; // can't revert to the future
        }
        // get nearest memo to desired time
        var index = memos.FindNearestSorted(time, memo => memo.Time);
        var nearestMemo = memos[index];
        // set new virtual time
        TimeManager.Inst.VirtualTime = nearestMemo.Time;
        TimeSinceMemo = 0;
        // clear all future memos
        for (int i = index + 1; i < memos.Count;) {
            memos.RemoveAt(i);
        }
    }
}
