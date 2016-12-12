using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Manager<AudioManager> {

    public AudioClip[] AudioClips;
    Dictionary<string, AudioClip> audios = new Dictionary<string, AudioClip>();

    public AudioSource AdvanceSource;

    HashSet<string> ToPlay = new HashSet<string>();

    void Start() {
        foreach (var audioClip in AudioClips) {
            audios[audioClip.name] = audioClip;
        }
    }

    void Update() {
        //foreach (var s in ToPlay) {
        //    AudioSource.PlayClipAtPoint(audios[s], Camera.main.transform.position);
        //}
    }

    public void Play(string a) {
        //ToPlay.Add(a);
    }

    public void SetAdvanceRewind(bool? are) {
    }
}
