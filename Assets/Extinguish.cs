using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguish : MonoBehaviour, IInteractable
{
    [SerializeField] ParticleSystem particleEffect;
    [SerializeField] GameObject lightObj;
    bool isActive = true;
    public void Interact()
    {
        if (isActive)
        {
            particleEffect.Stop();
            particleEffect.Clear();
            lightObj.SetActive(false);
            isActive = false;
        }
        else
        {
            particleEffect.Play();
            lightObj.SetActive(true);
            isActive = true;
        }
    }

    
}
