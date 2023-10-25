using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHModelRecorder : MonoBehaviour
{
    VNectModel model;

    bool isRecording = false;
    bool isPlaying = false;

    //녹화
    List<string[]> recordData;
    float recordTime = 0.0f;


    //재생
    List<float> timeList;
    Dictionary<float, List<(Vector3, Quaternion)>> recordList;
    float playTime = 0.0f;
    int curIdx = 0;

    [SerializeField] VideoCapture videoCapture;

    // Start is called before the first frame update
    void Start()
    {
        model = GetComponent<VNectModel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (model != null && isPlaying)
        {
            playTime += Time.deltaTime;
            if (playTime > timeList[timeList.Count - 1])
            {
                isPlaying = false;
                return;
            }

            //현재 시간에 가장 근접하는 키 데이터를 추론한다.
            for (int i = curIdx + 1; i < timeList.Count; i++)
            {
                if (timeList[i] > playTime)
                {
                    curIdx = i - 1;
                    break;
                }
            }

            float ratio = (playTime - timeList[curIdx]) / (timeList[curIdx + 1] - timeList[curIdx]);
            //모델 위치 조정
            for (int i = 0; i < model.JointPoints.Length; i++)
            {
                Vector3 pos = Vector3.Lerp(recordList[timeList[curIdx]][i].Item1, recordList[timeList[curIdx + 1]][i].Item1, ratio);
                Quaternion rot;
                if ((recordList[timeList[curIdx]][i].Item2.normalized == Quaternion.identity) && (recordList[timeList[curIdx + 1]][i].Item2.normalized == Quaternion.identity)) rot = recordList[timeList[curIdx + 1]][i].Item2;
                else rot = Quaternion.Lerp(recordList[timeList[curIdx]][i].Item2, recordList[timeList[curIdx + 1]][i].Item2, ratio);
                model.JointPoints[i].Pos3D = pos;
                model.JointPoints[i].InverseRotation = rot;
            }
        }
    }

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

    public void StopRecord()
    {
        isRecording = false;

        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        TestFileName = fileName;
        CSVManager.Instance.WriteCsv(fileName, recordData);
    }


    string TestFileName;
    public void LoadRecordData(string fileName = "")
    {
        videoCapture.CameraPlayStop();

        if (fileName == "")
            fileName = TestFileName;
        string[,] motionData = CSVManager.Instance.ReadCsv(fileName);

        timeList = new List<float>();
        recordList = new Dictionary<float, List<(Vector3, Quaternion)>>();

        for (int i = 1; i < motionData.GetLength(1); i++)
        {
            if (string.IsNullOrEmpty(motionData[0, i])) continue;
            float time = float.Parse(motionData[0, i]);
            List<(Vector3, Quaternion)> jointData = new List<(Vector3, Quaternion)>();
            for (int j = 1; j < motionData.GetLength(0); j++)
            {
                if (string.IsNullOrEmpty(motionData[j, i])) continue;
                string[] jointDatas = motionData[j, i].Split('_');
                if (jointDatas.Length != 7) continue;
                //Debug.Log(jointDatas[0] + "_" + jointDatas[1] + "_" + jointDatas[2]);
                Vector3 pos = new Vector3(float.Parse(jointDatas[0]), float.Parse(jointDatas[1]), float.Parse(jointDatas[2]));
                Quaternion rot = new Quaternion(float.Parse(jointDatas[3]), float.Parse(jointDatas[4]), float.Parse(jointDatas[5]), float.Parse(jointDatas[6]));
                jointData.Add((pos, rot));
            }

            timeList.Add(time);
            recordList.Add(time, jointData);
        }

        curIdx = 0;
    }

    public void StartPlay()
    {
        isPlaying = true;
        playTime = 0.0f;
    }

    public void StopPlay()
    {
        isPlaying = false;
    }
}
