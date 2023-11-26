using DeepL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TranslationManager : MonoBehaviour
{
    public static TranslationManager instance { get; private set; }

    private Translator translator;

    private string authKey = "39774928-946a-9d79-a580-148a5a61b7b5:fx";

    private void Awake()
    {   
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTranslator();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTranslator()
    {
        translator = new Translator(authKey);
    }

    public async Task<string> TranslateText(string text, string targetLanguage)
    {
        var translations = await translator.TranslateTextAsync(new[] {text}, null, targetLanguage);
        return translations[0].Text;
    }
    public async Task<string> DetectLanguage(string text)
    {
        try
        {
            var translations = await translator.TranslateTextAsync(new[] { text }, null, "EN"); // 'EN' is a placeholder target language
            return translations[0].DetectedSourceLanguageCode;
        }
        catch (Exception e)
        {
            Debug.LogError("Error detecting language: " + e.Message);
            return null; // or a default language code
        }
    }
}
