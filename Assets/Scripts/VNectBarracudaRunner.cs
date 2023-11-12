using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using System.Diagnostics;
using System.Threading;


/// <summary>
/// Define Joint points
/// </summary>
public class VNectBarracudaRunner : MonoBehaviour
{
    public bool IsTracking { get; set; }

    public VNectModel[] VNectModels;
    VNectModel VNectModel
    {
        get
        {
            foreach (var model in VNectModels)
                if (model.gameObject.activeSelf)
                    return model;
            return VNectModels[0];
        }
    }

    // Inspector 요소
    public NNModel NNModel;                                                // 모델
    public WorkerFactory.Type WorkerType = WorkerFactory.Type.Auto;        // 모델 진행 유형: Auto(Default)
    public bool Verbose = true;                                            // 결과 출력 여부
    public VideoCapture videoCapture;                                      // VideoCapture Script
    public int InputImageSize;                                             // Image Size = 448
    public int HeatMapCol;                                                 // HeatMapCol = 28
    public float KalmanParamQ;                                             // Kalman Param Q = 0.001
    public float KalmanParamR;                                             // Kalman Param R = 0.0015
    public bool UseLowPassFilter;                                          // Smoothing 사용 여부
    [Range(0, 0.5f)]
    public float LowPassParam = 0.5f;                                      // Smoothing일 때 parameter 값
    //public Text Msg;                                                       // Wait a minute
    public float WaitTimeModelLoad = 10f;                                  // 처음 대기시간 10초
    public Texture2D InitImg;                                              // 처음 화면 이미지

    // 초기 설정
    private Model _model;
    private IWorker _worker;
    private VNectModel.JointPoint[] jointPoints;

    // PredictPose 관련
    private const int JointNum = 24;
    private float InputImageSizeHalf;
    private float InputImageSizeF;
    private int HeatMapCol_Squared;
    private int HeatMapCol_Cube;
    private float ImageScale;
    private float[] heatMap2D;
    private float[] offset2D;
    private float[] heatMap3D;
    private float[] offset3D;
    private float unit;
    private int JointNum_Squared = JointNum * 2;
    private int JointNum_Cube = JointNum * 3;
    private int HeatMapCol_JointNum;
    private int CubeOffsetLinear;
    private int CubeOffsetSquared;

    private bool Lock = true;                                              // WaitLoad()에서 false로 바뀜
    // private float Countdown = 0;  


    private void Start()
    {
        IsTracking = false;
        // Initialize 
        HeatMapCol_Squared = HeatMapCol * HeatMapCol;                      // 28*28, feature map 1개 크기
        HeatMapCol_Cube = HeatMapCol * HeatMapCol * HeatMapCol;            // 28*28*28, 28개의 feature map 크기
        HeatMapCol_JointNum = HeatMapCol * JointNum;                       // 28*24, feature map 개수 * 관절 개수
        CubeOffsetLinear = HeatMapCol * JointNum_Cube;                     // 28*24*3, feature map 개수 * 관절 개수 * input image 개수
        CubeOffsetSquared = HeatMapCol_Squared * JointNum_Cube;            // 28*28*24*3, offset 크기

        heatMap2D = new float[JointNum * HeatMapCol_Squared];              // 24*28*28
        offset2D = new float[JointNum * HeatMapCol_Squared * 2];           // 24*28*28*2
        heatMap3D = new float[JointNum * HeatMapCol_Cube];                 // 24*28*28*28   실제 heatmap3D 출력 크기(672,28,28)
        offset3D = new float[JointNum * HeatMapCol_Cube * 3];              // 24*28*28*28*3 실제 offset3D 출력 크기(2016,28,28)
        unit = 1f / (float)HeatMapCol;                                     // unit = 1 / 28
        InputImageSizeF = InputImageSize;                                  // 448
        InputImageSizeHalf = InputImageSizeF / 2f;                         // 448 / 2 = 224
        ImageScale = InputImageSize / (float)HeatMapCol;                   // ImageScale = 448 / 28 = 16????


        // Disabel sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;                     // 화면 대기 시간 동안 화면이 꺼지지 않게 한다.

        // Init model                                                 
        _model = ModelLoader.Load(NNModel, Verbose);                       // 모델을 불러온다.
        _worker = WorkerFactory.CreateWorker(WorkerType, _model, Verbose); // 모델 실행 작업을 설정한다.


        StartCoroutine("WaitLoad");
    }

    private void Update()
    {
        if (!Lock)
        {
            UpdateVNectModel();
        }
    }

    private const string inputName_1 = "input.1";
    private const string inputName_2 = "input.4";
    private const string inputName_3 = "input.7";
    private IEnumerator WaitLoad()
    {
        // Input Images
        inputs[inputName_1] = new Tensor(InitImg);
        inputs[inputName_2] = new Tensor(InitImg);
        inputs[inputName_3] = new Tensor(InitImg);

        // 비동기적으로 모델을 실행한다.
        yield return _worker.StartManualSchedule(inputs);

        // Outputs 출력 값을 가져온다.
        for (var i = 2; i < _model.outputs.Count; i++)
        {
            b_outputs[i] = _worker.PeekOutput(_model.outputs[i]);
        }

        // Outputs 출력 데이터를 컴퓨터의 메모리로 다운로드한다.
        offset3D = b_outputs[2].data.Download(b_outputs[2].shape);
        heatMap3D = b_outputs[3].data.Download(b_outputs[3].shape);

        // 모델의 출력 데이터를 사용하지 않는다.(사용 후 메모리를 정리하는 코드)
        for (var i = 2; i < b_outputs.Length; i++)
        {
            b_outputs[i].Dispose();
        }

        // VNectModel을 초기화한다.
        jointPoints = VNectModel.Init();

        // 좌표를 추정한다.
        PredictPose();

        // 시간 지연을 처리한다. 몇 초 동안 기다렸다가 다음 단계를 진행한다.
        yield return new WaitForSeconds(WaitTimeModelLoad);

        // VideoCapture 초기화, Background의 크기 설정
        videoCapture.Init(InputImageSize, InputImageSize);
        Lock = false;
        //Msg.gameObject.SetActive(false);                     // Msg 없앤다.
    }
    /*
    private const string inputName_1 = "0";
    private const string inputName_2 = "1";
    private const string inputName_3 = "2";
    */

    private void UpdateVNectModel()
    {

        input = new Tensor(videoCapture.MainTexture);                          // New Image
        if (inputs[inputName_1] == null)                                       // 딕셔너리 비어 있으면 New Image를 1, 2, 3에 채운다.
        {
            inputs[inputName_1] = input;
            inputs[inputName_2] = new Tensor(videoCapture.MainTexture);
            inputs[inputName_3] = new Tensor(videoCapture.MainTexture);
        }
        else                                                                   // 딕셔너리가 비어 있지 않으면 3번 출력 데이터 사용하지 않는다.
        {                                                                      // 즉 3번 이미지 사용하지 않고, 2번을 3번으로 1번을 2번으로, 1번을 New Image로 설정한다.
            inputs[inputName_3].Dispose();

            inputs[inputName_3] = inputs[inputName_2];
            inputs[inputName_2] = inputs[inputName_1];
            inputs[inputName_1] = input;
        }

        StartCoroutine(ExecuteModelAsync());
    }

    /// <summary>
    /// Tensor has input image
    /// </summary>
    /// <returns></returns>
    /// Tensor has input image
    Tensor input = new Tensor();
    Dictionary<string, Tensor> inputs = new Dictionary<string, Tensor>() { { inputName_1, null }, { inputName_2, null }, { inputName_3, null }, };
    Tensor[] b_outputs = new Tensor[4];

    private IEnumerator ExecuteModelAsync()
    {
        // 비동기적으로 모델을 실행한다.
        yield return _worker.StartManualSchedule(inputs);

        IsTracking = true;

        // Outputs 출력 값을 가져온다.
        for (var i = 2; i < _model.outputs.Count; i++)
        {
            b_outputs[i] = _worker.PeekOutput(_model.outputs[i]);
        }

        // Outputs 출력 데이터를 컴퓨터의 메모리로 다운로드한다.
        offset3D = b_outputs[2].data.Download(b_outputs[2].shape);
        heatMap3D = b_outputs[3].data.Download(b_outputs[3].shape);

        // 모델의 출력 데이터를 사용하지 않는다.(사용 후 메모리를 정리하는 코드)
        for (var i = 2; i < b_outputs.Length; i++)
        {
            b_outputs[i].Dispose();
        }

        // 좌표를 추정한다.
        PredictPose();

    }

    /// <summary>
    /// Predict positions of each of joints based on network
    /// </summary>
    private void PredictPose()
    {
        // 28*28로 이루어진 feature map이 총 28개가 있는데, 이게 각 관절마다 있다. 즉, 24개의 관절에 대해 각각 28*28인 feature map 28개가 있다는 말이다.
        // 예를 들어 관절 1이 있다고 할 때, 28*28인 feature map이 28개 있는데 이 중 가장 높은 확률값을 가지는 곳의 좌표를 찾는다. 
        // 찾은 좌표를 maxXIndex, maxYIndex, maxZIndex라고 하겠다
        List<int> position_list = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 19 });  // 내가 찾고자 하는 관절들 번호
        var isPredict = 1;
        for (var j = 0; j < JointNum; j++)
        {
            var maxXIndex = 0;
            var maxYIndex = 0;
            var maxZIndex = 0;
            jointPoints[j].score3D = 0.0f;
            var jj = j * HeatMapCol;
            for (var z = 0; z < HeatMapCol; z++)
            {
                var zz = jj + z;
                for (var y = 0; y < HeatMapCol; y++)
                {
                    var yy = y * HeatMapCol_Squared * JointNum + zz;
                    for (var x = 0; x < HeatMapCol; x++)
                    {
                        float v = heatMap3D[yy + x * HeatMapCol_JointNum];
                        if (v > jointPoints[j].score3D)
                        {
                            jointPoints[j].score3D = v;
                            maxXIndex = x;
                            maxYIndex = y;
                            maxZIndex = z;
                        }
                    }
                }
            }
            // 구겨지지 않기 위한 장치
            if (position_list.Contains(j) && jointPoints[j].score3D < 0.15f)
            {
                isPredict = 0;
                break;
                //Debug.Log("NO" + j + " " + jointPoints[j].score3D);
            }
            //else
            //{
            //    Debug.Log("YES!" + j + " " + jointPoints[j].score3D);
            //}

            // 현재 추정된 좌표를 Now3D라고 하겠다. 좌표를 보정하는 계산식이다.
            jointPoints[j].Now3D.x = (offset3D[maxYIndex * CubeOffsetSquared + maxXIndex * CubeOffsetLinear + j * HeatMapCol + maxZIndex] + 0.5f + (float)maxXIndex) * ImageScale - InputImageSizeHalf;
            jointPoints[j].Now3D.y = InputImageSizeHalf - (offset3D[maxYIndex * CubeOffsetSquared + maxXIndex * CubeOffsetLinear + (j + JointNum) * HeatMapCol + maxZIndex] + 0.5f + (float)maxYIndex) * ImageScale;
            jointPoints[j].Now3D.z = (offset3D[maxYIndex * CubeOffsetSquared + maxXIndex * CubeOffsetLinear + (j + JointNum_Squared) * HeatMapCol + maxZIndex] + 0.5f + (float)(maxZIndex - 14)) * ImageScale;
        }

        // 탐색되지 않으면 이전 값으로 대체함.
        for (var j = 0; j < jointPoints.Length; j++)
        {
            if (isPredict == 1)
            {
                jointPoints[j].Save3D.x = jointPoints[j].Now3D.x;
                jointPoints[j].Save3D.y = jointPoints[j].Now3D.y;
                jointPoints[j].Save3D.z = jointPoints[j].Now3D.z;

            }
            else
            {
                jointPoints[j].Now3D.x = jointPoints[j].Save3D.x;
                jointPoints[j].Now3D.y = jointPoints[j].Save3D.y;
                jointPoints[j].Now3D.z = jointPoints[j].Save3D.z;
            }

        }

        // lc는 왼쪽 골반과 오른쪽 골반의 좌표의 중간값이다.
        var lc = (jointPoints[PositionIndex.rThighBend.Int()].Now3D + jointPoints[PositionIndex.lThighBend.Int()].Now3D) / 2f;

        // hip의 좌표를 계산한다. hip은 spine과 lc의 중간값이다.
        jointPoints[PositionIndex.hip.Int()].Now3D = (jointPoints[PositionIndex.abdomenUpper.Int()].Now3D + lc) / 2f;

        // neck의 좌표를 계산한다. neck은 rShldrBend와 lShldrBend의 중간값이다.
        jointPoints[PositionIndex.neck.Int()].Now3D = (jointPoints[PositionIndex.rShldrBend.Int()].Now3D + jointPoints[PositionIndex.lShldrBend.Int()].Now3D) / 2f;

        // head의 좌표를 계산한다.
        var cEar = (jointPoints[PositionIndex.rEar.Int()].Now3D + jointPoints[PositionIndex.lEar.Int()].Now3D) / 2f;                     // cEar는 lEar과 rEar의 중간값이다.
        var hv = cEar - jointPoints[PositionIndex.neck.Int()].Now3D;                                                                     // hv = cEar - Neck ↑
        var nhv = Vector3.Normalize(hv);                                                                                                 // nhv는 hv를 normalize 한 것이다. 방향 ↑
        var nv = jointPoints[PositionIndex.Nose.Int()].Now3D - jointPoints[PositionIndex.neck.Int()].Now3D;                              // nv = Nose - Neck ↗head
                                                                                                                                         // head의 좌표를 계산한다. head는 Neck + nhv * (nhv와 nv의 내적)이다. 이 의미는 nv를 nhv로 정사영 시켰을 때 길이를 길이가 1인 nhv에 곱한 것이다.
        jointPoints[PositionIndex.head.Int()].Now3D = jointPoints[PositionIndex.neck.Int()].Now3D + nhv * Vector3.Dot(nhv, nv);

        // spine의 좌표를 계산한다. spine은 abdomenUpper과 같다.
        jointPoints[PositionIndex.spine.Int()].Now3D = jointPoints[PositionIndex.abdomenUpper.Int()].Now3D;

        // Smoothing 과정. 관절 위치를 추정할 때 발생할 수 있는 노이즈와 불안정성을 줄이기 위한 것이다.
        // Smoothing 1. Kalman filter를 계산한다.
        foreach (var jp in jointPoints)
        {
            KalmanUpdate(jp);
        }

        // Smoothing 2. Low pass filter, Noise를 제거하기 위해 smoothing하는 과정이다.
        if (UseLowPassFilter)
        {
            foreach (var jp in jointPoints)
            {
                jp.PrevPos3D[0] = jp.Pos3D;
                for (var i = 1; i < jp.PrevPos3D.Length; i++)
                {
                    jp.PrevPos3D[i] = jp.PrevPos3D[i] * LowPassParam + jp.PrevPos3D[i - 1] * (1f - LowPassParam);
                }
                jp.Pos3D = jp.PrevPos3D[jp.PrevPos3D.Length - 1];
            }
        }
    }

    /// <summary>
    /// Kalman filter
    /// </summary>
    /// <param name="measurement">joint points</param>
    void KalmanUpdate(VNectModel.JointPoint measurement)
    {
        measurementUpdate(measurement);
        measurement.Pos3D.x = measurement.X.x + (measurement.Now3D.x - measurement.X.x) * measurement.K.x;
        measurement.Pos3D.y = measurement.X.y + (measurement.Now3D.y - measurement.X.y) * measurement.K.y;
        measurement.Pos3D.z = measurement.X.z + (measurement.Now3D.z - measurement.X.z) * measurement.K.z;
        measurement.X = measurement.Pos3D;
    }

    void measurementUpdate(VNectModel.JointPoint measurement)
    {
        measurement.K.x = (measurement.P.x + KalmanParamQ) / (measurement.P.x + KalmanParamQ + KalmanParamR);
        measurement.K.y = (measurement.P.y + KalmanParamQ) / (measurement.P.y + KalmanParamQ + KalmanParamR);
        measurement.K.z = (measurement.P.z + KalmanParamQ) / (measurement.P.z + KalmanParamQ + KalmanParamR);
        measurement.P.x = KalmanParamR * (measurement.P.x + KalmanParamQ) / (KalmanParamR + measurement.P.x + KalmanParamQ);
        measurement.P.y = KalmanParamR * (measurement.P.y + KalmanParamQ) / (KalmanParamR + measurement.P.y + KalmanParamQ);
        measurement.P.z = KalmanParamR * (measurement.P.z + KalmanParamQ) / (KalmanParamR + measurement.P.z + KalmanParamQ);
    }

}