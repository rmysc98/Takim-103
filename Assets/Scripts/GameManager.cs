using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using TMPro;

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
    [SerializeField] float displayInteractDistance = 1;
    [SerializeField] float displaySpeed = 0.8f;

    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject globalVolume;

    float deltaRotationX;
    float deltaRotationY;
    float rotateSpeed = 1f;

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
        }
    }


    public void ShowInspectMenu(GameObject Obj)
    {
        Obj.GetComponent<Collider>().enabled = false;
        globalVolume.SetActive(true);
        ChangeState(GameState.Inspecting);
        Obj.GetComponent<Pickup>().lastPosition = Obj.transform.position;
        Obj.GetComponent<Pickup>().lastRotation = Obj.transform.eulerAngles;
        Obj.layer = LayerMask.NameToLayer("Object");
        currentInspectingObject = Obj;
        Obj.transform.DOMove(mainCamera.transform.position + mainCamera.transform.forward * displayInteractDistance, 1);
        Obj.transform.DORotate(Vector3.zero, 1);
        Obj.GetComponent<Rigidbody>().useGravity = false;

        //Obj.transform.position = mainCamera.transform.position + mainCamera.transform.forward * displayInteractDistance;
        //Obj.transform.rotation = Quaternion.identity;
    }
    public void HideInspectMenu()
    {
        
        ChangeState(GameState.Playing);
        globalVolume.SetActive(false);
        currentInspectingObject.layer = LayerMask.NameToLayer("Default");
        currentInspectingObject.transform.DORotate(currentInspectingObject.GetComponent<Pickup>().lastRotation, displaySpeed - 0.02f);
        currentInspectingObject.transform.DOMove(currentInspectingObject.GetComponent<Pickup>().lastPosition, displaySpeed).OnComplete(() =>
        {
            currentInspectingObject.GetComponent<Collider>().enabled = true;
            currentInspectingObject.GetComponent<Rigidbody>().useGravity = true;
            currentInspectingObject = null;
        });

        //currentInspectingObject.transform.position = currentInspectingObject.GetComponent<Pickup>().lastPosition;
        //currentInspectingObject.transform.eulerAngles = currentInspectingObject.GetComponent<Pickup>().lastRotation;
    }

    private void Update()
    {
        if (CurrentState == GameState.Inspecting)
        {
            deltaRotationX = -Input.GetAxis("Mouse X");
            deltaRotationY = -Input.GetAxis("Mouse Y");

            if(Input.GetMouseButton(1))
            {
                currentInspectingObject.transform.rotation =
                    Quaternion.AngleAxis(deltaRotationX * rotateSpeed, transform.up) *
                    Quaternion.AngleAxis(deltaRotationY * rotateSpeed, transform.right) *
                    currentInspectingObject.transform.rotation;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                HideInspectMenu();

            }
        }
    }
}
