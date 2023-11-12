using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHModelRecorder : MonoBehaviour
{
    public VNectModel[] models;
    public VNectModel Model
    {
        get
        {
            foreach (var model in models)
                if (model.gameObject.activeSelf)
                    return model;
            return null;
        }
    }
    public KHHScreenEditor screenEditor;

    bool isRecording = false;
    public bool IsRecording { get { return isRecording; } }

    //녹화
    List<string[]> recordData;
    float recordTime = 0.0f;

    private void LateUpdate()
    {
        if (Model != null && KHHRecordManager.Instance.StartRecord)
        {
            recordTime += Time.deltaTime;

            //모델 녹화
            string[] curJointData = new string[PositionIndex.Count.Int() + 1];
            curJointData[0] = recordTime.ToString();
            for (int i = 1; i < curJointData.Length; i++)
                curJointData[i] = $"{Model.JointPoints[i - 1].Pos3D.x}_{Model.JointPoints[i - 1].Pos3D.y}_{Model.JointPoints[i - 1].Pos3D.z}_{Model.JointPoints[i - 1].InverseRotation.x}_{Model.JointPoints[i - 1].InverseRotation.y}_{Model.JointPoints[i - 1].InverseRotation.z}_{Model.JointPoints[i - 1].InverseRotation.w}";
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

        ////현재의 위치 저장
        //string[] curJointData = new string[PositionIndex.Count.Int() + 1];
        //curJointData[0] = recordTime.ToString();
        //for (int i = 1; i < curJointData.Length; i++)
        //    curJointData[i] = $"{model.JointPoints[i - 1].Pos3D.x}_{model.JointPoints[i - 1].Pos3D.y}_{model.JointPoints[i - 1].Pos3D.z}_{model.JointPoints[i - 1].InverseRotation.x}_{model.JointPoints[i - 1].InverseRotation.y}_{model.JointPoints[i - 1].InverseRotation.z}_{model.JointPoints[i - 1].InverseRotation.w}";
        //recordData.Add(curJointData);
    }

    public void StopRecord(string filePath)
    {
        isRecording = false;

        //TestFileName = fileName;
        CSVManager.Instance.WriteCsv(filePath, recordData);
    }
}
