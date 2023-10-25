using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_Login : MonoBehaviour
{
    //[Header("Login & Register")]
    //public InputField inputId;
    //public InputField inputPw;


    ////public InputField inputId;

    //public void NetSignUp()
    //{
    //    HttpInfo info = new HttpInfo();

    //    info.Set(RequestType.POST, "/sign-up", (DownloadHandler downloadHandler) => {
    //        //Post 데이터 전송했을 때 서버로부터 응답 옵니다~
    //        print("NetSignUp : " + downloadHandler.text);
    //        //ProjectManager.instance.myInfo = JsonUtility.FromJson<ReceiveUserInfo>(downloadHandler.text);

    //        SceneManager.LoadScene("GameSceneFInal");

    //    });

    //    SignUpInfo signUpInfo = new SignUpInfo();

    //    signUpInfo.nickname = inputId.text;
        

    //    info.body = JsonUtility.ToJson(signUpInfo);

    //    HttpManager.Get().SendRequest(info);
    //}
}
