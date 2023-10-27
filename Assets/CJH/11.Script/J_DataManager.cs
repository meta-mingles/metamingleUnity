using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class ShopInfo
{
    public string name;
    public int price;
    public bool sale;
    public string thumbnail;
}


[System.Serializable] // ����ȭ
public struct UserInfo //���� ����
{
    //id
    public string name;

    //ũ��������
    public string Creator;
}



[System.Serializable] // ����ȭ
public class ThumbnailInfo //���� ����
{

    public int shortFormNo; //���� ����

    public string title; //����

    public string thumbnailUrl; //����� url

    public string url; //url

    public string description; //���� ����

    public string memberName; // �г���

    public string date; //��¥

    public bool isInteractive; //���ͷ�Ƽ�� ����
}




[System.Serializable]
public struct JsonArray<T>
{
    public List<T> data;
}

public class J_DataManager : MonoBehaviour
{
    public static J_DataManager instance;

    //����� ����Ʈ ����
    public List<ThumbnailInfo> thumbnailInfoList = new List<ThumbnailInfo>();

    //�� ����
    public List<ShopInfo> shopInfoList = new List<ShopInfo>();

    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
    }

    void Update()
    {
        
    }
    public void SetRealVideoInfoListByCSV(string data)
    {
        thumbnailInfoList = J_CSVLoader.instance.ParseString<ThumbnailInfo>(data);
    }

    public void SetRealVideoInfoListByJSON(string data)
    {
        JsonArray<ThumbnailInfo> arrayData = JsonUtility.FromJson<JsonArray<ThumbnailInfo>>(data);
        thumbnailInfoList = arrayData.data;
    }


    public void SetShopInfoList(string data)
    {
        shopInfoList = J_CSVLoader.instance.ParseString<ShopInfo>(data);
    }
}
