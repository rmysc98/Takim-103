using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    public GameObject ghostObject;
    [SerializeField] LayerMask mask;

    [SerializeField] TextMeshProUGUI interactObjNameText;
    [SerializeField] GameObject nameHolderPanel;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.CurrentState == GameState.Inspecting)
            {
                GameManager.Instance.HideInspectMenu();
            }
            return;
        }

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        Debug.DrawRay(r.origin, r.direction * InteractRange, Color.red);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange, mask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (ghostObject == null)
                    {

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

}
