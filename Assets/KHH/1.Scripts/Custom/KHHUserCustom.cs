using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class KHHColorlData
{
    public float r;
    public float g;
    public float b;
}

[System.Serializable]
public class KHHMaterialData
{
    public string name;
    public List<KHHColorlData> colorDatas;
    public Color ColorA { get { return new Color(colorDatas[0].r, colorDatas[0].g, colorDatas[0].b); } }
    public Color ColorB { get { return new Color(colorDatas[1].r, colorDatas[1].g, colorDatas[1].b); } }
    public Color ColorC { get { return new Color(colorDatas[2].r, colorDatas[2].g, colorDatas[2].b); } }
}

[System.Serializable]
public class KHHCategoryData
{
    public string category; //카테고리 이름    
    public int itemIndex; //현 카테고리에서 사용하는 아이템
    public List<KHHMaterialData> materialDatas; //아이템이 사용하는 색상

    public void Reset()
    {
        itemIndex = 0;
        foreach (var materialData in materialDatas)
            materialData.colorDatas.Clear();
        materialDatas.Clear();
    }
}

[System.Serializable]
public class KHHUserCustomData
{
    public List<KHHCategoryData> datas;
}

public static class KHHUserCustom
{
    static KHHUserCustomData customData;
    static KHHCategoryData curCatData;

    static bool isLoaded = false;
    public static bool HasData { get; set; } 

    static void Init()
    {
        customData = new KHHUserCustomData();
        customData.datas = new List<KHHCategoryData>();
        for (int i = 0; i < KHHCustomizationDemo.CustomCategories.Count; i++)
        {
            var category = KHHCustomizationDemo.CustomCategories[i];
            var data = new KHHCategoryData();
            data.category = category;
            data.itemIndex = 0;
            data.materialDatas = new List<KHHMaterialData>();
            customData.datas.Add(data);
        }
    }

    public static void Clear()
    {
        foreach (var data in customData.datas)
            data.Reset();
    }

    public static void SetCategory(string category)
    {
        if (category.Equals("outfit"))
        {
            foreach (var data in customData.datas)
            {
                if (data.category.Equals("top") || data.category.Equals("bottom"))
                {
                    data.Reset();
                }
            }
        }
        else if (category.Equals("top") || category.Equals("bottom"))
        {
            foreach (var data in customData.datas)
            {
                if (data.category.Equals("outfit"))
                {
                    data.Reset();
                }
            }
        }

        curCatData = customData.datas.Find(x => x.category == category);
    }

    public static void SetItem(int itemIndex)
    {
        curCatData.itemIndex = itemIndex;
        curCatData.materialDatas.Clear();
    }

    public static void SetColor(string material, int index, Color color)
    {
        var materialData = curCatData.materialDatas.Find(x => x.name == material);
        if (materialData == null)
        {
            materialData = new KHHMaterialData();
            materialData.name = material;
            materialData.colorDatas = new List<KHHColorlData>();
            for (int i = 0; i < 3; i++)
                materialData.colorDatas.Add(new KHHColorlData());
            curCatData.materialDatas.Add(materialData);
        }

        var colorData = materialData.colorDatas[index];
        colorData.r = color.r;
        colorData.g = color.g;
        colorData.b = color.b;
    }

    public static async void SaveData(Action action)
    {
        var json = JsonUtility.ToJson(customData);
        FileStream fileStream = null;
        try
        {
            fileStream = new FileStream(Application.persistentDataPath + "/customData.json", FileMode.Create);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            await fileStream.WriteAsync(bytes, 0, bytes.Length);
            await PostJson(bytes);
        }
        catch (Exception e)
        {
            Debug.Log(e); //Debug.LogError(e);
        }
        finally
        {
            HasData = true;
            if (fileStream != null)
                fileStream.Close();
            action?.Invoke();
        }
    }

    static async Task PostJson(byte[] jsonData)
    {
        // HttpClient 객체 생성
        using (HttpClient client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://metaverse.ohgiraffers.com:8080/avatar");

            request.Content = new ByteArrayContent(jsonData);
            request.Content.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Authentication", HttpManager.instance.token);

            // POST 요청 전송
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            // 응답 처리
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

    public static async Task<KHHUserCustomData> LoadData()
    {
        if (isLoaded) return customData;

        Init();
        string json = await GetJson();

        if (string.IsNullOrEmpty(json))
        {
            HasData = false;
            return customData;
        }

        JsonUtility.FromJsonOverwrite(json, customData);
        isLoaded = true;
        HasData = true;

        return customData;
    }

    static async Task<string> GetJson()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "http://metaverse.ohgiraffers.com:8080/avatar");
                request.Headers.Add("Authentication", HttpManager.instance.token);

                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                // 응답 처리
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    Debug.Log($"Error: {response.StatusCode}");
                    if (!File.Exists(Application.persistentDataPath + "/customData.json"))
                        return string.Empty;

                    byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/customData.json");
                    var json = System.Text.Encoding.UTF8.GetString(bytes);
                    return json;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Error: {e}");
                if (!File.Exists(Application.persistentDataPath + "/customData.json"))
                    return string.Empty;

                byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/customData.json");
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                return json;
            }
        }
    }

    public static byte[] string_to_hex_array(string charstr)
    {
        string[] split = charstr.Split(new char[] { ' ' });
        List<byte> bList = new List<byte>();

        foreach (string str in split) bList.Add(byte.Parse(str, NumberStyles.HexNumber));

        return bList.ToArray();
    }
}
