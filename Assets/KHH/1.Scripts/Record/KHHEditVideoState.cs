using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KHHEditVideoState
{
    public static string MotionName //모션 이름
    {
        get { return PlayerPrefs.GetString($"{KHHVideoData.VideoName}M", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHVideoData.VideoName}M", value); }
    }
    public static float MotionChangeX   //모션 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MCX", value); }
    }
    public static float MotionChangeLeftX   //모션 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MCLX", value); }
    }
    public static float MotionChangeRightX  //모션 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MCRX", value); }
    }
    public static float MotionVChangeX  //모션 사운드 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MVCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MVCX", value); }
    }
    public static float MotionVChangeLeftX  //모션 사운드 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MVCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MVCLX", value); }
    }
    public static float MotionVChangeRightX //모션 사운드 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MVCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MVCRX", value); }
    }
    public static float MotionVVolume
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}MVV", 1f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}MVV", value); }
    }

    public static string SoundName  //사운드 이름
    {
        get { return PlayerPrefs.GetString($"{KHHVideoData.VideoName}S", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHVideoData.VideoName}S", value); }
    }
    public static float SoundChangeX    //사운드 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}SCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}SCX", value); }
    }
    public static float SoundChangeLeftX    //사운드 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}SCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}SCLX", value); }
    }
    public static float SoundChangeRightX   //사운드 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}SCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}SCRX", value); }
    }
    public static float SoundVolume
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}SV", 1f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}SV", value); }
    }

    public static string ImageName  //이미지 이름
    {
        get { return PlayerPrefs.GetString($"{KHHVideoData.VideoName}I", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHVideoData.VideoName}I", value); }
    }
    public static float ImageChangeX    //이미지 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}ICX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}ICX", value); }
    }
    public static float ImageChangeLeftX    //이미지 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}ICLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}ICLX", value); }
    }
    public static float ImageChangeRightX   //이미지 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHVideoData.VideoName}ICRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHVideoData.VideoName}ICRX", value); }
    }
}
