using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_Login : MonoBehaviour
{
    [Header("Login & Register")]
    public InputField inputId;
    public InputField inputPw;

    public string serverURL = "https://yourserver.com"; // 서버 URL을 여기에 입력

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
        StartCoroutine(RegisterUser(inputId.text, inputPw.text));
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
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Logged in successfully!");
                // 로그인 성공 시 추가 작업 수행
            }
        }
    }

    public void metamingleLogIn()
    {
        StartCoroutine(LogIn(inputId.text, inputPw.text));
    }
}
