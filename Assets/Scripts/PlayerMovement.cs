using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    private bool isWalking;
    private bool wasWalking;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing && GameManager.Instance.CurrentState != GameState.Holding) return;
        MyInput();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing && GameManager.Instance.CurrentState != GameState.Holding)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }
        
        MovePlayer();
        CheckWalkingSound();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }



    private void MovePlayer()
    {
        //if (rb.isKinematic == true)
        //    rb.isKinematic = false;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.velocity = moveDirection * moveSpeed;

        //rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        //rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, rb.velocity.x, moveSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, rb.velocity.z, moveSpeed));
    }

    private void CheckWalkingSound()
    {
        // Eðer hareket ediyorsa isWalking true olacak, deðilse false
        isWalking = rb.velocity.magnitude > 0.1f;

        if (isWalking && !wasWalking)
        {
            // Yürümeye baþladýysa sesi baþlat
            AudioManagerr.Instance.Playsfx("Walk");
        }
        else if (!isWalking && wasWalking)
        {
            // Durduysa sesi durdur
            AudioManagerr.Instance.StopSfx("Walk");
        }

        // Önceki durum güncellenir
        wasWalking = isWalking;
    }
}


















