using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseLock : MonoBehaviour
{
    [SerializeField] string thisLockName = "left";
    [SerializeField] Case caseScript;
    [SerializeField] Transform lockTransform;
    [SerializeField] AudioSource audioSource; // Ses kaynaðý
    [SerializeField] AudioClip lockSound; // Kilit sesi
    public void Interact()
    {
        //Debug.Log(transform.name);
        if (GameManager.Instance.CurrentState == GameState.Inspecting)
        {
            caseScript.ChangeLockState(thisLockName,lockTransform);
<<<<<<< Updated upstream
            AudioManager.Instance.PlaySFX("box_lock");

=======

            // Kilit sesini çal
            if (audioSource != null && lockSound != null)
            {
                audioSource.PlayOneShot(lockSound);
            }
>>>>>>> Stashed changes
        }
    }
}







