using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KHHVideoData
{
    static List<string> motionDatas = new List<string>();
    static List<string> soundDatas = new List<string>();
    static List<string> ImageDatas = new List<string>();

    public static string FilePath { get; set; }
    public static string FileMotionPath { get; set; }
    public static string FileSoundPath { get; set; }
    public static string FileImagePath { get; set; }

    public static void Open(string videoName)
    {
        FilePath = Application.persistentDataPath + "/" + videoName;
        FileMotionPath = FilePath + "/Motions";
        FileSoundPath = FilePath + "/Sounds";
        FileImagePath = FilePath + "/Images";
    }
}
