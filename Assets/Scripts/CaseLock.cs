using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseLock : MonoBehaviour
{
    [SerializeField] string thisLockName = "left";
    [SerializeField] Case caseScript;
    [SerializeField] Transform lockTransform;
    public void Interact()
    {
        //Debug.Log(transform.name);
        if (GameManager.Instance.CurrentState == GameState.Inspecting)
        {
            caseScript.ChangeLockState(thisLockName,lockTransform);
        }
    }
}
