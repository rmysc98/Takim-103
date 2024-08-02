using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Use : MonoBehaviour, IInteractable
{
    public bool isCorrect;
    [SerializeField] int correctAngle;
    bool isUsing;

    public void Interact()
    {
        if (isUsing) return;
        isUsing = true;
        transform.DORotate(new Vector3(0, transform.eulerAngles.y + 45, 0), 1).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            isUsing = false;
            if (AreAnglesEqual(correctAngle, Mathf.RoundToInt(transform.localEulerAngles.y)))
            {
                isCorrect = true;
                transform.parent.GetComponent<PuzzleMatch>().ControlDoor();
            }
            else isCorrect = false;
            
        });
    }

    public void MouseState()
    {
        GameManager.Instance.SetInGameCursor("use");
    }


    bool AreAnglesEqual(float angle1, float angle2)
    {
        Debug.Log(angle1);
        Debug.Log(angle2);
        float normalizedAngle1 = NormalizeAngle(angle1);
        float normalizedAngle2 = NormalizeAngle(angle2);

        return normalizedAngle1 == normalizedAngle2;
    }

    float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
            angle += 360;

        return angle;
    }
}
