using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DeepL;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Test_deepl : MonoBehaviour
{
    public Change_language get_language;
    private Translator translator;
    public Button KEButton;
    public Button EKButton;
    public TMP_InputField inputText;
    public TMP_InputField outputText;
    string input_language;
    string output_language;

    async void StartTranslation(string text, string selected_language)
    {
        string translatedText = await Translate(text);
        outputText.text = translatedText;
        Debug.Log("번역된 텍스트: " + translatedText);
    }
    async Task<string> Translate(string text)
    {
        var translations = await translator.TranslateTextAsync(
        new[] { text }, null, selected_language);

        Debug.Log("식별된 언어");
        Debug.Log(translations[0].DetectedSourceLanguageCode); // "JA"

        return translations[0].Text;
    }
    string selected_language;
    void KEOnButtonClicked()
    {
        if (get_language.iskorea)
        {
            selected_language = "ko";
        }
        else
        {
            selected_language = "EN-GB";
        }
        Debug.Log("버튼출력");
        Debug.Log(get_language.iskorea);

        string textValue = inputText.text;

        Debug.Log("Button Clicked! Text is: " + textValue);
        StartTranslation(textValue, selected_language);
    }
    void Start()
    {
        var authKey = "39774928-946a-9d79-a580-148a5a61b7b5:fx"; // Replace with your key
        translator = new Translator(authKey);
        KEButton.onClick.AddListener(KEOnButtonClicked);
        Debug.Log("테스트 시작");
    }
}
