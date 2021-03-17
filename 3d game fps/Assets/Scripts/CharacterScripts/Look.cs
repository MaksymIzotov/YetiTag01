using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Look : MonoBehaviour
{
    public static bool cursorLocked = true;

    public Transform player;
    public Transform normalCam;
    public Transform handCam;
    public Transform rotator;

    public float sensitivity;
    float sensMultiplier;
    public float maxAngle;

    PhotonView pv;

    private Quaternion camCenter;

    void Start()
    {
        sensMultiplier = 10;
        camCenter = normalCam.localRotation; //set rotation origin for cameras to camCenter
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!pv.IsMine || GameObject.Find("Spawner").GetComponent<PlayerSpawner>().isPaused) return;

        SetY();
        SetX();

    }

    void SetY()
    {
        float t_input = Input.GetAxis("Mouse Y") * sensitivity * sensMultiplier * Time.deltaTime;
        Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
        Quaternion t_delta = normalCam.localRotation * t_adj;

        if (Quaternion.Angle(camCenter, t_delta) < maxAngle)
        {
            normalCam.localRotation = t_delta;
            handCam.localRotation = t_delta;
            rotator.localRotation = t_delta;
        }

    }

    void SetX()
    {
        float t_input = Input.GetAxis("Mouse X") * sensitivity * sensMultiplier * Time.deltaTime;
        Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
        Quaternion t_delta = player.localRotation * t_adj;
        player.localRotation = t_delta;
    }

}
