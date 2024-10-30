using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class PMovement : MonoBehaviour
{
    [Header("KeyBinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    
    [Header("movement")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public Transform orientation;
    //variables involving ground movement
    [SerializeField] public float groundDrag;
    public bool groundCheck;
    public LayerMask groundDef;
    public float PHeight = 3f;

    //movement input
    private float horiInput;
    private float vertInput;
    private Vector3 moveDir;
    //player RigidBody
    private Rigidbody rb;
    
    //Jump Variables
    [SerializeField] public float jumpForce;
    [SerializeField] public float airMultiplier;
    [SerializeField] public float jumpCooldown = .2f;
    [SerializeField] public float jumpExaustion;
    public bool readyToJump;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }
    
    void Update()
    {
        groundCheck = Physics.Raycast(transform.position, Vector3.down, PHeight * .5f + .3f, groundDef);
        KeyInput();
        if (groundCheck)
        {
            rb.drag = groundDrag;
        }
        else
            rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        SpeedControl();
        MovePlayer();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void KeyInput()
    {
        horiInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && groundCheck )
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        }
        
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * vertInput + orientation.right * horiInput;
        if(groundCheck)
            rb.AddForce(moveDir.normalized * (moveSpeed * 10f), ForceMode.Force);
        else
            rb.AddForce(moveDir.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVal = new Vector3(rb.velocity.x, 0f, rb.velocity.x);

        if (flatVal.magnitude > moveSpeed)
        {
            Vector3 limitMove = flatVal.normalized * moveSpeed;
            rb.velocity = new Vector3(limitMove.x, rb.velocity.y, limitMove.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        
    }

    private void JumpReset()
    {
        readyToJump = true;
    }
    
}
