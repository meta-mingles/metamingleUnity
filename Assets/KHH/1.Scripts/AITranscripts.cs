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
    [System.Serializable]
    public class ApiResponse
    {
        public string apiStatus;
        public string message;
        public ImageData[] data;
    }

    [System.Serializable]

    public class ApiResponseSound
    {
        public string apiStatus;
        public string message;
        public SoundData[] data;
    }

    [System.Serializable]
    public class ImageData
    {
        public string title;
        public string imageUrl;
    }

    [System.Serializable]


    public class SoundData
    {
        public string title;
        public string soundUrl;
    }

    [System.Serializable]
    public class ApiResponseQuiz
    {
        public string apiStatus;
        public string message;
        public QuizData data;
    }

    [System.Serializable]
    public class QuizData
    {
        public string uuid;
    }

    private string apiUrl_chat = "http://metaverse.ohgiraffers.com:8080/scenario/streaming";
    private string apiUrl_image = "http://metaverse.ohgiraffers.com:8080/creative/image";
    private string apiUrl_sound = "http://metaverse.ohgiraffers.com:8080/creative/sound";
    private string apiUrl_quiz = "http://metaverse.ohgiraffers.com:8080/scenario/quiz";
    //private string apiUrl_chat = "http://192.168.0.5:8080/scenario/streaming";
    //private string apiUrl_image = "http://192.168.0.5:8080/creative/image";
    //private string apiUrl_sound = "http://192.168.0.5:8080/creative/sound";
    //private string apiUrl_quiz = "http://192.168.0.5:8080/scenario/quiz";

    public Button aiTranscriptsButton;
    public GameObject[] aiTranscriptsButtonTImages;
    public TMP_InputField chatInputField;
    public TMP_InputField transcriptsInputField;

    //public Button yourButton2;
    //public RawImage yourRawImage;

    //bool isGeneratingBG = false;
    //bool isGeneratingTS = false;

    // btn, btn2 에 클릭 이벤트가 발생하면 TaskOnClick, TaskOnClick2를 각각 실행하라.
    void Start()
    {
        aiTranscriptsButton.onClick.AddListener(AITranscriptsButtonEvent);
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

        aiTranscriptsButtonTImages[0].SetActive(false);
        aiTranscriptsButtonTImages[1].SetActive(true);
        aiTranscriptsButton.interactable = false;

        //isGeneratingBG = true;
        //isGeneratingTS = true;

        // JSON 데이터를 바이트 배열로 변환
        byte[] jsonData = Encoding.UTF8.GetBytes(json);
        ////Debug.Log("배경생성 요청");
        //StartCoroutine(PostImageFile(jsonData));
        ////Debug.Log("시나리오 생성 요청");
        //StartCoroutine(PostJson(jsonData));

        transcriptsInputField.text = "";

        try
        {
            StartCoroutine(PostImage(jsonData));
            StartCoroutine(PostSound(jsonData));
            StartCoroutine(PostQuiz(jsonData));   //퀴즈 생성 요청 추가
            await PostJson(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
        }
    }

    //IEnumerator PostImageFile(byte[] jsonData)
    //{
    //    using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
    //    {
    //        www.uploadHandler = new UploadHandlerRaw(jsonData);
    //        www.downloadHandler = new DownloadHandlerBuffer();
    //        www.SetRequestHeader("Content-Type", "application/json");

    //        yield return www.SendWebRequest();

    //        void SaveImage(byte[] imageBytes, string fileName)
    //        {
    //            if (Directory.Exists(Path.GetDirectoryName(fileName)) == false)
    //                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
    //            File.WriteAllBytes(fileName, imageBytes);
    //            Debug.Log("Image saved");
    //        }

    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError(www.error);
    //        }
    //        else
    //        {

    //            SaveImage(www.downloadHandler.data, Application.persistentDataPath + "/Images/test.jpg");
    //            //SaveImage(www.downloadHandler.data, "./Assets/Resources/Images/test.jpg");
    //            //Debug.Log("Success!!!!!");

    //            // 여기에서 responseText를 파싱하여 결과값을 추출
    //        }

    //        isGeneratingBG = false;
    //        if (isGeneratingBG == false && isGeneratingTS == false)
    //        {
    //            aiTranscriptsButtonText.text = "작성";
    //            aiTranscriptsButton.interactable = true;
    //        }

    //        KHHEditManager.Instance.BackgroundButtonEvent();
    //    }
    //}

    async Task PostJson(byte[] jsonData)
    {
        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl_chat);

        //실시간 streaming 데이터 받도록
        request.Headers.Add("Accept", "text/event-stream");

        ////헤더에 토큰 추가 (현재 생략 가능)
        request.Headers.Add("Authentication", HttpManager.instance.token);

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

                    // Check if the line contains " |" and replace it with a newline
                    if (line.Contains("|"))
                    {
                        /*line = line.Replace("|", "\n\n");*/
                        transcriptsInputField.text += "\n\n";
                        Debug.Log("good");
                        continue;
                    }

                    transcriptsInputField.text += line;

                    await Task.Delay(30);

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

    IEnumerator PostImage(byte[] jsonData)
    {
        using (UnityWebRequest www = new UnityWebRequest(apiUrl_image, "GET"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authentication", HttpManager.instance.token);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonString = www.downloadHandler.text;
                ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(jsonString);

                if (apiResponse != null && apiResponse.apiStatus == "SUCCESS")
                {
                    foreach (ImageData imageData in apiResponse.data)
                    {
                        yield return StartCoroutine(DownloadImage(imageData.imageUrl, imageData.title));
                    }
                }
                else
                {
                    Debug.LogError("이미지 저장 실패");
                }

                KHHEditManager.Instance.BackgroundButtonEvent();
            }
        }
    }

    IEnumerator DownloadImage(string imageUrl, string title)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Get the downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // Save the image with a unique filename
                string fileName = KHHEditData.FileImagePath + "/" + title + ".jpg";
                SaveData(texture.EncodeToJPG(), fileName);
            }
        }
    }

    IEnumerator PostSound(byte[] jsonData)
    {
        using (UnityWebRequest www = new UnityWebRequest(apiUrl_sound, "GET"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authentication", HttpManager.instance.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonString = www.downloadHandler.text;
                ApiResponseSound apiResponse = JsonUtility.FromJson<ApiResponseSound>(jsonString);

                if (apiResponse != null && apiResponse.apiStatus == "SUCCESS")
                {
                    foreach (SoundData soundData in apiResponse.data)
                    {
                        yield return StartCoroutine(DownloadSound(soundData.soundUrl, soundData.title));
                    }
                }
                else
                {
                    Debug.LogError("BGM 저장 실패");
                }

                KHHEditManager.Instance.SoundButtonEvent();
            }
        }
    }

    IEnumerator PostQuiz(byte[] jsonData)
    {
        using (UnityWebRequest www = new UnityWebRequest(apiUrl_quiz, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authentication", HttpManager.instance.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonString = www.downloadHandler.text;
                ApiResponseQuiz apiResponse = JsonUtility.FromJson<ApiResponseQuiz>(jsonString);

                if (apiResponse != null && apiResponse.apiStatus == "SUCCESS")
                {
                    PlayerPrefs.SetString($"{KHHEditData.VideoTitle}uuid", apiResponse.data.uuid);  //반환된 UUID PlayerPrefs 저장
                }
                else
                {
                    Debug.LogError("QUIZ 저장 실패");
                }

            }
        }
    }

    IEnumerator DownloadSound(string soundUrl, string title)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(soundUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string fileName = KHHEditData.FileSoundPath + "/" + title + ".wav";
                SaveData(www.downloadHandler.data, fileName);
            }
        }
    }

    void SaveData(byte[] dataBytes, string fileName)
    {
        if (Directory.Exists(Path.GetDirectoryName(fileName)) == false)
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        File.WriteAllBytes(fileName, dataBytes);
        Debug.Log("Data saved: " + fileName);
    }

    void HandleServerResponse()
    {
        aiTranscriptsButtonTImages[0].SetActive(true);
        aiTranscriptsButtonTImages[1].SetActive(false);
        aiTranscriptsButton.interactable = true;
    }
}