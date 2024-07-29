using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    [SerializeField] GameObject currentHighlightObject; // debug sonras� serialize kald�r�lcak.

    [SerializeField] TextMeshProUGUI interactObjNameText;
    [SerializeField] GameObject nameHolderPanel;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        //RaycastHit hit;
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        Debug.DrawRay(r.origin, r.direction * InteractRange, Color.red);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Extinguish extinguish))
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    extinguish.Interact();
                    //ApplyOutline(currentHighlightObject, false);
                    //GameManager.Instance.ShowInspectMenu(hitInfo.collider.gameObject);
                }
            }
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (currentHighlightObject && hitInfo.transform.gameObject != currentHighlightObject)
                {
                    ApplyOutline(currentHighlightObject, false);
                }
                SetNewCurrentInteractable(hitInfo.collider.gameObject);
                DisplayName(hitInfo.collider.name);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //interactObj.Interact();
                    // Buras� d�zeltilmeli, �ncelemenin gamemanager'de olmamas� gerekiyor gibi g�z�k�yor.
                    ApplyOutline(currentHighlightObject, false);
                    GameManager.Instance.ShowInspectMenu(hitInfo.collider.gameObject);
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
        if (Obj == currentHighlightObject) return;
        //Debug.Log("new current");
        currentHighlightObject = Obj;
        ApplyOutline(currentHighlightObject, true);

    }

    void DisableCurrentInteractable()
    {
        if (currentHighlightObject)
        {
            ApplyOutline(currentHighlightObject, false);
            currentHighlightObject = null;
            DisplayName(false);
        }
    }

    void ApplyOutline(GameObject Obj, bool enable) => Obj.GetComponent<Outline>().enabled = enable;

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
