using UnityEngine;

public static class GlobalValue
{
    public static int directVideoNo { get; set; }
    public static SystemLanguage myLanguage
    {
        get
        {
            return (SystemLanguage)PlayerPrefs.GetInt("Language",
            Application.systemLanguage == SystemLanguage.Korean ? (int)SystemLanguage.Korean : (int)SystemLanguage.English);
        }
        set
        {
            PlayerPrefs.SetInt("Language", (int)value);
        }
    }
    public static string PrevSceneName { get; set; }
    public static string CurSceneName { get; set; }
}
