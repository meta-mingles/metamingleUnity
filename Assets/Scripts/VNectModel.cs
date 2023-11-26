using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Position index of joint points
/// </summary>
public enum PositionIndex : int
{
    rShldrBend = 0,                           // 0
    rForearmBend,                             // 1
    rHand,                                    // 2
    rThumb2,                                  // 3
    rMid1,                                    // 4

    lShldrBend,                               // 5
    lForearmBend,                             // 6
    lHand,                                    // 7
    lThumb2,                                  // 8
    lMid1,                                    // 9

    lEar,                                     // 10
    lEye,                                     // 11
    rEar,                                     // 12
    rEye,                                     // 13
    Nose,                                     // 14

    rThighBend,                               // 15
    rShin,                                    // 16
    rFoot,                                    // 17
    rToe,                                     // 18

    lThighBend,                               // 19
    lShin,                                    // 20
    lFoot,                                    // 21
    lToe,                                     // 22

    abdomenUpper,                             // 23

    //Calculated coordinates
    hip,
    head,
    neck,
    spine,

    Count,
    None,
}

public static partial class EnumExtend
{
    public static int Int(this PositionIndex i)
    {
        return (int)i;
    }
}

public class VNectModel : MonoBehaviour
{

    public class JointPoint
    {
        // public Vector2 Pos2D = new Vector2();                          // 사용 안함
        // public float score2D;                                          // 사용 안함

        public Vector3 Pos3D = new Vector3();                             // 관절 좌표
        public Vector3 Now3D = new Vector3();                             // 현재 관절 좌표
        public Vector3 Save3D = new Vector3();                            // 구겨지지 않게 하기 위한 이전 값 저장 장
        public Vector3[] PrevPos3D = new Vector3[6];                      // 이전 6개 좌표(Smoothing 계산시 필요)
        public float score3D;                                             // 값 1로 고정

        // Bones
        public Transform Transform = null;                                // Transform 정보 비움
        public Quaternion InitRotation;                                   // 초기 Rotation 값
        public Quaternion Inverse;                                        // Quaternion의 회전 값의 Inverse
        public Quaternion InverseRotation;                                // InitRotation * Inverse

        public JointPoint Child = null;                                   // 관절에 대한 자식 정보
        public JointPoint Parent = null;                                  // 관절에 대한 부모 정보

        // For Kalman filter
        public Vector3 P = new Vector3();                                 // 오차 공분산 행렬(Error Covariance Matrix)
        public Vector3 X = new Vector3();                                 // 상태 추정 벡터(State Estimate Vector)
        public Vector3 K = new Vector3();                                 // 칼만 이득 행렬(Kalman Gain Matrix)
    }

    public class Skeleton
    {
        public GameObject LineObject;                                     // Line object
        public LineRenderer Line;

        public JointPoint start = null;                                   // Line의 Start 지점
        public JointPoint end = null;                                     // Line의 End 지점
    }

    private List<Skeleton> Skeletons = new List<Skeleton>();              // Skeleton에 대한 Line List
    public Material SkeletonMaterial;                                     // Skeleton의 Material

    public bool ShowSkeleton;                                             // Skeleton 나타낼 지 여부
    private bool useSkeleton;                                             // ShowSkeleton
    public float SkeletonX;                                               // Skeleton의 위치 X 
    public float SkeletonY;                                               // Skeleton의 위치 Y
    public float SkeletonZ;                                               // Skeleton의 위치 Z
    public float SkeletonScale;                                           // Skeleton의 크기

    // Joint position and bone
    private JointPoint[] jointPoints;                                     // JointPoint 배열
    public JointPoint[] JointPoints { get { return jointPoints; } }

    private Vector3 initPosition;                                         // Center Position

    // private Quaternion InitGazeRotation;                               // 시선에 대한 Rotation 값
    // private Quaternion gazeInverse;                                    // 시선에 대한 Inverse 값

    // UnityChan
    public GameObject ModelObject;                                        // Avatar
    public GameObject Nose;                                               // Avatar의 코 위치의 GameObject
    private Animator anim;                                                // Avatar의 Animator

    // Move in z direction
    private float centerTall = 224 * 0.75f;                               // Tall 고정
    private float tall = 224 * 0.75f;
    private float prevTall = 224 * 0.75f;
    public float ZScale = 0.8f;

    private void Update()
    {
        if (!KHHCustomCharacter.instance.LoadCustom) return;
        if (jointPoints == null) return;
        PoseUpdate();
    }

    /// <summary>
    /// Initialize joint points
    /// </summary>
    /// <returns></returns>

    public JointPoint[] Init()
    {
        jointPoints = new JointPoint[PositionIndex.Count.Int()];
        for (var i = 0; i < PositionIndex.Count.Int(); i++) jointPoints[i] = new JointPoint();

        anim = ModelObject.GetComponent<Animator>();

        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        jointPoints[PositionIndex.rForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        jointPoints[PositionIndex.rHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        jointPoints[PositionIndex.rThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        jointPoints[PositionIndex.rMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        jointPoints[PositionIndex.lForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        jointPoints[PositionIndex.lHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        jointPoints[PositionIndex.lThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        jointPoints[PositionIndex.lMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);

        // Face
        jointPoints[PositionIndex.lEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.lEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        jointPoints[PositionIndex.rEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.rEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);
        jointPoints[PositionIndex.Nose.Int()].Transform = Nose.transform;

        // Right Leg
        jointPoints[PositionIndex.rThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        //jointPoints[PositionIndex.rShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        //jointPoints[PositionIndex.rFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        //jointPoints[PositionIndex.rToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightToes);

        // Left Leg
        jointPoints[PositionIndex.lThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        //jointPoints[PositionIndex.lShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        //jointPoints[PositionIndex.lFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        //jointPoints[PositionIndex.lToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftToes);

        // etc
        jointPoints[PositionIndex.abdomenUpper.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);
        jointPoints[PositionIndex.hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Hips);
        jointPoints[PositionIndex.head.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.neck.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);
        jointPoints[PositionIndex.spine.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);

        // Child Settings
        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Child = jointPoints[PositionIndex.rForearmBend.Int()];
        jointPoints[PositionIndex.rForearmBend.Int()].Child = jointPoints[PositionIndex.rHand.Int()];
        jointPoints[PositionIndex.rForearmBend.Int()].Parent = jointPoints[PositionIndex.rShldrBend.Int()];

        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Child = jointPoints[PositionIndex.lForearmBend.Int()];
        jointPoints[PositionIndex.lForearmBend.Int()].Child = jointPoints[PositionIndex.lHand.Int()];
        jointPoints[PositionIndex.lForearmBend.Int()].Parent = jointPoints[PositionIndex.lShldrBend.Int()];

        // Right Hands
        //jointPoints[PositionIndex.rHand.Int()].Child = jointPoints[PositionIndex.rMid1.Int()];
        //jointPoints[PositionIndex.rHand.Int()].Child = jointPoints[PositionIndex.rThumb2.Int()];
        //jointPoints[PositionIndex.rMid1.Int()].Parent = jointPoints[PositionIndex.rHand.Int()];

        //// Left Hands
        //jointPoints[PositionIndex.lHand.Int()].Child = jointPoints[PositionIndex.lMid1.Int()];
        //jointPoints[PositionIndex.lHand.Int()].Child = jointPoints[PositionIndex.lThumb2.Int()];
        //jointPoints[PositionIndex.lMid1.Int()].Parent = jointPoints[PositionIndex.lHand.Int()];

        // Fase

        // Right Leg
        //jointPoints[PositionIndex.rThighBend.Int()].Child = jointPoints[PositionIndex.rShin.Int()];
        //jointPoints[PositionIndex.rShin.Int()].Child = jointPoints[PositionIndex.rFoot.Int()];
        //jointPoints[PositionIndex.rFoot.Int()].Child = jointPoints[PositionIndex.rToe.Int()];
        //jointPoints[PositionIndex.rFoot.Int()].Parent = jointPoints[PositionIndex.rShin.Int()];

        // Left Leg
        //jointPoints[PositionIndex.lThighBend.Int()].Child = jointPoints[PositionIndex.lShin.Int()];
        //jointPoints[PositionIndex.lShin.Int()].Child = jointPoints[PositionIndex.lFoot.Int()];
        //jointPoints[PositionIndex.lFoot.Int()].Child = jointPoints[PositionIndex.lToe.Int()];
        //jointPoints[PositionIndex.lFoot.Int()].Parent = jointPoints[PositionIndex.lShin.Int()];

        // etc
        jointPoints[PositionIndex.spine.Int()].Child = jointPoints[PositionIndex.neck.Int()];
        jointPoints[PositionIndex.neck.Int()].Child = jointPoints[PositionIndex.head.Int()];
        //jointPoints[PositionIndex.head.Int()].Child = jointPoints[PositionIndex.Nose.Int()];

        useSkeleton = ShowSkeleton;
        if (useSkeleton)
        {
            // Line Child Settings
            // Right Arm
            AddSkeleton(PositionIndex.rShldrBend, PositionIndex.rForearmBend);
            AddSkeleton(PositionIndex.rForearmBend, PositionIndex.rHand);
            AddSkeleton(PositionIndex.rHand, PositionIndex.rThumb2);
            AddSkeleton(PositionIndex.rHand, PositionIndex.rMid1);

            // Left Arm
            AddSkeleton(PositionIndex.lShldrBend, PositionIndex.lForearmBend);
            AddSkeleton(PositionIndex.lForearmBend, PositionIndex.lHand);
            AddSkeleton(PositionIndex.lHand, PositionIndex.lThumb2);
            AddSkeleton(PositionIndex.lHand, PositionIndex.lMid1);

            // Fase
            AddSkeleton(PositionIndex.lEar, PositionIndex.Nose);
            AddSkeleton(PositionIndex.rEar, PositionIndex.Nose);

            // Right Leg
            //AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShin);
            //AddSkeleton(PositionIndex.rShin, PositionIndex.rFoot);
            //AddSkeleton(PositionIndex.rFoot, PositionIndex.rToe);

            // Left Leg
            //AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShin);
            //AddSkeleton(PositionIndex.lShin, PositionIndex.lFoot);
            //AddSkeleton(PositionIndex.lFoot, PositionIndex.lToe);

            // etc
            AddSkeleton(PositionIndex.spine, PositionIndex.neck);
            AddSkeleton(PositionIndex.neck, PositionIndex.head);
            AddSkeleton(PositionIndex.head, PositionIndex.Nose);
            AddSkeleton(PositionIndex.neck, PositionIndex.rShldrBend);
            AddSkeleton(PositionIndex.neck, PositionIndex.lShldrBend);
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShldrBend);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShldrBend);
            AddSkeleton(PositionIndex.rShldrBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lShldrBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.rThighBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.abdomenUpper);
            AddSkeleton(PositionIndex.lThighBend, PositionIndex.rThighBend);
        }

        // Set Inverse
        // hip(center)과 양쪽 골반의 삼각형에서의 법선 벡터 - 인체 방향을 뜻한다.
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Transform.position, jointPoints[PositionIndex.lThighBend.Int()].Transform.position, jointPoints[PositionIndex.rThighBend.Int()].Transform.position);
        foreach (var jointPoint in jointPoints)
        {
            if (jointPoint.Transform != null)                                                 // Transform 정보가 없는 경우, 초기 Rotation 값을 각 관절의 Rotation 값으로 정한다.       
            {
                jointPoint.InitRotation = jointPoint.Transform.rotation;
            }

            if (jointPoint.Child != null)                                                     // 해당 관절의 Child가 있는 경우 다음과 같이 계산한다.
            {
                jointPoint.Inverse = GetInverse(jointPoint, jointPoint.Child, forward);       // Inverse = 해당 관절과, Child 관절, 인체의 방향(forward)의 Quaternion Rotation 값의 Inverse
                jointPoint.InverseRotation = jointPoint.Inverse * jointPoint.InitRotation;    // InverseRotation = 초기 Rotation * Inverse
            }
        }

        // Center Position hip 정의(forward에 대한 Quaternion 회전값의 Inverse 이용)
        var hip = jointPoints[PositionIndex.hip.Int()];
        initPosition = jointPoints[PositionIndex.hip.Int()].Transform.position;
        hip.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        hip.InverseRotation = hip.Inverse * hip.InitRotation;

        // head 정의(→ 시선에 대한 Quaternion 회전값의 Inverse 이동)
        var head = jointPoints[PositionIndex.head.Int()];
        head.InitRotation = jointPoints[PositionIndex.head.Int()].Transform.rotation;
        var gaze = jointPoints[PositionIndex.Nose.Int()].Transform.position - jointPoints[PositionIndex.head.Int()].Transform.position;
        head.Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));
        head.InverseRotation = head.Inverse * head.InitRotation;

        //// hand 정의(forward 자리에 손바닥 향하는 위치인 lf 대체)
        //var lHand = jointPoints[PositionIndex.lHand.Int()];
        //var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        //lHand.InitRotation = lHand.Transform.rotation;
        //lHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Transform.position - jointPoints[PositionIndex.lMid1.Int()].Transform.position, lf));
        //lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;

        //// hand 정의(forward 자리에 손바닥 향하는 위치인 rf 대체)
        //var rHand = jointPoints[PositionIndex.rHand.Int()];
        //var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        //rHand.InitRotation = jointPoints[PositionIndex.rHand.Int()].Transform.rotation;
        //rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Transform.position - jointPoints[PositionIndex.rMid1.Int()].Transform.position, rf));
        //rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;

        // 3DScore는 모두 1로 통일
        jointPoints[PositionIndex.hip.Int()].score3D = 1f;
        jointPoints[PositionIndex.neck.Int()].score3D = 1f;
        jointPoints[PositionIndex.Nose.Int()].score3D = 1f;
        jointPoints[PositionIndex.head.Int()].score3D = 1f;
        jointPoints[PositionIndex.spine.Int()].score3D = 1f;


        return JointPoints;
    }

    public void PoseUpdate()
    {
        // 사용자의 키(tall) 계산
        var t1 = Vector3.Distance(jointPoints[PositionIndex.head.Int()].Pos3D, jointPoints[PositionIndex.neck.Int()].Pos3D);
        var t2 = Vector3.Distance(jointPoints[PositionIndex.neck.Int()].Pos3D, jointPoints[PositionIndex.spine.Int()].Pos3D);
        var pm = (jointPoints[PositionIndex.rThighBend.Int()].Pos3D + jointPoints[PositionIndex.lThighBend.Int()].Pos3D) / 2f;
        var t3 = Vector3.Distance(jointPoints[PositionIndex.spine.Int()].Pos3D, pm);
        //var t4r = Vector3.Distance(jointPoints[PositionIndex.rThighBend.Int()].Pos3D, jointPoints[PositionIndex.rShin.Int()].Pos3D);
        //var t4l = Vector3.Distance(jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.lShin.Int()].Pos3D);
        //var t4 = (t4r + t4l) / 2f;
        //var t5r = Vector3.Distance(jointPoints[PositionIndex.rShin.Int()].Pos3D, jointPoints[PositionIndex.rFoot.Int()].Pos3D);
        //var t5l = Vector3.Distance(jointPoints[PositionIndex.lShin.Int()].Pos3D, jointPoints[PositionIndex.lFoot.Int()].Pos3D);
        //var t5 = (t5r + t5l) / 2f;
        //var t = t1 + t2 + t3 + t4 + t5;
        var t = t1 + t2 + t3;

        // 얼마나 이동했는지에 대한 척도
        tall = t * 0.7f + prevTall * 0.3f;
        //tall = 224;
        prevTall = tall;

        if (tall == 0)
        {
            tall = centerTall;
        }
        var dz = (centerTall - tall) / centerTall * ZScale;

        // center 관절(hip)의 transform과 rotation 설정
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Pos3D, jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.rThighBend.Int()].Pos3D);
        jointPoints[PositionIndex.hip.Int()].Transform.position = jointPoints[PositionIndex.hip.Int()].Pos3D * 0.005f + new Vector3(initPosition.x, initPosition.y, initPosition.z + dz);
        jointPoints[PositionIndex.hip.Int()].Transform.rotation = Quaternion.LookRotation(forward) * jointPoints[PositionIndex.hip.Int()].InverseRotation;

        // 각 관절의 rotation 계산
        foreach (var jointPoint in jointPoints)
        {
            if (jointPoint.Parent != null)
            {
                var fv = jointPoint.Parent.Pos3D - jointPoint.Pos3D;
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, fv) * jointPoint.InverseRotation;
            }
            else if (jointPoint.Child != null)
            {
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, forward) * jointPoint.InverseRotation;
            }
        }

        // Head Rotation
        var gaze = jointPoints[PositionIndex.Nose.Int()].Pos3D - jointPoints[PositionIndex.head.Int()].Pos3D;
        var f = TriangleNormal(jointPoints[PositionIndex.Nose.Int()].Pos3D, jointPoints[PositionIndex.rEar.Int()].Pos3D, jointPoints[PositionIndex.lEar.Int()].Pos3D);
        var head = jointPoints[PositionIndex.head.Int()];
        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;

        //// Wrist rotation (Test code)
        //var lHand = jointPoints[PositionIndex.lHand.Int()];
        //var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        //lHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Pos3D - jointPoints[PositionIndex.lMid1.Int()].Pos3D, lf) * lHand.InverseRotation;

        //var rHand = jointPoints[PositionIndex.rHand.Int()];
        //var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        ////rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;
        //rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;

        foreach (var sk in Skeletons)
        {
            var s = sk.start;
            var e = sk.end;

            sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * SkeletonScale + SkeletonX, s.Pos3D.y * SkeletonScale + SkeletonY, s.Pos3D.z * SkeletonScale + SkeletonZ));
            sk.Line.SetPosition(1, new Vector3(e.Pos3D.x * SkeletonScale + SkeletonX, e.Pos3D.y * SkeletonScale + SkeletonY, e.Pos3D.z * SkeletonScale + SkeletonZ));
        }
    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    private Quaternion GetInverse(JointPoint p1, JointPoint p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
    }

    /// <summary>
    /// Add skelton from joint points
    /// </summary>
    /// <param name="s">position index</param>
    /// <param name="e">position index</param>
    private void AddSkeleton(PositionIndex s, PositionIndex e)
    {
        var sk = new Skeleton()
        {
            LineObject = new GameObject("Line"),
            start = jointPoints[s.Int()],
            end = jointPoints[e.Int()],
        };

        sk.Line = sk.LineObject.AddComponent<LineRenderer>();
        sk.Line.startWidth = 0.04f;
        sk.Line.endWidth = 0.01f;

        // define the number of vertex
        sk.Line.positionCount = 2;
        sk.Line.material = SkeletonMaterial;

        Skeletons.Add(sk);
    }
}