using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AITranscripts : MonoBehaviour
{
    private string apiUrl_chat = "https://7091-221-163-19-218.ngrok.io/chatbot/test_text";
    private string apiUrl = "https://7091-221-163-19-218.ngrok.io/chatbot/test_image";
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
        if(isGeneratingBG)
        {
            transcriptsInputField.text = "대본 작성 중...";
        }
    }

    // 첫번째 Box안에 있는 텍스트를 Python으로 보내라.
    void AITranscriptsButtonEvent()
    {
        string inputText = chatInputField.text;
        //Debug.Log(inputText);
        string json = "{\"text\":\"" + inputText + "\"}";

        aiTranscriptsButtonText.text = "작성중";
        aiTranscriptsButton.interactable = false;

        isGeneratingBG = true;
        isGeneratingTS = true;

        // JSON 데이터를 바이트 배열로 변환
        byte[] jsonData = Encoding.UTF8.GetBytes(json);
        //Debug.Log("배경생성 요청");
        StartCoroutine(PostImageFile(jsonData));
        //Debug.Log("시나리오 생성 요청");
        StartCoroutine(PostJson(jsonData));
    }


    //// 두번째 Box안에 있는 텍스트를 Python으로 보내라.
    //void TaskOnClick2()
    //{
    //    // string json = "{\"text\":\"Image Test Hi!\"}";
    //    // string inputText = max_yourInputField.text;
    //    // Debug.Log(inputText);
    //    // string json = "{\"text\":\"" + inputText + "\"}";

    //    // JSON 데이터를 바이트 배열로 변환
    //    // byte[] jsonData = Encoding.UTF8.GetBytes(json);

    //    // StartCoroutine(PostImageFile(jsonData));


    //    // Texture2D[] textures = Resources.LoadAll<Texture2D>("Images");

    //    //Texture2D new_image = Resources.Load<Texture2D>("Images/test");
    //    //Debug.Log(new_image);

    //    // 이미지 중에서 랜덤으로 하나를 선택합니다.
    //    // Texture2D randomTexture = textures[Random.Range(0, textures.Length)];

    //    // 선택한 이미지를 Raw Image의 텍스처로 설정합니다.
    //    //yourRawImage.texture = randomTexture;
    //    //yourRawImage.texture = new_image;
    //}

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

    IEnumerator PostJson(byte[] jsonData)
    {
        using (UnityWebRequest www = new UnityWebRequest(apiUrl_chat, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                //Debug.Log("Response from server: " + responseText);
                responseText = responseText.Replace("\\n", "\n").Replace("\\\"", "\"");
                transcriptsInputField.text = responseText;
                // 여기에서 responseText를 파싱하여 결과값을 추출
            }

            isGeneratingTS = false;
            if(isGeneratingBG == false && isGeneratingTS == false)
            {
                aiTranscriptsButtonText.text = "작성";
                aiTranscriptsButton.interactable = true;
            }
        }
    }
}
