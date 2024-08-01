using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float breakForceThreshold = 5.0f;
    [SerializeField] GameObject gfx;
    [SerializeField] GameObject brokenGfx;

    void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;

        if (collisionForce > breakForceThreshold)
        {
            Debug.Log("Nesne kýrýldý! Çarpýþma þiddeti: " + collisionForce);
            brokenGfx.SetActive(true);
            gfx.SetActive(false);
            //GetComponent<BoxCollider>().enabled = false;
            Destroy(GetComponent<BoxCollider>(),0.3f);
            //GetComponent<Rigidbody>().useGravity = false;
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            //GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
