using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Dictionary<string, bool> Exploded = new Dictionary<string, bool>();

    public static Memo Lerp(Memo memo1, Memo memo2, float t) {
        var memo = new Memo {
            Time = Mathf.Lerp(memo1.Time, memo2.Time, t),
            BackgroundParticleTimes = memo1.BackgroundParticleTimes.Map(memo2.BackgroundParticleTimes, (time1, time2) => Mathf.Lerp(time1, time2, t)),
        };
        foreach (var breakableRigidbodyInfo in memo1.BreakableRigidbodyInfos) {
            memo.BreakableRigidbodyInfos[breakableRigidbodyInfo.Key] =
                memo1.BreakableRigidbodyInfos[breakableRigidbodyInfo.Key].Map(
                    memo2.BreakableRigidbodyInfos[breakableRigidbodyInfo.Key],
                    (info1, info2) => new RigidbodyInfo {
                        Position = Vector3.Lerp(info1.Position, info2.Position, t),
                        Rotation = Quaternion.Slerp(info1.Rotation, info2.Rotation, t),
                        Velocity = Vector3.Lerp(info1.Velocity, info2.Velocity, t),
                        AngularVelocity = Vector3.Lerp(info1.AngularVelocity, info2.AngularVelocity, t),
                    }
                );
            memo.Exploded[breakableRigidbodyInfo.Key] = Mathf.Abs(t - memo1.Time) < Mathf.Abs(t - memo2.Time) ? memo1.Exploded[breakableRigidbodyInfo.Key] : memo2.Exploded[breakableRigidbodyInfo.Key];
        }
        return memo;
    }
}

public class MemoManager : Manager<MemoManager> {
    public float MemoInterval = 1;
    public float TimeSinceMemo;

    public List<Memo> memos = new List<Memo>();

    public ParticleSystem[] BackgroundParticles;

    void Start() {
        var paths = new List<string>();
        memos.Clear();
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
                    Position = piece.transform.localPosition,
                    Rotation = piece.transform.localRotation,
                    Velocity = rb != null ? rb.velocity : Vector3.zero,
                    AngularVelocity = rb != null ? rb.angularVelocity : Vector3.zero,
                };
            }
            // main object
            var mainPiece = breakable.Value.gameObject;
            var mainRB = breakable.Value.Rigidbody;
            var mainInfos = new Memo.RigidbodyInfo {
                Position = mainPiece.transform.localPosition,
                Rotation = mainPiece.transform.localRotation,
                Velocity = mainRB.velocity,
                AngularVelocity = mainRB.angularVelocity,
            };
            infos[breakable.Value.Pieces.Length] = mainInfos;
            // set infos
            memo.BreakableRigidbodyInfos[breakable.Key] = infos;
            // set exploded
            memo.Exploded[breakable.Key] = breakable.Value.Exploded;
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
            var breakable = BreakableManager.Inst.Breakables[breakableRigidbodyInfo.Key];
            breakable.RigidbodyInfosToSet = breakableRigidbodyInfo.Value;
            breakable.Exploded = memo.Exploded[breakableRigidbodyInfo.Key];
        }
    }

    void FixedUpdate() {
        TimeSinceMemo += Time.fixedDeltaTime;
        if (TimeSinceMemo >= MemoInterval) {
            RecordMemo();
            var m = memos[memos.Count - 1];
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
        // add new memo for current state
        RecordMemo();
        // lerp memo based on desired time
        var lerp = (time - memos[index].Time) / (memos[index + 1].Time - memos[index].Time);
        var lerpedMemo = Memo.Lerp(memos[index], memos[index + 1], lerp);
        // apply memo and set times
        ApplyMemo(lerpedMemo);
        TimeManager.Inst.VirtualTime = lerpedMemo.Time;
        TimeSinceMemo = lerp * MemoInterval;
        // clear all future memos
        for (int i = index + 1; i < memos.Count;) {
            memos.RemoveAt(i);
        }
    }
}
