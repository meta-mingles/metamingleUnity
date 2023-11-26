using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class KHHNickName : MonoBehaviourPun
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI nickNameText;
    [SerializeField] GameObject enterTab;
    [SerializeField] Button enterBt;
    [SerializeField] float constDist;
    string nickName;
    // Start is called before the first frame update
    void Start()
    {
        //닉네임 변경
        if (photonView.IsMine)
            photonView.RPC("ChangeNickName", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        EnterUI();
        canvas.transform.LookAt(Camera.main.transform);
    }

    [PunRPC]
    void ChangeNickName(string nickName)
    {
        this.nickName = nickName;
        nickNameText.text = nickName;
    }
    //플레이어가 전광판의 일정거리안에 들어가면 EnterUI가 생성된다.
    public void EnterUI()
    {
        GameObject billboard = GameObject.Find("billboard");
        //플레이어와 전광판간의 거리를 잰다
        float distance = Vector3.Distance(billboard.transform.position, KHHPhotonManager.Instance.player.transform.position);
        //일정거리 안에 들어가면
        if (distance < constDist)
        {
            enterTab.SetActive(true);
        }
        else
        {
            enterTab.SetActive(false);
        }
        if (enterTab.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            GlobalValue.directVideoNo =  J_PlatformUIManager.Instance.videoNo;
            GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
            GlobalValue.CurSceneName = "VideoScene";
            PhotonNetwork.LeaveRoom();

            SceneManager.LoadScene("VideoScene");

            //씬이동을 했을 때 비디오가 바로 나와야함

        }
    }
}
