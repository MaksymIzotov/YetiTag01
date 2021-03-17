using System.Collections;
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
    public float walkingSpeed = 0;
    public float runningSpeed = 0;
    public float jumpSpeed = 8;

    bool isWalling;
    public float wallCimbingDuration;
    public int wallClimbingLayers;
    float normalizedTime;
    bool canWall;


    bool hasJumped;
    bool headHit;

    [SerializeField] GameObject handYeti;
    [SerializeField] GameObject handHuman;

    PathCreator pc;
    float distance = 0;
    bool isZippin;
    bool justZipped;

    public bool isFrozen;

    Vector3 impact = Vector3.zero;
    


    float baseFOV;
    float runFOV;

    public Camera cam;
    public Camera handCam;

    bool isForward;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            Destroy(cam.gameObject);
            Destroy(handCam.gameObject);
            MoveToLayer(handHuman.transform,0);
            MoveToLayer(handYeti.transform, 0);
            return;
        }

        UpdateHnds();
        if (gameObject.layer == 10)
            runningSpeed = 9;

        isZippin = false;
        hasJumped = false;
        wallClimbingLayers = 0;

        justZipped = false;
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


        if (justZipped)
            justZipped = !cc.isGrounded;

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

        Vector3 climbingPos = new Vector3(transform.position.x,transform.position.y -0.5f,transform.position.z);

        Debug.DrawRay(climbingPos, transform.forward * 0.75f, Color.green);

        if (!canWall)
            canWall = cc.isGrounded;

        if (Physics.Raycast(climbingPos, transform.forward, out hit, 0.75f))
        {
            if (hit.transform.gameObject.layer == wallClimbingLayers)
            {
                if (!isWalling)
                {
                    isWalling = Input.GetKey(KeyCode.W) && !cc.isGrounded && canWall;
                    Debug.Log(isWalling);
                    if (isWalling)
                        if (moveDirection.y > 0)
                            moveDirection.y = 5;
                        else
                            moveDirection.y = 3.5f;
                }
            }
            else
            {
                if (isWalling)
                    UpdateWallParams(false);
            }
        }
        else
        {
            if (isWalling)
                UpdateWallParams(false);
        }

        if (isWalling)
            WallClimbing();

        if (!isZippin)
            Move(IsWalking);

        if (impact.magnitude > 0.2)
            cc.Move(impact * Time.deltaTime);

        impact = Vector3.Lerp(impact, Vector3.zero, 1f * Time.deltaTime);

        //ZIPLINE BELOW

        Collider[] objects;
        objects = Physics.OverlapSphere(cam.transform.position, 3);
        foreach (Collider n in objects)
        {
            if (n.gameObject.layer == 11)
            {
                pc = n.gameObject.GetComponent<PathCreator>();
                n.gameObject.GetComponent<Renderer>().material.color = Color.green;
                n.gameObject.GetComponent<ZipLineColor>().isGreen = true;
                break;
            }
            pc = null;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isZippin)
            {

                if (pc != null)
                {
                    distance = pc.path.GetClosestDistanceAlongPath(transform.position);
                    if (justZipped)
                    {
                        isForward = !isForward;
                    }
                    else
                    {
                        if (distance < pc.path.length / 2)
                            isForward = true;
                        else
                            isForward = false;
                    }

                    isZippin = true;
                    if (impact.magnitude > 0.2)
                        impact = Vector3.zero;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isZippin)
            {
                justZipped = true;
                isZippin = false;
                moveDirection.y = 5;
            }
        }

        if (isZippin)
        {
            ZipLineMove();
        }

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
        UpdateHnds();
        runningSpeed = 9;
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


        if (Input.GetButton("Jump") && canMove && cc.isGrounded && !isWalling)
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = movementDirectionY;


        if (!cc.isGrounded)
            if (!isWalling)
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
        if (moveDirection.y >= 2)
        {    
            moveDirection.y -= 2.5f * Time.deltaTime;
        }
        else
        {
            UpdateWallParams(false);
        }
    }

    void UpdateWallParams(bool check)
    {
        if (isWalling)
            moveDirection = transform.up * 2;

        canWall = check;
        isWalling = false;
    }

    void ZipLineMove()
    {
        Debug.Log(pc.path.length);

        transform.position = pc.path.GetPointAtDistance(distance);

        if (isForward)
            distance += 0.3f * Time.deltaTime;
        else
            distance -= 0.3f * Time.deltaTime;
        
        if(distance >= pc.path.length || distance <= 0)
        {
            isZippin = false;
            moveDirection.y = 5;
        }
    }


    void ChangeFOV(bool isRunning)
    {
        if (isRunning)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, runFOV, Time.deltaTime * 8f);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, Time.deltaTime * 8f);
    }

    public void UpdateHnds()
    {
        handYeti.SetActive(false);
        handHuman.SetActive(false);
        if (gameObject.layer == 10)
            handYeti.SetActive(true);
        else
            handHuman.SetActive(true);
    }

    void MoveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root)
            MoveToLayer(child, layer);
    }
}
