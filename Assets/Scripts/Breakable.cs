using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float breakForceThreshold = 5.0f;
    [SerializeField] GameObject gfx;
    [SerializeField] GameObject brokenGfx;
<<<<<<< Updated upstream
    [SerializeField] GameObject newObject;
    bool isBroken;
=======
    [SerializeField] AudioSource audioSource; // Ses kaynaðý
    [SerializeField] AudioClip breakSound; // Kýrýlma sesi
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
            AudioManager.Instance.PlaySFX("vase");
=======

            // Kýrýlma sesini çal
            if (audioSource != null && breakSound != null)
            {
                audioSource.PlayOneShot(breakSound);
            }
            //GetComponent<BoxCollider>().enabled = false;
>>>>>>> Stashed changes
            Destroy(GetComponent<BoxCollider>(),0.3f);
            
            if (newObject != null)
            {
                newObject.SetActive(true);
            }
        }
    }
}




