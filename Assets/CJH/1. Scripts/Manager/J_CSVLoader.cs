using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class J_CSVLoader : MonoBehaviour
{
    public static J_CSVLoader instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
        

    public List<T> Parse<T>(string fileName) where T : new()
    {        
        string path = Application.streamingAssetsPath + "/TestData/" + fileName + ".csv";

        //전체 읽어 오기
        string stringData = File.ReadAllText(path);

        return ParseString<T>(stringData);
    }

    public List<T> ParseString<T>(string stringData) where T : new()
    {
        //T 자료형으로 List 만든다
        List<T> list = new List<T>();
              
        //엔터를 기준으로 한줄씩 자르기
        string[] lines = stringData.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] temp = lines[i].Split("\r");
            lines[i] = temp[0];
        }

        //변수 나누기
        string[] variableName = lines[0].Split(",");

        //값 나누기
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(",");

            T data = new T();

            for (int j = 0; j < variableName.Length; j++)
            {
                //variableName[0] = 이름, variableName[1] = 전화번호, variableName[2] = 이메일
                //T에는 이름, 전화번호, 이메일로 변수가 있다. 해당 변수의 정보를 가져오자
                System.Reflection.FieldInfo fieldInfo = typeof(T).GetField(variableName[j]);
                //int.parse, byte.parse와 같은 종류 알아내기
                TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                //data의 각 변수에 값을 넣자
                if (values[j].Length > 0)
                {
                    fieldInfo.SetValue(data, typeConverter.ConvertFrom(values[j]));
                }
            }
            list.Add(data);
        }
        return list;
    }
}