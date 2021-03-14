﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using PathCreation;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    PhotonView PV;

    CharacterController cc;
    private float velocity;
    private float gravity = 18.9f;
    float movementSpeed = 1;
    bool canMove = true;
    Vector3 moveDirection = Vector3.zero;
    public float walkingSpeed = 2f;
    public float runningSpeed = 3.5f;
    public float jumpSpeed = 8;

    bool isWalling;
    public float wallCimbingDuration;
    public int wallClimbingLayers;
    float normalizedTime;
    bool canWall;

    bool hasJumped;
    bool headHit;

    PathCreator pc;
    float distance = 0;

    public bool isFrozen;

    Vector3 impact = Vector3.zero;
    


    float baseFOV;
    float runFOV;

    public Camera cam;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            Destroy(cam.gameObject);
            return;
        }

        pc = GameObject.Find("Path").GetComponent<PathCreator>();
        hasJumped = false;
        wallClimbingLayers = 0;

        isFrozen = false;
        headHit = false;
        normalizedTime = 0;
        canWall = true;
        baseFOV = 60f;
        runFOV = 50f;
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        
        if (Input.GetKeyDown(KeyCode.E))
        {
            //distance = pc.path.GetClosestDistanceAlongPath(transform.position);
        }
        if (Input.GetKey(KeyCode.E))
        {
            distance += 5 * Time.deltaTime;
            PV.transform.position = pc.path.GetPointAtDistance(distance);
        }

        if (isFrozen)
        {
            if (!cc.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
                cc.Move(moveDirection * Time.deltaTime);
            }
            return;
        }

        bool IsWalking = Input.GetKey(KeyCode.LeftShift);

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * 0.75f, Color.green);

        if (!canWall)
            canWall = cc.isGrounded;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.75f))
        {   
            if (hit.transform.gameObject.layer == wallClimbingLayers)
            {
                isWalling = Input.GetKey(KeyCode.W) && !cc.isGrounded && canWall;
                Debug.Log(isWalling);
            }
            else
            {
                UpdateWallParams(false);
                isWalling = false;
            }
        }
        else
        {
            UpdateWallParams(false);
            isWalling = false;
        }

        if (isWalling)
            WallClimbing();
        else
            Move(IsWalking);

        //ChangeFOV(IsWalking);

        if (impact.magnitude > 0.2)
            cc.Move(impact * Time.deltaTime);

        impact = Vector3.Lerp(impact, Vector3.zero, 1f * Time.deltaTime);

        ChangeFOV(IsWalking);
    }

    private void LateUpdate()
    {
        if (!PV.IsMine)
            return;

        if (impact.magnitude < 0.2)
            hasJumped = cc.isGrounded;

        if (cc.isGrounded)
            impact = Vector3.zero;

    }

    public void LoadYetiSettings()
    {
        //TODO: change values like speed etc.
    }

    void Move(bool IsWalking)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run

        float curSpeedX = Input.GetAxis("Vertical");
        float curSpeedY = Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
        moveDirection *= canMove ? (IsWalking ? walkingSpeed : runningSpeed) : 0;


        if (Input.GetButton("Jump") && canMove && cc.isGrounded)
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = movementDirectionY;


        if (!cc.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        cc.Move((moveDirection) * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Trampoline" && !hasJumped)
        {
            AddImpact(cc.velocity, 40);
        }
        else
        {
            impact = Vector3.zero;
        }
    }

    void AddImpact(Vector3 dir, float force)
    {
        moveDirection.y = -6;
        dir.y = 15;

        impact += dir.normalized * force;
    }


    void WallClimbing()
    {
        if (normalizedTime <= wallCimbingDuration)
        {
            //Do climbing
            cc.Move(new Vector3(0, 0.08f, 0 * Time.deltaTime));
            normalizedTime += Time.deltaTime;
            Debug.Log(normalizedTime);
        }
        else
        {
            UpdateWallParams(false);
        }
    }

    void UpdateWallParams(bool check)
    {
        canWall = check;
        isWalling = false;
        normalizedTime = 0;
    }

    void ZipLineMove(PathCreator pc, float distance)
    {
        distance += 5 * Time.deltaTime;
        transform.position = pc.path.GetPointAtDistance(distance);      
    }


    void ChangeFOV(bool isRunning)
    {
        if (isRunning)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, runFOV, Time.deltaTime * 8f);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, Time.deltaTime * 8f);
    }
}
