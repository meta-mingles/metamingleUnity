using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public J_TpsCamera tpsCamera;
    // Start is called before the first frame update
    void Start()
    {
        //OnPhotonSerializeView ȣ�� ��
        PhotonNetwork.SerializationRate = 10;

        //���� �÷��̾� ����
        GameObject myPlayer = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        tpsCamera.target = myPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
