using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

        wallClimbingLayers = 0;

        baseFOV = 60f;
        runFOV = 80f;
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);
        if (Input.GetKey(KeyCode.S))
            isRunning = false;

        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * 0.75f, Color.green);
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.75f))
        {

            // collision detected
            if (hit.transform.gameObject.layer == wallClimbingLayers)
            {
                isWalling = Input.GetKey(KeyCode.W);
                Debug.Log(isWalling);
            }
            else
            {
                //Nothing
            }
        }
        else
        {
            // no collision at all
        }

        Move(isRunning);
        ChangeFOV(isRunning);
    }

    void Move(bool isRunning)
    {
        if (!isWalling)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            // Press Left Shift to run

            float curSpeedX = Input.GetAxis("Vertical");
            float curSpeedY = Input.GetAxis("Horizontal");
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
            moveDirection *= canMove ? (isRunning ? runningSpeed : walkingSpeed) : 0;

            if (Input.GetButton("Jump") && canMove && cc.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!cc.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Move the controller
            cc.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            StartCoroutine("WallClimbing");
        }
    }

    public IEnumerator WallClimbing()
    {
        float duration = wallCimbingDuration; 

        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            //Do climbing
            normalizedTime += Time.deltaTime / duration;
        }
        isWalling = false;
        yield return null;
    }

    void ChangeFOV(bool isRunning)
    {
        if (isRunning)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, runFOV, Time.deltaTime * 8f);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, Time.deltaTime * 8f);
    }
}
