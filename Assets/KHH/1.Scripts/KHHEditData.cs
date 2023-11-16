using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KHHEditData
{
    //static List<string> motionDatas = new List<string>();
    //static List<string> soundDatas = new List<string>();
    //static List<string> ImageDatas = new List<string>();

    public static string VideoTitle { get; set; }
    public static string FilePath { get; set; }
    public static string FileMotionPath { get; set; }
    public static string FileSoundPath { get; set; }
    public static string FileImagePath { get; set; }
    public static string FileVideoPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RockVR/Video/"; } }

    public static void Open(string videoTitle)
    {
        VideoTitle = videoTitle;
        FilePath = Application.persistentDataPath + "/" + videoTitle;
        FileMotionPath = FilePath + "/Motions";
        FileSoundPath = FilePath + "/Sounds";
        FileImagePath = FilePath + "/Images";
    }

    public static void Close()
    {
        VideoTitle = "";
        FilePath = "";
        FileMotionPath = "";
        FileSoundPath = "";
        FileImagePath = "";
    }

    public static bool ChangeVideoTitle(string title)
    {
        //중복되는 타이틀 존재 확인
        if (System.IO.Directory.Exists(Application.persistentDataPath + "/" + title)) return false;

        //폴더 이동
        //System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/" + title);
        System.IO.Directory.Move(FilePath, Application.persistentDataPath + "/" + title);

        //모션
        ChangePlayerPrefString($"{VideoTitle}M", $"{title}M");
        ChangePlayerPrefFloat($"{VideoTitle}MCX", $"{title}MCX");
        ChangePlayerPrefFloat($"{VideoTitle}MCLX", $"{title}MCLX");
        ChangePlayerPrefFloat($"{VideoTitle}MCRX", $"{title}MCRX");
        ChangePlayerPrefFloat($"{VideoTitle}MVCX", $"{title}MVCX");
        ChangePlayerPrefFloat($"{VideoTitle}MVCLX", $"{title}MVCLX");
        ChangePlayerPrefFloat($"{VideoTitle}MVCRX", $"{title}MVCRX");
        ChangePlayerPrefFloat($"{VideoTitle}MVV", $"{title}MVV");

        //사운드
        ChangePlayerPrefString($"{VideoTitle}S", $"{title}S");
        ChangePlayerPrefFloat($"{VideoTitle}SCX", $"{title}SCX");
        ChangePlayerPrefFloat($"{VideoTitle}SCLX", $"{title}SCLX");
        ChangePlayerPrefFloat($"{VideoTitle}SCRX", $"{title}SCRX");
        ChangePlayerPrefFloat($"{VideoTitle}SV", $"{title}SV");

        //이미지
        ChangePlayerPrefString($"{VideoTitle}I", $"{title}I");
        ChangePlayerPrefFloat($"{VideoTitle}ICX", $"{title}ICX");
        ChangePlayerPrefFloat($"{VideoTitle}ICLX", $"{title}ICLX");
        ChangePlayerPrefFloat($"{VideoTitle}ICRX", $"{title}ICRX");

        VideoTitle = title;
        FilePath = Application.persistentDataPath + "/" + title;
        FileMotionPath = FilePath + "/Motions";
        FileSoundPath = FilePath + "/Sounds";
        FileImagePath = FilePath + "/Images";
        return true;
    }

    static void ChangePlayerPrefString(string key, string newKey)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);
            PlayerPrefs.SetString(newKey, value);
            PlayerPrefs.DeleteKey(key);
        }
    }

    static void ChangePlayerPrefFloat(string key, string newKey)
    {
        if (PlayerPrefs.HasKey(key))
        {
            float value = PlayerPrefs.GetFloat(key);
            PlayerPrefs.SetFloat(newKey, value);
            PlayerPrefs.DeleteKey(key);
        }
    }
}
