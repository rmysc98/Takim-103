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
    [SerializeField] GameObject currentHighlightObject; // debug sonrasý serialize kaldýrýlcak.

    [SerializeField] TextMeshProUGUI interactObjNameText;
    [SerializeField] GameObject nameHolderPanel;

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        Debug.DrawRay(r.origin, r.direction * InteractRange, Color.red);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
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
                    // Burasý düzeltilmeli, Ýncelemenin gamemanager'de olmamasý gerekiyor gibi gözüküyor.
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
