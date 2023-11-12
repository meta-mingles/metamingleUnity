using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KHHEditData
{
    //static List<string> motionDatas = new List<string>();
    //static List<string> soundDatas = new List<string>();
    //static List<string> ImageDatas = new List<string>();

    public static string VideoName { get; set; }
    public static string FilePath { get; set; }
    public static string FileMotionPath { get; set; }
    public static string FileSoundPath { get; set; }
    public static string FileImagePath { get; set; }
    public static string FileVideoPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RockVR/Video/"; } }

    public static void Open(string videoName)
    {
        VideoName = videoName;
        FilePath = Application.persistentDataPath + "/" + videoName;
        FileMotionPath = FilePath + "/Motions";
        FileSoundPath = FilePath + "/Sounds";
        FileImagePath = FilePath + "/Images";
    }
}
