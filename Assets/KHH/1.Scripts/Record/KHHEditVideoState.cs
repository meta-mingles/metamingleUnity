using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KHHEditVideoState
{
    public static string MotionName //모션 이름
    {
        get { return PlayerPrefs.GetString($"{KHHEditData.VideoName}M", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHEditData.VideoName}M", value); }
    }
    public static float MotionChangeX   //모션 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MCX", value); }
    }
    public static float MotionChangeLeftX   //모션 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MCLX", value); }
    }
    public static float MotionChangeRightX  //모션 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MCRX", value); }
    }
    public static float MotionVChangeX  //모션 사운드 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MVCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MVCX", value); }
    }
    public static float MotionVChangeLeftX  //모션 사운드 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MVCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MVCLX", value); }
    }
    public static float MotionVChangeRightX //모션 사운드 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MVCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MVCRX", value); }
    }
    public static float MotionVVolume
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}MVV", 1f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}MVV", value); }
    }

    public static string SoundName  //사운드 이름
    {
        get { return PlayerPrefs.GetString($"{KHHEditData.VideoName}S", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHEditData.VideoName}S", value); }
    }
    public static float SoundChangeX    //사운드 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}SCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}SCX", value); }
    }
    public static float SoundChangeLeftX    //사운드 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}SCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}SCLX", value); }
    }
    public static float SoundChangeRightX   //사운드 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}SCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}SCRX", value); }
    }
    public static float SoundVolume
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}SV", 1f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}SV", value); }
    }

    public static string ImageName  //이미지 이름
    {
        get { return PlayerPrefs.GetString($"{KHHEditData.VideoName}I", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHEditData.VideoName}I", value); }
    }
    public static float ImageChangeX    //이미지 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}ICX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}ICX", value); }
    }
    public static float ImageChangeLeftX    //이미지 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}ICLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}ICLX", value); }
    }
    public static float ImageChangeRightX   //이미지 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoName}ICRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoName}ICRX", value); }
    }
}
