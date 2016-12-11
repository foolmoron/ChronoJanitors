using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    [Range(0, 10)]
    public float ExplodeForce = 1f;
    [Range(1, 5)]
    public int Width = 1;
    [Range(1, 5)]
    public int Length = 1;
    [Range(0, 5)]
    public float Height = 1f;
    public bool IsStackable;
    public bool IsTable;
    
    public GameObject[] Pieces;
    public Rigidbody[] Rigidbodies;
    public Rigidbody Rigidbody;

    public Memo.RigidbodyInfo[] RigidbodyInfosToSet;

    public bool Exploded;

    void Awake() {
        Init();
    }
    
	public void Init() {
        if (!Rigidbody) {
            // get all child rigidbodies
            Rigidbody = GetComponent<Rigidbody>();
            Pieces = GetComponentsInChildren<Collider>().Map(child => child.gameObject);
            // setup rigidbody container
            Rigidbodies = new Rigidbody[Pieces.Length];
        }
    }

    void HandleCollision(Collision collision) {
        var force = 0f;
        foreach (var contact in collision.contacts) {
            force = Mathf.Max(force, Vector3.Dot(contact.normal, collision.relativeVelocity) * Rigidbody.mass * (contact.otherCollider.attachedRigidbody ? contact.otherCollider.attachedRigidbody.mass : 1));
        }
        if (force >= ExplodeForce) {
            Exploded = true;
        }
    }

    void OnCollisionEnter(Collision collision) {
        HandleCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        HandleCollision(collision);
    }

    /*
     You are from the apocalyptic future Jan 21 2017
     Nearly all human life on Earth has been lost to the AI singularity
     Unfortunately, your cleaning business has no more rooms to clean
     Use your trusty time machine to destroy rooms so you can get more business in the future

     Bonuses:
     Few explosions (1-5)
     Longest streak without explosions
     Average streak without explosions
     Full destruction
    */

    void Update() {
        if (Rigidbodies.Length > 0) {
            // exploding
            if (Exploded && Rigidbodies[0] == null) {
                // add rigidbodies to children to let them fly around
                for (int i = 0; i < Pieces.Length; i++) {
                    Pieces[i].AddComponent<Rigidbody>(); // this doesn't return a rigidbody component properly so we have to GetComponent
                    Rigidbodies[i] = Pieces[i].GetComponent<Rigidbody>();
                    Rigidbodies[i].AddExplosionForce(ExplodeForce * 100, transform.position, 10f);
                }
            } else if (!Exploded && Rigidbodies[0] != null) {
                // remove rigidbodies to combine object again
                for (int i = 0; i < Rigidbodies.Length; i++) {
                    Destroy(Rigidbodies[i]);
                    Rigidbodies[i] = null;
                }
            }
            // set infos
            if (RigidbodyInfosToSet != null && RigidbodyInfosToSet.Length > 0) {
                // main body
                var mainInfo = RigidbodyInfosToSet[RigidbodyInfosToSet.Length - 1];
                transform.localPosition = mainInfo.Position;
                transform.localRotation = mainInfo.Rotation;
                Rigidbody.velocity = mainInfo.Velocity;
                Rigidbody.angularVelocity = mainInfo.AngularVelocity;
                // pieces
                for (int i = 0; i < Pieces.Length && i < (RigidbodyInfosToSet.Length - 1); i++) {
                    var piece = Pieces[i];
                    var rb = Rigidbodies[i];
                    var info = RigidbodyInfosToSet[i];
                    if (info != null) {
                        piece.transform.localPosition = info.Position;
                        piece.transform.localRotation = info.Rotation;
                        if (rb != null) {
                            rb.velocity = info.Velocity;
                            rb.angularVelocity = info.AngularVelocity;
                        }
                    }
                }
            }
            // clear infos after setting
            RigidbodyInfosToSet = null;
        }
    }
    
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red.withAlpha(0.35f);
        Gizmos.DrawCube(transform.position + new Vector3((Width - 1)/2f, Height/2, (Length - 1) / 2f), new Vector3(Width, Height, Length));
    }
}
