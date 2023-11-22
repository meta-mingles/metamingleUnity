using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHPhotonManager : MonoBehaviour
{
    public static KHHPhotonManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] CinemachineVirtualCamera m_CinemachineVirtualCamera;

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;

        player = PhotonNetwork.Instantiate("PlayerArmature", new Vector3(0.5f, -0.4f, 0.5f), new Quaternion(0, -90, 0, 0));
        m_CinemachineVirtualCamera.Follow = player.transform.GetChild(0);
        m_CinemachineVirtualCamera.enabled = true;
    }
}
