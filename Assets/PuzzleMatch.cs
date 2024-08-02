using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMatch : MonoBehaviour
{
    [SerializeField] Use[] parts;
    [SerializeField] Transform door;
    public void ControlDoor()
    {
        foreach (var item in parts)
        {
            if (item.isCorrect == false) return;
        }
        door.DOMoveY(door.transform.position.y - 15, 4).SetEase(Ease.InBack);
        AudioManager.Instance.PlaySFX("opendoor");

    }
}
