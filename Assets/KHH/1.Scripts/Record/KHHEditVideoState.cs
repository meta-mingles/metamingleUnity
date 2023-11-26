using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KHHEditVideoState
{
    public static string MotionName //모션 이름
    {
        get { return PlayerPrefs.GetString($"{KHHEditData.VideoTitle}M", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHEditData.VideoTitle}M", value); }
    }
    public static float MotionChangeX   //모션 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MCX", value); }
    }
    public static float MotionChangeLeftX   //모션 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MCLX", value); }
    }
    public static float MotionChangeRightX  //모션 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MCRX", value); }
    }
    public static float MotionVChangeX  //모션 사운드 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MVCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MVCX", value); }
    }
    public static float MotionVChangeLeftX  //모션 사운드 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MVCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MVCLX", value); }
    }
    public static float MotionVChangeRightX //모션 사운드 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MVCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MVCRX", value); }
    }
    public static float MotionVVolume
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}MVV", 1f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}MVV", value); }
    }

    public static string SoundName  //사운드 이름
    {
        get { return PlayerPrefs.GetString($"{KHHEditData.VideoTitle}S", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHEditData.VideoTitle}S", value); }
    }
    public static float SoundChangeX    //사운드 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}SCX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}SCX", value); }
    }
    public static float SoundChangeLeftX    //사운드 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}SCLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}SCLX", value); }
    }
    public static float SoundChangeRightX   //사운드 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}SCRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}SCRX", value); }
    }
    public static float SoundVolume
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}SV", 1f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}SV", value); }
    }

    public static string ImageName  //이미지 이름
    {
        get { return PlayerPrefs.GetString($"{KHHEditData.VideoTitle}I", string.Empty); }
        set { PlayerPrefs.SetString($"{KHHEditData.VideoTitle}I", value); }
    }
    public static float ImageChangeX    //이미지 위치변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}ICX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}ICX", value); }
    }
    public static float ImageChangeLeftX    //이미지 좌측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}ICLX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}ICLX", value); }
    }
    public static float ImageChangeRightX   //이미지 우측변화값
    {
        get { return PlayerPrefs.GetFloat($"{KHHEditData.VideoTitle}ICRX", 0f); }
        set { PlayerPrefs.SetFloat($"{KHHEditData.VideoTitle}ICRX", value); }
    }
}
