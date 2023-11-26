using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Import HolisticBarracuda
using MediaPipe.Holistic;

public class VisualizerTest : MonoBehaviour
{
    // Blend Shape
    //public GameObject HeadSlot;
    public bool Drawhand = true;
    public float EyeThreshold = 0.008f;
    public float TalkThreshold = 0.008f;
    public float SmileThreshold = 0f;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh skinnedMesh;
    private bool isSmile = false;
    private bool isCloseEyes = false;
    private bool isTalk = false;
    private float smileweight = 0f;
    private float eyeweight = 0f;
    private float mouthweight = 0f;

    // Hand Index
    private float KalmanParamQ = 0.001f;                                            // Kalman Param Q = 0.001
    private float KalmanParamR = 0.0015f;                                           // Kalman Param R = 0.0015
    public HandInformation[] handInformations;
    public VideoCapture videoCapture;

    private HandInformation.HandJoint[] handJoints;


    // Holistic Barracuda 속성들
    //[SerializeField, Range(0, 1)] float humanExistThreshold = 0.f;
    [SerializeField, Range(0, 1)] float handScoreThreshold = 0.7f;
    //[SerializeField] Shader handShader;
    [SerializeField] HolisticInferenceType holisticInferenceType = HolisticInferenceType.full;

    HolisticPipeline holisticPipeline;
    // Material 
    Material handMaterial;

    //int InputImageSize = 448;

    bool isInit = false;

    IEnumerator Start()
    {
        while (!videoCapture.SetEnd)
            yield return null;

        HandInformation handInformation = handInformations[0];
        foreach (var hand in handInformations) { if (hand.gameObject.activeSelf) { handInformation = hand; break; } }
        foreach (Transform transform in handInformation.transform)
        {
            if (transform.name.Contains("head") && transform.gameObject.activeSelf)
            {
                // BlendShape 
                skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
                skinnedMesh = transform.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                break;
            }
        }
        //skinnedMeshRenderer = HeadSlot.GetComponent<SkinnedMeshRenderer>();
        //skinnedMesh = HeadSlot.GetComponent<SkinnedMeshRenderer>().sharedMesh;

        // 웹캠 캡쳐하기 
        //videoCapture.Init(InputImageSize, InputImageSize);
        //Debug.Log("videocapture를 시작합니다");

        // 손가락 관절 초기화하기
        handJoints = handInformation.Init();

        // Make instance of HolisticPipeline
        holisticPipeline = new HolisticPipeline();
        //handMaterial = new Material(handShader);

        isInit = true;
    }

    private void Update()
    {
        if (!isInit) return;
        CloseEyes();
        Smiling();
        Talking();
    }

    void LateUpdate()
    {
        if (!isInit) return;
        // Inference. Switchable inference type anytime.
        holisticPipeline.ProcessImage(videoCapture.MainTexture, holisticInferenceType);
    }

    void OnRenderObject()
    {
        if (!isInit) return;
        if (holisticInferenceType == HolisticInferenceType.pose_only) return;

        if (holisticInferenceType == HolisticInferenceType.full ||
            holisticInferenceType == HolisticInferenceType.pose_and_face ||
            holisticInferenceType == HolisticInferenceType.face_only)
        {
            FaceRender();
        }

        if (holisticInferenceType == HolisticInferenceType.full ||
            holisticInferenceType == HolisticInferenceType.pose_and_hand)
        {
            HandRender(false);
            HandRender(true);
        }
    }

    void FaceRender()
    {
        int BufferSize = 468;
        Vector4[] vertexData = new Vector4[BufferSize];
        holisticPipeline.faceVertexBuffer.GetData(vertexData);

        // Eyes
        Vector3 RightEyeUp = vertexData[159];
        Vector3 RightEyeDown = vertexData[145];
        Vector3 LeftEyeUp = vertexData[386];
        Vector3 LeftEyeDown = vertexData[374];
        float RightDistance = Vector3.Distance(RightEyeUp, RightEyeDown);
        float LeftDistance = Vector3.Distance(LeftEyeUp, LeftEyeDown);

        //Debug.Log($"Distance: ({RightDistance},{LeftDistance})");

        if (LeftDistance < EyeThreshold || RightDistance < EyeThreshold)
        {
            isCloseEyes = true;
            //Debug.Log($"눈 감았다!!!!!!!! Distance: ({RightDistance},{LeftDistance})");
        }
        else
        {
            isCloseEyes = false;
        }


        // Mouth
        Vector3 MouthUp = vertexData[13];
        Vector3 MouthDown = vertexData[14];
        float MouthDistance = Vector3.Distance(MouthUp, MouthDown);

        if (MouthDistance > TalkThreshold)
        {
            isTalk = true;
            //Debug.Log($"입 벌렸다 우왕o0o? Distance: {MouthDistance}");
        }
        else
        {
            isTalk = false;
        }

        float faceScore = holisticPipeline.faceDetectionScore;
        //Debug.Log($"Face Score: {faceScore}");

        Vector3 MouthLeft = vertexData[61];
        Vector3 MouthRight = vertexData[291];
        float DistanceDiff = Vector3.Distance(LeftEyeDown, RightEyeDown) - Vector3.Distance(MouthLeft, MouthRight);

        if (DistanceDiff < SmileThreshold)
        {
            isSmile = true;
            //Debug.Log($"지금 웃고 있어요!!! Distance: {DistanceDiff}");
        }
        else
        {
            isSmile = false;
        }

    }

    void HandRender(bool isRight)
    {
        // Results
        int BufferSize = 21;
        Vector4[] LeftHandvertexData = new Vector4[BufferSize];
        float LeftHandScore = holisticPipeline.leftHandDetectionScore;
        holisticPipeline.leftHandVertexBuffer.GetData(LeftHandvertexData);

        Vector4[] RightHandvertexData = new Vector4[BufferSize];
        float RightHandScore = holisticPipeline.rightHandDetectionScore;
        holisticPipeline.rightHandVertexBuffer.GetData(RightHandvertexData);

        //if (LeftHandScore < 0.7 || RightHandScore < 0.7)
        //{
        //    Debug.Log("Hands를 찾을 수 없습니다. 손을 보여주세요.");
        //}
        //else
        //{
        //    Debug.Log($"Left Hand: {LeftHandScore}");
        //    Debug.Log($"Right Hand: {RightHandScore}");
        //}
        // Hand를 잘 찾지 못하면 이전 정보 저장하기

        //Debug.Log($"Left Hand: {LeftHandScore}");
        //Debug.Log($"Right Hand: {RightHandScore}");


        // 오른손 업데이트 
        if (RightHandScore > handScoreThreshold)
        {
            RightHandUpdate(RightHandvertexData);
        }
        else
        {
            int ind = 0;
            foreach (Vector4 point in LeftHandvertexData)
            {
                //Debug.Log($"Left: {point}");
                handJoints[ind].Now3D = handJoints[ind].Save3D;
                ind += 1;
            }
        }

        // 왼손 업데이트
        if (LeftHandScore > handScoreThreshold)
        {
            LeftHandUpdate(LeftHandvertexData);
        }
        else
        {
            int ind = 21;
            foreach (Vector4 point in LeftHandvertexData)
            {
                //Debug.Log($"Left: {point}");
                handJoints[ind].Now3D = handJoints[ind].Save3D;
                ind += 1;
            }
        }
        //Debug.Log($"Left Hand: {LeftHandvertexData[0]}");
        //Debug.Log($"Right Hand: {RightHandvertexData[0]}");

        foreach (var point in handJoints)
        {
            KalmanUpdate(point);
        }

        // Avatar 관절 움직이기
        HandRotate();

        // Draw Hands
        if (Drawhand == true)
        {
            if (RightHandScore > handScoreThreshold || LeftHandScore > handScoreThreshold)
            {
                // Draw
                var w = 448;
                var h = 448;
                handMaterial.SetVector("_uiScale", new Vector2(w, h));
                handMaterial.SetVector("_pointColor", isRight ? Color.cyan : Color.yellow);
                handMaterial.SetFloat("_handScoreThreshold", handScoreThreshold);
                // Set inferenced hand landmark results.
                handMaterial.SetBuffer("_vertices", isRight ? holisticPipeline.rightHandVertexBuffer : holisticPipeline.leftHandVertexBuffer);

                // Draw 21 key point circles.
                handMaterial.SetPass(0);
                Graphics.DrawProceduralNow(MeshTopology.Triangles, 96, holisticPipeline.handVertexCount);

                // Draw skeleton lines.
                handMaterial.SetPass(1);
                Graphics.DrawProceduralNow(MeshTopology.Lines, 2, 4 * 5 + 1);
            }
        }
    }

    void OnDestroy()
    {
        // Must call Dispose method when no longer in use.
        holisticPipeline.Dispose();
    }

    void RightHandUpdate(Vector4[] RightvertexData)
    {
        int ind = 0;
        foreach (Vector4 point in RightvertexData)
        {
            //Debug.Log($"Left: {point}");
            handJoints[ind].Now3D = new Vector3(point.x, point.y, point.z);
            handJoints[ind].Save3D = handJoints[ind].Now3D;
            handJoints[ind].Prev3D[0] = handJoints[ind].Now3D;
            for (int i = 1; i < handJoints[ind].Prev3D.Length; i++)
            {
                handJoints[ind].Prev3D[i] = handJoints[ind].Prev3D[i] * 0.5f + handJoints[ind].Prev3D[i - 1] * (1f - 0.5f);
            }
            handJoints[ind].Pos3D = handJoints[ind].Prev3D[handJoints[ind].Prev3D.Length - 1];
            ind += 1;
        }

    }
    void LeftHandUpdate(Vector4[] LeftvertexData)
    {
        int ind = 21;
        foreach (Vector4 point in LeftvertexData)
        {
            //Debug.Log($"Left: {point}");
            handJoints[ind].Now3D = new Vector3(point.x, point.y, point.z);
            handJoints[ind].Save3D = handJoints[ind].Now3D;
            handJoints[ind].Prev3D[0] = handJoints[ind].Now3D;
            for (int i = 1; i < handJoints[ind].Prev3D.Length; i++)
            {
                handJoints[ind].Prev3D[i] = handJoints[ind].Prev3D[i] * 0.5f + handJoints[ind].Prev3D[i - 1] * (1f - 0.5f);
            }
            handJoints[ind].Pos3D = handJoints[ind].Prev3D[handJoints[ind].Prev3D.Length - 1];
            ind += 1;
        }
    }

    public void HandRotate()
    {
        // 오른쪽 엄지손가락
        float angle_Rthumb = CalculateAngle(handJoints[HandIndex.rthumb_cmc.Int()].Pos3D, handJoints[HandIndex.rthumb_mcp.Int()].Pos3D, handJoints[HandIndex.rthumb_ip.Int()].Pos3D);
        handJoints[HandIndex.rthumb_mcp.Int()].Transform.localRotation = handJoints[HandIndex.rthumb_mcp.Int()].InitRotation * Quaternion.Euler(-angle_Rthumb, 0.0f, 0.0f);
        float angle_Rthumb2 = CalculateAngle(handJoints[HandIndex.rthumb_mcp.Int()].Pos3D, handJoints[HandIndex.rthumb_ip.Int()].Pos3D, handJoints[HandIndex.rthumb_tip.Int()].Pos3D);
        handJoints[HandIndex.rthumb_ip.Int()].Transform.localRotation = handJoints[HandIndex.rthumb_ip.Int()].InitRotation * Quaternion.Euler(-angle_Rthumb - angle_Rthumb2, 0.0f, 0.0f);

        // 오른쪽 맨 안쪽 관절
        float angle_Rindex = CalculateAngle(handJoints[HandIndex.rwrist.Int()].Pos3D, handJoints[HandIndex.rindex_mcp.Int()].Pos3D, handJoints[HandIndex.rindex_pip.Int()].Pos3D);
        float angle_Rmiddle = CalculateAngle(handJoints[HandIndex.rwrist.Int()].Pos3D, handJoints[HandIndex.rmiddle_mcp.Int()].Pos3D, handJoints[HandIndex.rmiddle_pip.Int()].Pos3D);
        float angle_Rring = CalculateAngle(handJoints[HandIndex.rwrist.Int()].Pos3D, handJoints[HandIndex.rring_mcp.Int()].Pos3D, handJoints[HandIndex.rring_pip.Int()].Pos3D);
        float angle_Rpinky = CalculateAngle(handJoints[HandIndex.rwrist.Int()].Pos3D, handJoints[HandIndex.rpinky_mcp.Int()].Pos3D, handJoints[HandIndex.rpinky_pip.Int()].Pos3D);

        handJoints[HandIndex.rindex_mcp.Int()].Transform.localRotation = handJoints[HandIndex.rindex_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rindex);
        handJoints[HandIndex.rmiddle_mcp.Int()].Transform.localRotation = handJoints[HandIndex.rmiddle_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rmiddle);
        handJoints[HandIndex.rring_mcp.Int()].Transform.localRotation = handJoints[HandIndex.rring_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rring);
        handJoints[HandIndex.rpinky_mcp.Int()].Transform.localRotation = handJoints[HandIndex.rpinky_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rpinky);
        //Debug.Log($"가운데 엄지손가락 MCP 관절 각도 {angle_Rindex}");

        // 오른쪽 중간 관절
        float angle_Rindex2 = CalculateAngle(handJoints[HandIndex.rindex_mcp.Int()].Pos3D, handJoints[HandIndex.rindex_pip.Int()].Pos3D, handJoints[HandIndex.rindex_dip.Int()].Pos3D);
        float angle_Rmiddle2 = CalculateAngle(handJoints[HandIndex.rmiddle_mcp.Int()].Pos3D, handJoints[HandIndex.rmiddle_pip.Int()].Pos3D, handJoints[HandIndex.rmiddle_dip.Int()].Pos3D);
        float angle_Rring2 = CalculateAngle(handJoints[HandIndex.rring_mcp.Int()].Pos3D, handJoints[HandIndex.rring_pip.Int()].Pos3D, handJoints[HandIndex.rring_dip.Int()].Pos3D);
        float angle_Rpinky2 = CalculateAngle(handJoints[HandIndex.rpinky_mcp.Int()].Pos3D, handJoints[HandIndex.rpinky_pip.Int()].Pos3D, handJoints[HandIndex.rpinky_dip.Int()].Pos3D);

        handJoints[HandIndex.rindex_pip.Int()].Transform.localRotation = handJoints[HandIndex.rindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rindex2);
        handJoints[HandIndex.rmiddle_pip.Int()].Transform.localRotation = handJoints[HandIndex.rindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rmiddle2);
        handJoints[HandIndex.rring_pip.Int()].Transform.localRotation = handJoints[HandIndex.rindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rring2);
        handJoints[HandIndex.rpinky_pip.Int()].Transform.localRotation = handJoints[HandIndex.rindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rpinky2);
        //UnityEngine.Debug.Log($"가운데 엄지손가락 PIP 관절 각도 {angle_index2}");

        //float angle_Rindex3 = CalculateAngle(handJoints[HandIndex.rindex_pip.Int()].Pos3D, handJoints[HandIndex.rindex_dip.Int()].Pos3D, handJoints[HandIndex.rindex_tip.Int()].Pos3D);
        //float angle_Rmiddle3 = CalculateAngle(handJoints[HandIndex.rmiddle_pip.Int()].Pos3D, handJoints[HandIndex.rmiddle_dip.Int()].Pos3D, handJoints[HandIndex.rmiddle_tip.Int()].Pos3D);
        //float angle_Rring3 = CalculateAngle(handJoints[HandIndex.rring_pip.Int()].Pos3D, handJoints[HandIndex.rring_dip.Int()].Pos3D, handJoints[HandIndex.rring_tip.Int()].Pos3D);
        //float angle_Rpinky3 = CalculateAngle(handJoints[HandIndex.rpinky_pip.Int()].Pos3D, handJoints[HandIndex.rpinky_dip.Int()].Pos3D, handJoints[HandIndex.rpinky_tip.Int()].Pos3D);

        //handJoints[HandIndex.rindex_dip.Int()].Transform.rotation = handJoints[HandIndex.rindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rindex - angle_Rindex2 - angle_Rindex3);
        //handJoints[HandIndex.rmiddle_dip.Int()].Transform.rotation = handJoints[HandIndex.rindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rmiddle - angle_Rmiddle2 - angle_Rmiddle3);
        //handJoints[HandIndex.rring_dip.Int()].Transform.rotation = handJoints[HandIndex.rindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rring - angle_Rring2 - angle_Rring3);
        //handJoints[HandIndex.rpinky_dip.Int()].Transform.rotation = handJoints[HandIndex.rindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, -angle_Rpinky - angle_Rpinky2 - angle_Rpinky3);
        ////UnityEngine.Debug.Log($"가운데 엄지손가락 PIP 관절 각도 {angle_index3}");

        // 왼쪽 엄지손가락
        float angle_Lthumb = CalculateAngle(handJoints[HandIndex.lthumb_cmc.Int()].Pos3D, handJoints[HandIndex.lthumb_mcp.Int()].Pos3D, handJoints[HandIndex.lthumb_ip.Int()].Pos3D);
        handJoints[HandIndex.lthumb_mcp.Int()].Transform.localRotation = handJoints[HandIndex.lthumb_mcp.Int()].InitRotation * Quaternion.Euler(-angle_Lthumb, 0.0f, 0.0f);
        float angle_Lthumb2 = CalculateAngle(handJoints[HandIndex.lthumb_mcp.Int()].Pos3D, handJoints[HandIndex.lthumb_ip.Int()].Pos3D, handJoints[HandIndex.lthumb_tip.Int()].Pos3D);
        handJoints[HandIndex.lthumb_ip.Int()].Transform.localRotation = handJoints[HandIndex.lthumb_ip.Int()].InitRotation * Quaternion.Euler(-angle_Lthumb - angle_Lthumb2, 0.0f, 0.0f);
        //UnityEngine.Debug.Log($"엄지손가락 안쪽 관절 각도 {angle_Lthumb}");
        //UnityEngine.Debug.Log($"엄지손가락 안쪽 관절 각도 {angle_Rthumb}");
        //UnityEngine.Debug.Log($"엄지손가락 중간 관절 각도 {angle_Lthumb2}");
        //UnityEngine.Debug.Log($"엄지손가락 중간 관절 각도 {angle_Rthumb2}");

        float angle_Lindex = CalculateAngle(handJoints[HandIndex.lwrist.Int()].Pos3D, handJoints[HandIndex.lindex_mcp.Int()].Pos3D, handJoints[HandIndex.lindex_pip.Int()].Pos3D);
        float angle_Lmiddle = CalculateAngle(handJoints[HandIndex.lwrist.Int()].Pos3D, handJoints[HandIndex.lmiddle_mcp.Int()].Pos3D, handJoints[HandIndex.lmiddle_pip.Int()].Pos3D);
        float angle_Lring = CalculateAngle(handJoints[HandIndex.lwrist.Int()].Pos3D, handJoints[HandIndex.lring_mcp.Int()].Pos3D, handJoints[HandIndex.lring_pip.Int()].Pos3D);
        float angle_Lpinky = CalculateAngle(handJoints[HandIndex.lwrist.Int()].Pos3D, handJoints[HandIndex.lpinky_mcp.Int()].Pos3D, handJoints[HandIndex.lpinky_pip.Int()].Pos3D);

        // 왼쪽 맨 안쪽 관절
        handJoints[HandIndex.lindex_mcp.Int()].Transform.localRotation = handJoints[HandIndex.lindex_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lindex);
        handJoints[HandIndex.lmiddle_mcp.Int()].Transform.localRotation = handJoints[HandIndex.lmiddle_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lmiddle);
        handJoints[HandIndex.lring_mcp.Int()].Transform.localRotation = handJoints[HandIndex.lring_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lring);
        handJoints[HandIndex.lpinky_mcp.Int()].Transform.localRotation = handJoints[HandIndex.lpinky_mcp.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lpinky);
        //UnityEngine.Debug.Log($"가운데 엄지손가락 MCP 관절 각도 {angle_index}");

        // 왼쪽 중간 관절
        float angle_Lindex2 = CalculateAngle(handJoints[HandIndex.lindex_mcp.Int()].Pos3D, handJoints[HandIndex.lindex_pip.Int()].Pos3D, handJoints[HandIndex.lindex_dip.Int()].Pos3D);
        float angle_Lmiddle2 = CalculateAngle(handJoints[HandIndex.lmiddle_mcp.Int()].Pos3D, handJoints[HandIndex.lmiddle_pip.Int()].Pos3D, handJoints[HandIndex.lmiddle_dip.Int()].Pos3D);
        float angle_Lring2 = CalculateAngle(handJoints[HandIndex.lring_mcp.Int()].Pos3D, handJoints[HandIndex.lring_pip.Int()].Pos3D, handJoints[HandIndex.lring_dip.Int()].Pos3D);
        float angle_Lpinky2 = CalculateAngle(handJoints[HandIndex.lpinky_mcp.Int()].Pos3D, handJoints[HandIndex.lpinky_pip.Int()].Pos3D, handJoints[HandIndex.lpinky_dip.Int()].Pos3D);

        handJoints[HandIndex.lindex_pip.Int()].Transform.localRotation = handJoints[HandIndex.lindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lindex2);
        handJoints[HandIndex.lmiddle_pip.Int()].Transform.localRotation = handJoints[HandIndex.lindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lmiddle2);
        handJoints[HandIndex.lring_pip.Int()].Transform.localRotation = handJoints[HandIndex.lindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lring2);
        handJoints[HandIndex.lpinky_pip.Int()].Transform.localRotation = handJoints[HandIndex.lindex_pip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lpinky2);
        ////UnityEngine.Debug.Log($"가운데 엄지손가락 PIP 관절 각도 {angle_index2}");

        //float angle_Lindex3 = CalculateAngle(handJoints[HandIndex.lindex_pip.Int()].Pos3D, handJoints[HandIndex.lindex_dip.Int()].Pos3D, handJoints[HandIndex.lindex_tip.Int()].Pos3D);
        //float angle_Lmiddle3 = CalculateAngle(handJoints[HandIndex.lmiddle_pip.Int()].Pos3D, handJoints[HandIndex.lmiddle_dip.Int()].Pos3D, handJoints[HandIndex.lmiddle_tip.Int()].Pos3D);
        //float angle_Lring3 = CalculateAngle(handJoints[HandIndex.lring_pip.Int()].Pos3D, handJoints[HandIndex.lring_dip.Int()].Pos3D, handJoints[HandIndex.lring_tip.Int()].Pos3D);
        //float angle_Lpinky3 = CalculateAngle(handJoints[HandIndex.lpinky_pip.Int()].Pos3D, handJoints[HandIndex.lpinky_dip.Int()].Pos3D, handJoints[HandIndex.lpinky_tip.Int()].Pos3D);

        //handJoints[HandIndex.lindex_dip.Int()].Transform.rotation = handJoints[HandIndex.lindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lindex + angle_Lindex2 + angle_Lindex3);
        //handJoints[HandIndex.lmiddle_dip.Int()].Transform.rotation = handJoints[HandIndex.lindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lmiddle + angle_Lmiddle2 + angle_Lmiddle3);
        //handJoints[HandIndex.lring_dip.Int()].Transform.rotation = handJoints[HandIndex.lindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lring + angle_Lring2 + angle_Lring3);
        //handJoints[HandIndex.lpinky_dip.Int()].Transform.rotation = handJoints[HandIndex.lindex_dip.Int()].InitRotation * Quaternion.Euler(0.0f, 0.0f, angle_Lpinky + angle_Lpinky2 + angle_Lpinky3);
        ////UnityEngine.Debug.Log($"가운데 엄지손가락 PIP 관절 각도 {angle_index3}");
    }

    public float CalculateAngle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        // 두 벡터를 만듭니다.
        Vector3 vectorAB = pointB - pointA;
        Vector3 vectorBC = pointC - pointB;

        // 각도 계산
        float angle = Vector3.Angle(vectorAB, vectorBC);
        angle = angle < 20f ? 0f : angle;
                
        //if (float.IsNaN(angle))
        //    angle = 0f;
        return angle;
    }



    private void CloseEyes()
    {
        if (isCloseEyes)
        {
            eyeweight += Time.deltaTime * 500f;
        }
        else
        {
            eyeweight -= Time.deltaTime * 500f;
        }
        eyeweight = Mathf.Clamp(eyeweight, 0, 100);
        skinnedMeshRenderer.SetBlendShapeWeight(24, eyeweight);

    }

    private void Smiling()
    {
        if (isSmile)
        {
            smileweight += Time.deltaTime * 100f;
        }
        else
        {
            smileweight -= Time.deltaTime * 100f;
        }
        smileweight = Mathf.Clamp(smileweight, 0, 50);
        skinnedMeshRenderer.SetBlendShapeWeight(3, smileweight);
        skinnedMeshRenderer.SetBlendShapeWeight(4, smileweight);
        skinnedMeshRenderer.SetBlendShapeWeight(5, smileweight);
    }
    private void Talking()
    {
        if (isTalk)
        {
            mouthweight += Time.deltaTime * 500f;
        }
        else
        {
            mouthweight -= Time.deltaTime * 500f;
        }
        mouthweight = Mathf.Clamp(mouthweight, 0, 100);
        skinnedMeshRenderer.SetBlendShapeWeight(31, mouthweight);
    }

    /// Kalman filter
    void KalmanUpdate(HandInformation.HandJoint measurement)
    {
        measurementUpdate(measurement);
        measurement.Pos3D.x = measurement.X.x + (measurement.Now3D.x - measurement.X.x) * measurement.K.x;
        measurement.Pos3D.y = measurement.X.y + (measurement.Now3D.y - measurement.X.y) * measurement.K.y;
        measurement.Pos3D.z = measurement.X.z + (measurement.Now3D.z - measurement.X.z) * measurement.K.z;
        measurement.X = measurement.Pos3D;
    }

    void measurementUpdate(HandInformation.HandJoint measurement)
    {
        measurement.K.x = (measurement.P.x + KalmanParamQ) / (measurement.P.x + KalmanParamQ + KalmanParamR);
        measurement.K.y = (measurement.P.y + KalmanParamQ) / (measurement.P.y + KalmanParamQ + KalmanParamR);
        measurement.K.z = (measurement.P.z + KalmanParamQ) / (measurement.P.z + KalmanParamQ + KalmanParamR);
        measurement.P.x = KalmanParamR * (measurement.P.x + KalmanParamQ) / (KalmanParamR + measurement.P.x + KalmanParamQ);
        measurement.P.y = KalmanParamR * (measurement.P.y + KalmanParamQ) / (KalmanParamR + measurement.P.y + KalmanParamQ);
        measurement.P.z = KalmanParamR * (measurement.P.z + KalmanParamQ) / (KalmanParamR + measurement.P.z + KalmanParamQ);
    }
}
