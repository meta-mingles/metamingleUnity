using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_Login : MonoBehaviour
{
    [Header("Login & Register")]
    public TMP_InputField loginId;
    public TMP_InputField loginPw;
    public TMP_InputField registerId;
    public TMP_InputField registerPw;

    public Button loginBt;
    Action onClickEvent;


    public string serverURL = "http://192.168.0.28/"; // 서버 URL을 여기에 입력

    // 회원가입 함수
    public IEnumerator RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL + "/register", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                //회원가입 실패
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("회원가입 성공!");
                // 회원가입 성공 시 추가 작업 수행
            }
        }
    }

    public void metamingleSignUp()
    {
        StartCoroutine(RegisterUser(registerId.text, registerPw.text));
    }

    // 로그인 함수
    public IEnumerator LogIn(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL + "/login", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);           //로그인 실패

                //UI 확인
            }
            else
            {
                Debug.Log("로그인 성공!");
                // 로그인 성공 시 추가 작업 수행
            }
        }
    }

    

    
    //로그인 버튼을 눌렀을 때 호출하는 함수
    public void metamingleLogIn()
    { 
        onClickEvent?.Invoke();
        StartCoroutine(LogIn(loginId.text, loginPw.text));
    }

    //PlayerPrefs를 활용한 데이터 저장
    public void Save()
    {
        PlayerPrefs.SetString("ID", loginId.text);
        PlayerPrefs.SetString("PW", loginPw.text);
    }
    public void Load()
    {
        if (PlayerPrefs.HasKey(""))
        {
            loginId.text = PlayerPrefs.GetString("ID");
            loginPw.text = PlayerPrefs.GetString("PW");
        }
    }
}
