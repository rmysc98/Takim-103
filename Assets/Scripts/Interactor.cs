using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

interface IInteractable
{
    public void Interact();
    public void MouseState();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public float placeRange;
    public GameObject ghostObject;
    [SerializeField] LayerMask mask;
    [SerializeField] LayerMask placeMask;

    [SerializeField] TextMeshProUGUI interactObjNameText;
    [SerializeField] GameObject nameHolderPanel;

    bool isHoldingObject;
    GameObject holdObject;
    [SerializeField] Transform holdPosition;
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform ghostObjectsPool;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.CurrentState == GameState.Inspecting)
            {
                if (GameManager.Instance.isInspectBusy) return;

                GameManager.Instance.HideInspectMenu();
            }
            if (GameManager.Instance.CurrentState == GameState.Holding)
            {
                
                Ray ray = new Ray(InteractorSource.position, InteractorSource.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, placeRange, placeMask))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        isHoldingObject = false;
                        GameManager.Instance.ChangeState(GameState.Playing);
                        holdObject.GetComponent<Pickup>().EnablePhysics();
                        holdObject.GetComponent<Rigidbody>().isKinematic = false;
                        holdObject.transform.position = ghostObject.transform.position;

                        ghostObject.gameObject.SetActive(false);
                        GameManager.Instance.SetInGameCursor("normal");
                        ghostObject.transform.SetParent(ghostObjectsPool);
                        holdObject.transform.parent = null;
                        holdObject = null;
                        ghostObject = null;
                        return;
                    }

                    if (!ghostObject.activeSelf) ghostObject.SetActive(true);
                    ghostObject.transform.SetParent(playerOrientation);
                    ghostObject.transform.localRotation = Quaternion.identity;


                    Vector3 proposedPosition = hit.point;
                    Collider[] colliders = Physics.OverlapSphere(proposedPosition, ghostObject.GetComponent<Collider>().bounds.size.y, placeMask);

                    if (colliders.Length == 0)
                    {
                        ghostObject.transform.position = proposedPosition;
                    }
                    else
                    {

                        Vector3 adjustedPosition = proposedPosition + new Vector3(0, ghostObject.GetComponent<Collider>().bounds.size.y, 0);
                        ghostObject.transform.position = adjustedPosition;
                    }

                }
                else
                {
                    if (ghostObject.activeSelf) ghostObject.SetActive(false);
                }

            }
            return;
        }

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        Debug.DrawRay(r.origin, r.direction * InteractRange, Color.red);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange, mask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.MouseState();
                if (Input.GetMouseButtonDown(0))
                {
                    if (hitInfo.collider.gameObject.GetComponent<Pickup>() == null) return;

                    if (!isHoldingObject)
                    {
                        isHoldingObject = true;
                        holdObject = hitInfo.collider.gameObject;
                        GameManager.Instance.ChangeState(GameState.Holding);
                        hitInfo.collider.gameObject.GetComponent<Pickup>().CreateGhostObject();
                        hitInfo.collider.gameObject.GetComponent<Pickup>().DisablePhysics();
                        ghostObject = hitInfo.collider.gameObject.GetComponent<Pickup>().ghostObject.gameObject;
                        hitInfo.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                        StartCoroutine(MoveTransformToTarget(hitInfo.collider.gameObject.transform, holdPosition, 1));
                        hitInfo.collider.gameObject.transform.DORotateQuaternion(holdPosition.rotation,1);
                        
                        ApplyOutline(GameManager.Instance.currentHighlightObject, false);
                        ghostObject.gameObject.SetActive(true);
                        GameManager.Instance.SetInGameCursor("none");
                    }
                    return;
                }

                if (GameManager.Instance.currentHighlightObject && hitInfo.transform.gameObject != GameManager.Instance.currentHighlightObject)
                {
                    ApplyOutline(GameManager.Instance.currentHighlightObject, false);
                }
                SetNewCurrentInteractable(hitInfo.collider.gameObject);
                DisplayName(hitInfo.collider.name);
                //&& GameManager.Instance.CurrentState == GameState.Playing
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObj.Interact();
                    ApplyOutline(GameManager.Instance.currentHighlightObject, false);
                }
            }
            else
            {
                DisableCurrentInteractable();
            }
        }
        else
        {
            DisableCurrentInteractable();
        }
    }

    void SetNewCurrentInteractable(GameObject Obj)
    {
        if (Obj == GameManager.Instance.currentHighlightObject) return;
        //Debug.Log("new current");
        GameManager.Instance.currentHighlightObject = Obj;
        ApplyOutline(GameManager.Instance.currentHighlightObject, true);

    }

    void DisableCurrentInteractable()
    {
        if (GameManager.Instance.currentHighlightObject)
        {
            ApplyOutline(GameManager.Instance.currentHighlightObject, false);
            GameManager.Instance.currentHighlightObject = null;
            DisplayName(false);
            GameManager.Instance.SetInGameCursor("normal");
        }
    }

    void ApplyOutline(GameObject Obj, bool enable)
    {
        Obj.GetComponent<Outline>().enabled = enable;
    }

    void DisplayName(string name)
    {
        nameHolderPanel.SetActive(true);
        interactObjNameText.text = name;
    }
    void DisplayName(bool enable)
    {
        if (enable == false)
        {
            nameHolderPanel.SetActive(false);
        }
    }


    IEnumerator MoveTransformToTarget(Transform move, Transform target, float duration = 1.0f)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            move.position = Vector3.Lerp(move.position, target.position, t);

            yield return null;
        }

        move.position = target.position;

        move.SetParent(target);
    }
}
