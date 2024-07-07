using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    public Vector3 lastPosition;
    public Vector3 lastRotation;
    public void Interact()
    {
        //GameManager.Instance.ShowInspectMenu(gameObject);
    }
}
