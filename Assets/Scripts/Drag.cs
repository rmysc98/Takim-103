using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Drag : MonoBehaviour, IInteractable
{
    public float forceAmount = 500;

    Rigidbody selectedRigidbody;
    Camera targetCamera;
    Vector3 originalScreenTargetPosition;
    Vector3 originalRigidbodyPos;
    float selectionDistance;
    //[SerializeField] LayerMask DragLayerMask;

    void Start()
    {
        targetCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectedRigidbody)
        {
            selectedRigidbody = null;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                if (hit.collider.gameObject == gameObject)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedRigidbody = GetRigidbodyFromMouseClick(hit, ray);
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (selectedRigidbody)
        {
            Vector3 mousePositionOffset = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance)) - originalScreenTargetPosition;
            // Hýz sýnýrlarýný belirleyin
            float minVelocity = 0f;
            float maxVelocity = 1f;

            // Hesaplanan hýz vektörünü normalleþtirin ve sýnýrlayýn
            Vector3 calculatedVelocity = (originalRigidbodyPos + mousePositionOffset - selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
            float magnitude = Mathf.Clamp(calculatedVelocity.magnitude, minVelocity, maxVelocity);

            // Normalleþtirilmiþ hýzý tekrar uygulayýn
            selectedRigidbody.velocity = calculatedVelocity.normalized * magnitude;

        }
    }

    Rigidbody GetRigidbodyFromMouseClick(RaycastHit hitInfo, Ray ray)
    {

        if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
        {
            selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
            originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance));
            originalRigidbodyPos = hitInfo.collider.transform.position;
            return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
        }

        return null;
    }

    public void MouseState()
    {
        GameManager.Instance.SetInGameCursor("drag");
    }

    public void Interact()
    {
        //Debug.Log("");

    }
}