using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    float xRotation;
    float yRotation;
    public float speed;

    private bool isGameViewActive;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isGameViewActive = false;
    }

    void Update()
    {
        //if (isGameViewActive == false) //BUILDDE SIL
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        isGameViewActive = true;
        //    }
        //    return;
        //}
        if (GameManager.Instance.CurrentState != GameState.Playing && GameManager.Instance.CurrentState != GameState.Holding) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        Quaternion target = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);


        orientation.rotation = Quaternion.Slerp(orientation.rotation, target, Time.deltaTime * speed);
        orientation.rotation = Quaternion.Euler(0, orientation.rotation.eulerAngles.y, 0);

    }
}
