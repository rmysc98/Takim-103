using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    [SerializeField] GameObject upperCase;
    [SerializeField] Transform closedPosTransform;
    [SerializeField] Transform openedPosTransform;
    [SerializeField] InspectSettings inspectSettingsScript;
    bool isOpen;

    bool isLeftOpen;
    bool isRightOpen;
    bool canOpenable;

    [SerializeField] GameObject bookPref;

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
        if (GameManager.Instance.isInspectBusy) return;
        AudioManager.Instance.PlaySFX("box_open");

        isOpen = true;
        GameManager.Instance.isInspectBusy = true;
        upperCase.transform.DORotateQuaternion(openedPosTransform.rotation, 1).OnComplete(() =>
        {
            GameManager.Instance.isInspectBusy = false;
        });
        //transform.DOMove(transform.localPosition - new Vector3(0, 0.5f, -0.5f),1);
    }

    public void CloseCase()
    {
        if (GameManager.Instance.isInspectBusy) return;
        isOpen = false;
        GameManager.Instance.isInspectBusy = true;
        AudioManager.Instance.PlaySFX("box_close");


        upperCase.transform.DORotateQuaternion(closedPosTransform.rotation, 1).OnComplete(() =>
        {
            GameManager.Instance.isInspectBusy = false;
        });
        //transform.DOMove(GameManager.Instance.mainCamera.transform.position + GameManager.Instance.mainCamera.transform.forward * inspectSettingsScript.distance, 1);
    }

    public void CloseCaseImmediate()
    {
        upperCase.transform.rotation = closedPosTransform.rotation;
        isOpen = false;
    }

    public void TakeBook(GameObject placerGFX)
    {
        if (!isOpen) return;

        AudioManager.Instance.PlaySFX("grab");

        bookPref.SetActive(true);
        bookPref.transform.SetPositionAndRotation(placerGFX.transform.position, placerGFX.transform.rotation);
        placerGFX.SetActive(false);
    }
}
