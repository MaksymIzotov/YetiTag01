using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;

public class SetNickName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine) { return; }

        SetName();
    }

    private void SetName() => nameText.text = GetComponent<PhotonView>().Owner.NickName;
}
