using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguish : MonoBehaviour, IInteractable
{
    [SerializeField] ParticleSystem particleEffect;
    [SerializeField] GameObject lightObj;
    [SerializeField] AudioSource audioSource; // AudioSource bile�eni
    [SerializeField] AudioClip extinguishSound; // S�nd�rme sesi
    bool isActive = true;
    public void Interact()
    {
        
        if (isActive)
        {
            particleEffect.Stop();
            particleEffect.Clear();
            lightObj.SetActive(false);
            isActive = false;
<<<<<<< Updated upstream
            AudioManager.Instance.PlaySFX("blow");
=======

            // S�nd�rme sesi oynat
            if (audioSource && extinguishSound)
            {
                audioSource.PlayOneShot(extinguishSound);
            }

>>>>>>> Stashed changes
        }
        else
        {
            particleEffect.Play();
            lightObj.SetActive(true);
            isActive = true;
            AudioManager.Instance.PlaySFX("light");

        }
    }

    public void MouseState()
    {
        GameManager.Instance.SetInGameCursor("blow");
    }
}
