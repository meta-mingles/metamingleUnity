using UnityEngine;
using TMPro;
using DeepL;
using System.Threading.Tasks;
using System;

public class Test_deepl : MonoBehaviour
{
    public static Test_deepl Instance { get; private set; }
    public Change_language get_language;
    private Translator translator;
    public TMP_InputField inputText;
    public TMP_InputField outputText;
    string input_language;
    string output_language;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task<string> StartTranslation(string text,string selected_language)
    {
        string translatedText = await Translate(text,selected_language);
        outputText.text = translatedText;
        Debug.Log("번역된 텍스트: " + translatedText);
        return translatedText;
    }
    async Task<string> Translate(string text,string selected_language)
    {
        Debug.Log("Selected language before assignment: " + selected_language);
        try
        {
            var translations = await translator.TranslateTextAsync(new[] { text }, null, selected_language);
            Debug.Log("Selected language after assignment: " + selected_language);
            if (translations != null && translations.Length > 0)
            {
                return translations[0].Text;
            }
            else
            {
                Debug.LogError("No translation received.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Translation error: " + ex.Message);
            return null;
        }
    }
    string selected_language;
    
    void Start()
    {
        var authKey = "39774928-946a-9d79-a580-148a5a61b7b5:fx"; // Replace with your key
        translator = new Translator(authKey);
        //KEButton.onClick.AddListener(KEOnButtonClicked);
        Debug.Log("테스트 시작");
    }
}
