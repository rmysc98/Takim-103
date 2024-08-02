using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InspectSettings))]
[RequireComponent(typeof(Outline))]
public class Pickup : MonoBehaviour, IInteractable
{
    public Vector3 lastPosition;
    public Vector3 lastRotation;

    public Transform ghostObject;


    public void EnablePhysics()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }
    public void DisablePhysics()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GetComponent<Collider>().enabled = false;
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }

    public void CreateGhostObject()
    {
        if (ghostObject == null)
        {
            Transform obj = Instantiate(transform, transform.position, Quaternion.identity);
            Destroy(obj.GetComponent<Pickup>());
            Destroy(obj.GetComponent<Rigidbody>());
            //Destroy(obj.GetComponent<MeshRenderer>());
            obj.GetComponent<Collider>().isTrigger = true;
            //obj.AddComponent<MeshRenderer>();
            obj.GetComponent<MeshRenderer>().material = GameManager.Instance.ghostMaterial;
            obj.gameObject.SetActive(false);
            obj.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            ghostObject = obj;
        }
    }

    public void Interact()
    {
        GameManager.Instance.ShowInspectMenu(GameManager.Instance.currentHighlightObject);
        
    }

    public void MouseState()
    {
        GameManager.Instance.SetInGameCursor("grab");
    }
}
