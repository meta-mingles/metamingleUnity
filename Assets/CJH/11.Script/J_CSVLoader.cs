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

        //��ü �о� ����
        string stringData = File.ReadAllText(path);

        return ParseString<T>(stringData);
    }

    public List<T> ParseString<T>(string stringData) where T : new()
    {
        //T �ڷ������� List �����
        List<T> list = new List<T>();
              
        //���͸� �������� ���پ� �ڸ���
        string[] lines = stringData.Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] temp = lines[i].Split("\r");
            lines[i] = temp[0];
        }

        //���� ������
        string[] variableName = lines[0].Split(",");

        //�� ������
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(",");

            T data = new T();

            for (int j = 0; j < variableName.Length; j++)
            {
                //variableName[0] = �̸�, variableName[1] = ��ȭ��ȣ, variableName[2] = �̸���
                //T���� �̸�, ��ȭ��ȣ, �̸��Ϸ� ������ �ִ�. �ش� ������ ������ ��������
                System.Reflection.FieldInfo fieldInfo = typeof(T).GetField(variableName[j]);
                //int.parse, byte.parse�� ���� ���� �˾Ƴ���
                TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                //data�� �� ������ ���� ����
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