using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float breakForceThreshold = 5.0f;
    [SerializeField] GameObject gfx;
    [SerializeField] GameObject brokenGfx;
    [SerializeField] GameObject newObject;
    bool isBroken;

    void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;

        if (collisionForce > breakForceThreshold)
        {
            if (isBroken) return;

            isBroken = true;
            Debug.Log("Nesne kýrýldý! Çarpýþma þiddeti: " + collisionForce);
            brokenGfx.SetActive(true);
            gfx.SetActive(false);
            AudioManager.Instance.PlaySFX("vase");
            Destroy(GetComponent<BoxCollider>(),0.3f);
            
            if (newObject != null)
            {
                newObject.SetActive(true);
            }
        }
    }
}
