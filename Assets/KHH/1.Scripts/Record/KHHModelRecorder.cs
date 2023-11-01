using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHModelRecorder : MonoBehaviour
{
    public VNectModel model;
    public KHHScreenEditor screenEditor;

    bool isRecording = false;

    //녹화
    List<string[]> recordData;
    float recordTime = 0.0f;

    private void LateUpdate()
    {
        if (model != null && isRecording)
        {
            recordTime += Time.deltaTime;

            //모델 녹화
            string[] curJointData = new string[PositionIndex.Count.Int() + 1];
            curJointData[0] = recordTime.ToString();
            for (int i = 1; i < curJointData.Length; i++)
                curJointData[i] = $"{model.JointPoints[i - 1].Pos3D.x}_{model.JointPoints[i - 1].Pos3D.y}_{model.JointPoints[i - 1].Pos3D.z}_{model.JointPoints[i - 1].InverseRotation.x}_{model.JointPoints[i - 1].InverseRotation.y}_{model.JointPoints[i - 1].InverseRotation.z}_{model.JointPoints[i - 1].InverseRotation.w}";
            recordData.Add(curJointData);
        }
    }

    //녹화 시작
    public void StartRecord()
    {
        isRecording = true;
        recordTime = 0.0f;
        recordData = new List<string[]>();
        string[] datas = new string[PositionIndex.Count.Int() + 1];
        datas[0] = "time";
        for (int i = 1; i < datas.Length; i++)
            datas[i] = ((PositionIndex)(i - 1)).ToString();
        recordData.Add(datas);

        //현재의 위치 저장
        string[] curJointData = new string[PositionIndex.Count.Int() + 1];
        curJointData[0] = recordTime.ToString();
        for (int i = 1; i < curJointData.Length; i++)
            curJointData[i] = $"{model.JointPoints[i - 1].Pos3D.x}_{model.JointPoints[i - 1].Pos3D.y}_{model.JointPoints[i - 1].Pos3D.z}_{model.JointPoints[i - 1].InverseRotation.x}_{model.JointPoints[i - 1].InverseRotation.y}_{model.JointPoints[i - 1].InverseRotation.z}_{model.JointPoints[i - 1].InverseRotation.w}";
        recordData.Add(curJointData);
    }

    public void StopRecord(string fileName)
    {
        isRecording = false;
        //TestFileName = fileName;
        CSVManager.Instance.WriteCsv(fileName, recordData);
    }
}
