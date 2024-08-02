using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using System;
using Cinemachine;
using static UnityEditor.Progress;
using static Unity.Burst.Intrinsics.X86.Avx;

public enum GameState
{
    Playing,
    Holding,
    Inspecting,
    Paused
}


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    private GameState currentState;

    public delegate void OnStateChange(GameState newState);

    [HideInInspector] public GameObject currentInspectingObject;
    [HideInInspector] public GameObject currentHighlightObject;
    //[SerializeField] float displayInteractDistance = 1;
    [SerializeField] float displaySpeed = 0.8f;
    [SerializeField] CinemachineVirtualCamera cmv;

    public Camera mainCamera;
    [SerializeField] GameObject globalVolume;
    [SerializeField] GameObject player;
    [SerializeField] GameObject inspectLight;
    [SerializeField] LayerMask detailLayer;
    //public float InteractRange;

    float deltaRotationX;
    float deltaRotationY;
    [SerializeField] float rotateSpeed = 1f;

    //public Texture2D cursorNormal;
    //public Texture2D cursorClickable;
    //public Texture2D cursorDragable;

    [SerializeField] CursorTexture[] gameplayMouseIcons;
    [SerializeField] Image crosshairImage;
    public Material ghostMaterial;

    public bool isInspectBusy;
    private Tween fovTween;

    private void Start()
    {
        ChangeState(GameState.Playing);
    }

    public GameState CurrentState
    {
        get { return currentState; }
    }
    public void ChangeState(GameState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;

            if (currentState == GameState.Playing || currentState == GameState.Holding)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (currentState == GameState.Inspecting)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }


    public void ShowInspectMenu(GameObject Obj)
    {
        SetInGameCursor("none");
        ChangeState(GameState.Inspecting);

        Obj.GetComponent<Pickup>().DisablePhysics();
        globalVolume.SetActive(true);
        
        Obj.layer = LayerMask.NameToLayer("Object");
        List<Transform> transforms = new(Obj.GetComponent<InspectSettings>().objectParts);
        foreach (Transform child in transforms)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Object");
        }


        currentInspectingObject = Obj;
        float distanceValue = Obj.GetComponent<InspectSettings>().distance;
        Obj.transform.DOMove(mainCamera.transform.position + mainCamera.transform.forward * distanceValue, 1);


        Vector3 direction = player.transform.position - Obj.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Obj.transform.DORotateQuaternion(targetRotation, 1);

        inspectLight.SetActive(true);

        //Obj.transform.position = mainCamera.transform.position + mainCamera.transform.forward * displayInteractDistance;
        //Obj.transform.rotation = Quaternion.identity;
    }
    public void HideInspectMenu()
    {
        SetInGameCursor("none");
        ChangeState(GameState.Playing);
        globalVolume.SetActive(false);
        currentInspectingObject.layer = LayerMask.NameToLayer("Default");
        List<Transform> transforms = new(currentInspectingObject.GetComponent<InspectSettings>().objectParts);
        foreach (Transform child in transforms)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }


        currentInspectingObject.transform.DORotate(currentInspectingObject.GetComponent<Pickup>().lastRotation, displaySpeed - 0.02f);
        currentInspectingObject.transform.DOMove(currentInspectingObject.GetComponent<Pickup>().lastPosition, displaySpeed).OnComplete(() =>
        {
            currentInspectingObject.GetComponent<Pickup>().EnablePhysics();
            currentInspectingObject = null;
        });
        inspectLight.SetActive(false);

        if (currentInspectingObject.TryGetComponent(out Case component))
        {
            component.CloseCaseImmediate();
        }

        //currentInspectingObject.transform.position = currentInspectingObject.GetComponent<Pickup>().lastPosition;
        //currentInspectingObject.transform.eulerAngles = currentInspectingObject.GetComponent<Pickup>().lastRotation;
    }

    private void Update()
    {
        if (CurrentState == GameState.Inspecting)
        {
            if (isInspectBusy) return;

            deltaRotationX = -Input.GetAxis("Mouse X");
            deltaRotationY = Input.GetAxis("Mouse Y");

            if (Input.GetMouseButton(0))
            {
                currentInspectingObject.transform.rotation =
                    Quaternion.AngleAxis(deltaRotationX * rotateSpeed, transform.up) *
                    Quaternion.AngleAxis(deltaRotationY * rotateSpeed, transform.right) *
                    currentInspectingObject.transform.rotation;

                currentInspectingObject.transform.eulerAngles = new Vector3(currentInspectingObject.transform.eulerAngles.x, currentInspectingObject.transform.eulerAngles.y, 0);
            }

        }
    }

    public void SetInGameCursor(string state)
    {
        CursorTexture cursor = Array.Find(gameplayMouseIcons, x => x.cursorName == state);

        if (cursor == null)
        {
            crosshairImage.enabled = false;
        }
        else
        {
            crosshairImage.enabled = true;
            crosshairImage.sprite = cursor.cursorSprite;
        }
    }

    public void FOVSetter(float target = 140, float duration = 1f)
    {
        if (fovTween != null)
        {
            fovTween.Kill();
        }

        float initialFOV = cmv.m_Lens.FieldOfView;

        fovTween = DOTween.To(
            () => cmv.m_Lens.FieldOfView,
            x => cmv.m_Lens.FieldOfView = x, 
            target, 
            duration 
        )
        .SetEase(Ease.OutCubic)
        .SetUpdate(true).OnComplete(() =>
        {
            FOVSetter(60);
        });


    }


}
