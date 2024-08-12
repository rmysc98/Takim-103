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
    [SerializeField] AudioSource audioSource; // Ses kayna��
    [SerializeField] AudioClip breakSound; // K�r�lma sesi
>>>>>>> Stashed changes

    void OnCollisionEnter(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;

        if (collisionForce > breakForceThreshold)
        {
            if (isBroken) return;

            isBroken = true;
            Debug.Log("Nesne k�r�ld�! �arp��ma �iddeti: " + collisionForce);
            brokenGfx.SetActive(true);
            gfx.SetActive(false);
<<<<<<< Updated upstream
            AudioManager.Instance.PlaySFX("vase");
=======

            // K�r�lma sesini �al
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




