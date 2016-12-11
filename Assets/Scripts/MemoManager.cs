﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Memo {
    [Serializable]
    public class RigidbodyInfo {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
    }

    public float Time;
    public float[] BackgroundParticleTimes;
    public Dictionary<string, RigidbodyInfo[]> BreakableRigidbodyInfos = new Dictionary<string, RigidbodyInfo[]>();
}

public class MemoManager : Manager<MemoManager> {
    public float MemoInterval = 1;
    public float TimeSinceMemo;

    public List<Memo> memos = new List<Memo>();

    public ParticleSystem[] BackgroundParticles;

    void Start() {
        var paths = new List<string>();
        memos.Clear();
        foreach (var child in BreakableManager.Inst.BreakableContainer.transform) {
        }
        RecordMemo();
    }

    public void RecordMemo() {
        var memo = new Memo {
            Time = TimeManager.Inst.VirtualTime,
            BackgroundParticleTimes = BackgroundParticles.Map(particles => particles.time),
        };
        // breakables
        foreach (var breakable in BreakableManager.Inst.Breakables) {
            var infos = new Memo.RigidbodyInfo[breakable.Value.Pieces.Length + 1]; // extra entry for the main rigidbody
            // pieces
            for (int i = 0; i < breakable.Value.Pieces.Length; i++) {
                var piece = breakable.Value.Pieces[i];
                var rb = breakable.Value.Rigidbodies[i];
                infos[i] = new Memo.RigidbodyInfo {
                    Position = piece.transform.position,
                    Rotation = piece.transform.rotation,
                    Velocity = rb != null ? rb.velocity : Vector3.zero,
                    AngularVelocity = rb != null ? rb.angularVelocity : Vector3.zero,
                };
            }
            // main object
            var mainPiece = breakable.Value.gameObject;
            var mainRB = breakable.Value.Rigidbody;
            var mainInfos = new Memo.RigidbodyInfo {
                Position = mainPiece.transform.position,
                Rotation = mainPiece.transform.rotation,
                Velocity = mainRB.velocity,
                AngularVelocity = mainRB.angularVelocity,
            };
            infos[breakable.Value.Pieces.Length] = mainInfos;
            // set infos
            memo.BreakableRigidbodyInfos[breakable.Key] = infos;
        }
        memos.Add(memo);
    }

    public void ApplyMemo(Memo memo) {
        // bg particles
        for (int i = 0; i < memo.BackgroundParticleTimes.Length; i++) {
            BackgroundParticles[i].Simulate(memo.BackgroundParticleTimes[i], true, true);
            BackgroundParticles[i].Play(true);
        }
        // breakables
        foreach (var breakableRigidbodyInfo in memo.BreakableRigidbodyInfos) {
            BreakableManager.Inst.Breakables[breakableRigidbodyInfo.Key].RigidbodyInfosToSet = breakableRigidbodyInfo.Value;
        }
    }

    void Update() {
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
        // apply memo
        ApplyMemo(nearestMemo);
        // clear all future memos
        for (int i = index + 1; i < memos.Count;) {
            memos.RemoveAt(i);
        }
    }
}
