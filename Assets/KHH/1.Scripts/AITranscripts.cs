using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AITranscripts : MonoBehaviour
{
    private string apiUrl_chat = "http://metaverse.ohgiraffers.com:8080/scenario/streaming";
    //private string apiUrl_chat = "https://7091-221-163-19-218.ngrok.io/chatbot/test_text";
    private string apiUrl = "https://metaverse.ohgiraffers.com:8080/chatbot/test_image";
    //private string apiUrl_chat = "https://bec3-221-163-19-218.ngrok-free.app/chatbot/test_text";
    //private string apiUrl = "https://bec3-221-163-19-218.ngrok-free.app/chatbot/test_image"; // FastAPI 서버의 엔드포인트 URL을 입력하세요.
    // private string apiUrl_chat = "http://192.168.0.77:8001/chatbot/test_text";
    // private string apiUrl = "http://192.168.0.77:8001/chatbot/test_image"; // FastAPI 서버의 엔드포인트 URL을 입력하세요.
    /*    private string apiUrl_chat = "http://127.0.0.1:8000/chatbot/test_text";
        private string apiUrl = "http://127.0.0.1:8000/chatbot/test_image"; // FastAPI 서버의 엔드포인트 URL을 입력하세요.*/

    public Button aiTranscriptsButton;
    public TextMeshProUGUI aiTranscriptsButtonText;
    public TMP_InputField chatInputField;
    public TMP_InputField transcriptsInputField;

    //public Button yourButton2;
    //public RawImage yourRawImage;

    bool isGeneratingBG = false;
    bool isGeneratingTS = false;

    // btn, btn2 에 클릭 이벤트가 발생하면 TaskOnClick, TaskOnClick2를 각각 실행하라.
    void Start()
    {
        aiTranscriptsButton.onClick.AddListener(AITranscriptsButtonEvent);

        //Button btn2 = yourButton2.GetComponent<Button>();
        //btn2.onClick.AddListener(TaskOnClick2);
    }

    void Update()
    {
        //if(isGeneratingBG)
        //{
        //    transcriptsInputField.text = "대본 작성 중...";
        //}
    }

    // 첫번째 Box안에 있는 텍스트를 Python으로 보내라.
    async void AITranscriptsButtonEvent()
    {
        string inputText = chatInputField.text;
        string json = "{\"text\":\"" + inputText + "\"}";

        aiTranscriptsButtonText.text = "작성중";
        aiTranscriptsButton.interactable = false;

        isGeneratingBG = true;
        isGeneratingTS = true;

        // JSON 데이터를 바이트 배열로 변환
        byte[] jsonData = Encoding.UTF8.GetBytes(json);
        ////Debug.Log("배경생성 요청");
        //StartCoroutine(PostImageFile(jsonData));
        ////Debug.Log("시나리오 생성 요청");
        //StartCoroutine(PostJson(jsonData));

        transcriptsInputField.text = "";

        try
        {
            await PostJson(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
        }
    }

    IEnumerator PostImageFile(byte[] jsonData)
    {
        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            void SaveImage(byte[] imageBytes, string fileName)
            {
                if (Directory.Exists(Path.GetDirectoryName(fileName)) == false)
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllBytes(fileName, imageBytes);
                Debug.Log("Image saved");
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {

                SaveImage(www.downloadHandler.data, Application.persistentDataPath + "/Images/test.jpg");
                //SaveImage(www.downloadHandler.data, "./Assets/Resources/Images/test.jpg");
                //Debug.Log("Success!!!!!");

                // 여기에서 responseText를 파싱하여 결과값을 추출
            }

            isGeneratingBG = false;
            if (isGeneratingBG == false && isGeneratingTS == false)
            {
                aiTranscriptsButtonText.text = "작성";
                aiTranscriptsButton.interactable = true;
            }

            KHHEditManager.Instance.BackgroundButtonEvent();
        }
    }

    async Task PostJson(byte[] jsonData)
    {
        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl_chat);

        //실시간 streaming 데이터 받도록
        request.Headers.Add("Accept", "text/event-stream");

        ////헤더에 토큰 추가 (현재 생략 가능)
        //request.Headers.Add("Authorization", 토큰);

        request.Content = new ByteArrayContent(jsonData);
        request.Content.Headers.Add("Content-Type", "application/json");

        try
        {
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream, Encoding.UTF8);

                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();

                    Debug.Log(line);  //지우기

                    const string prefix = "data:";

                    if (line.StartsWith(prefix))
                    {
                        line = line.Substring(prefix.Length);
                    }

                    transcriptsInputField.text += line;

                    if (string.IsNullOrEmpty(line)) continue;
                }
            }
            else
            {
                Debug.LogError($"Request failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
        }
        finally
        {
            Debug.Log("전송 끝");
            HandleServerResponse();
        }
    }

    void HandleServerResponse()
    {
        aiTranscriptsButtonText.text = "작성";
        aiTranscriptsButton.interactable = true;
    }

    //IEnumerator PostJson(byte[] jsonData)
    //{
    //    using (UnityWebRequest www = new UnityWebRequest(apiUrl_chat, "POST"))
    //    {
    //        www.uploadHandler = new UploadHandlerRaw(jsonData);
    //        www.downloadHandler = new DownloadHandlerBuffer();
    //        www.SetRequestHeader("Content-Type", "application/json");

    //        yield return www.SendWebRequest();

    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError(www.error);
    //        }
    //        else
    //        {
    //            string responseText = www.downloadHandler.text;
    //            //Debug.Log("Response from server: " + responseText);
    //            responseText = responseText.Replace("\\n", "\n").Replace("\\\"", "\"");
    //            transcriptsInputField.text = responseText;
    //            // 여기에서 responseText를 파싱하여 결과값을 추출
    //        }

    //        isGeneratingTS = false;
    //        if(isGeneratingBG == false && isGeneratingTS == false)
    //        {
    //            aiTranscriptsButtonText.text = "작성";
    //            aiTranscriptsButton.interactable = true;
    //        }
    //    }
    //}
}
