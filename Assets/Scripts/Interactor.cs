using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Coroutine objMoveCoroutine;
    [SerializeField] Transform debug;

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
                
                Ray ray = new(InteractorSource.position, InteractorSource.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, placeRange, placeMask))
                {
                    Vector3 proposedPosition = hit.point + new Vector3(0, 0.20f, 0);

                    if (Input.GetMouseButtonDown(0))
                    {
                        isHoldingObject = false;

                        if (objMoveCoroutine != null)
                        {
                            StopCoroutine(objMoveCoroutine);
                            objMoveCoroutine = null;
                        }

                        GameManager.Instance.ChangeState(GameState.Playing);
                        ghostObject.transform.SetParent(ghostObjectsPool);
                        holdObject.transform.SetParent(null);
                        holdObject.GetComponent<Pickup>().EnablePhysics();
                        holdObject.GetComponent<Rigidbody>().isKinematic = false;
                        holdObject.transform.position = proposedPosition;

                        ghostObject.SetActive(false);
                        GameManager.Instance.SetInGameCursor("normal");
                        holdObject = null;
                        ghostObject = null;
                        return;
                    }

                    if (!ghostObject.activeSelf) ghostObject.SetActive(true);
                    ghostObject.transform.SetParent(playerOrientation);
                    ghostObject.transform.localRotation = Quaternion.identity;
                    ghostObject.transform.position = proposedPosition;

                }
                else
                {
                    if (ghostObject.activeSelf) ghostObject.SetActive(false);
                }

            }
            return;
        }

        Ray r = new(InteractorSource.position, InteractorSource.forward);
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
                    Debug.Log("click + Pick");
                        isHoldingObject = true;
                        holdObject = hitInfo.collider.gameObject;
                        GameManager.Instance.ChangeState(GameState.Holding);
                        hitInfo.collider.gameObject.GetComponent<Pickup>().CreateGhostObject();
                        hitInfo.collider.gameObject.GetComponent<Pickup>().DisablePhysics();
                        ghostObject = hitInfo.collider.gameObject.GetComponent<Pickup>().ghostObject.gameObject;
                        hitInfo.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                        objMoveCoroutine = StartCoroutine(MoveTransformToTarget(hitInfo.collider.gameObject.transform, holdPosition, 1));
                        //hitInfo.collider.gameObject.transform.DORotateQuaternion(holdPosition.rotation,1);
                        
                        ApplyOutline(GameManager.Instance.currentHighlightObject, false);
                        ghostObject.SetActive(true);
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
        move.SetParent(target);
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            move.SetPositionAndRotation(Vector3.Lerp(move.position, target.position, t), Quaternion.Slerp(move.rotation, target.rotation, t));

            yield return null;
        }

        move.SetPositionAndRotation(target.position, target.rotation);
    }

}
