using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    [SerializeField] GameObject upperCase;
    [SerializeField] Transform closedPosTransform;
    [SerializeField] Transform openedPosTransform;
    bool isOpen;

    bool isLeftOpen;
    bool isRightOpen;
    bool canOpenable;

    public void ChangeLockState(string lockname, Transform lockTransform)
    {
        if (lockname == "left")
        {
            if(isLeftOpen)
            {
                isLeftOpen = false;
                lockTransform.DOLocalRotate(new Vector3(0, 0, 0), 1);
            }
            else
            {
                isLeftOpen = true;
                lockTransform.DOLocalRotate(new Vector3(0, 0, 150), 1);
            }
        }
        else if (lockname == "right")
        {
            if (isRightOpen)
            {
                isRightOpen = false;
                lockTransform.DOLocalRotate(new Vector3(0, 0, 0), 1);
            }
            else
            {
                isRightOpen = true;
                lockTransform.DOLocalRotate(new Vector3(0, 0, 150), 1);
            }
        }

        
        if (!isOpen)
        {
            if (isLeftOpen && isRightOpen)
            {
                canOpenable = true;
            }
            else
            {
                canOpenable = false;
            }
        }
    }

    public void OpenClose()
    {
        if (!canOpenable) return;

        if (isOpen) CloseCase();
        else OpenCase();
    }

    public void OpenCase()
    {
        if (!canOpenable) return;

        isOpen = true;
        GameManager.Instance.isInspectBusy = true;
        upperCase.transform.DORotateQuaternion(openedPosTransform.rotation, 1).OnComplete(() =>
        {
            GameManager.Instance.isInspectBusy = false;
        });
    }

    public void CloseCase()
    {
        isOpen = false;
        GameManager.Instance.isInspectBusy = true;
        upperCase.transform.DORotateQuaternion(closedPosTransform.rotation, 1).OnComplete(() =>
        {
            GameManager.Instance.isInspectBusy = false;
        });
    }
}
