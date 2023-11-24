using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KHHNickName : MonoBehaviourPun
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI nickNameText;

    string nickName;
    // Start is called before the first frame update
    void Start()
    {
        //닉네임 변경
        if (photonView.IsMine)
            photonView.RPC("ChangeNickName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }

    private void FixedUpdate()
    {
        canvas.transform.LookAt(Camera.main.transform);
    }

    [PunRPC]
    void ChangeNickName(string nickName)
    {
        this.nickName = nickName;
        nickNameText.text = nickName;
    }
}
