using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public enum GameState
{
    Playing,
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

    public GameObject currentInspectingObject;
    public GameObject currentHighlightObject;
    //[SerializeField] float displayInteractDistance = 1;
    [SerializeField] float displaySpeed = 0.8f;

    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject globalVolume;
    [SerializeField] GameObject player;
    [SerializeField] GameObject inspectLight;
    [SerializeField] LayerMask detailLayer;
    public float InteractRange;

    float deltaRotationX;
    float deltaRotationY;
    [SerializeField] float rotateSpeed = 1f;

    public Texture2D cursorNormal;
    public Texture2D cursorClickable;
    public Texture2D cursorDragable;

    public bool isInspectBusy;

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

            if (currentState == GameState.Playing)
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
        ChangeState(GameState.Inspecting);
        Obj.GetComponent<Rigidbody>().useGravity = false;
        Obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        Obj.GetComponent<Collider>().enabled = false;
        Obj.GetComponent<Pickup>().lastPosition = Obj.transform.position;
        Obj.GetComponent<Pickup>().lastRotation = Obj.transform.eulerAngles;
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
            currentInspectingObject.GetComponent<Collider>().enabled = true;
            currentInspectingObject.GetComponent<Rigidbody>().useGravity = true;
            currentInspectingObject = null;
        });
        inspectLight.SetActive(false);

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
}
