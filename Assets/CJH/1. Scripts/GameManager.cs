using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //나의 플레이어 생성
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        //
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
