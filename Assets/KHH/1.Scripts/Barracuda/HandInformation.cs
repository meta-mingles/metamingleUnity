using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

// MediaPipe에서 정의되는 관절 정의
public enum HandIndex : int
{
    rwrist,                  // 0
    rthumb_cmc,              // 1
    rthumb_mcp,              // 2
    rthumb_ip,               // 3
    rthumb_tip,              // 4
    rindex_mcp,              // 5
    rindex_pip,              // 6
    rindex_dip,              // 7
    rindex_tip,              // 8
    rmiddle_mcp,             // 9
    rmiddle_pip,             // 10
    rmiddle_dip,             // 11
    rmiddle_tip,             // 12
    rring_mcp,               // 13
    rring_pip,               // 14
    rring_dip,               // 15
    rring_tip,               // 16
    rpinky_mcp,              // 17
    rpinky_pip,              // 18
    rpinky_dip,              // 19
    rpinky_tip,              // 20

    lwrist,                  // 21
    lthumb_cmc,              // 22
    lthumb_mcp,              // 23
    lthumb_ip,               // 24
    lthumb_tip,              // 25
    lindex_mcp,              // 26
    lindex_pip,              // 27
    lindex_dip,              // 28
    lindex_tip,              // 29
    lmiddle_mcp,             // 30
    lmiddle_pip,             // 31
    lmiddle_dip,             // 32
    lmiddle_tip,             // 33
    lring_mcp,               // 34
    lring_pip,               // 35
    lring_dip,               // 36
    lring_tip,               // 37
    lpinky_mcp,              // 38
    lpinky_pip,              // 39
    lpinky_dip,              // 40
    lpinky_tip,              // 41

    Count,
}

public static partial class EnumExtend
{
    public static int Int(this HandIndex i)
    {
        return (int)i;
    }
}

public class HandInformation : MonoBehaviour
{
    public class HandJoint
    {
        public Vector3 Pos3D = new Vector3();
        public Vector3 Now3D = new Vector3();
        public Vector3 Save3D = new Vector3();
        public Vector3[] Prev3D = new Vector3[10];

        public Transform Transform = null;
        public Quaternion InitRotation;
        public Quaternion Inverse;
        public Quaternion InverseRotation;

        public HandJoint Child = null;
        public HandJoint Parent = null;

        // For Kalman filter
        public Vector3 P = new Vector3();                                 // 오차 공분산 행렬(Error Covariance Matrix)
        public Vector3 X = new Vector3();                                 // 상태 추정 벡터(State Estimate Vector)
        public Vector3 K = new Vector3();                                 // 칼만 이득 행렬(Kalman Gain Matrix)
    }

    public GameObject ModelObject;
    private HandJoint[] handJoints;

    public HandJoint[] HandJoints { get { return handJoints; } }
    private Animator anim;

    public HandJoint[] Init()
    {
        handJoints = new HandJoint[HandIndex.Count.Int()];
        for (var i = 0; i < HandIndex.Count.Int(); i++) handJoints[i] = new HandJoint();

        anim = ModelObject.GetComponent<Animator>();

        // 관절 매칭(Right)
        handJoints[HandIndex.rwrist.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);

        handJoints[HandIndex.rthumb_cmc.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);
        handJoints[HandIndex.rthumb_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        handJoints[HandIndex.rthumb_ip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbDistal);

        handJoints[HandIndex.rindex_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        handJoints[HandIndex.rindex_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
        handJoints[HandIndex.rindex_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightIndexDistal);

        handJoints[HandIndex.rmiddle_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        handJoints[HandIndex.rmiddle_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
        handJoints[HandIndex.rmiddle_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal);

        handJoints[HandIndex.rring_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightRingProximal);
        handJoints[HandIndex.rring_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
        handJoints[HandIndex.rring_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightRingDistal);

        handJoints[HandIndex.rpinky_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
        handJoints[HandIndex.rpinky_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
        handJoints[HandIndex.rpinky_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLittleDistal);

        // 관절 매칭(Left)
        handJoints[HandIndex.lwrist.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);

        handJoints[HandIndex.lthumb_cmc.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
        handJoints[HandIndex.lthumb_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        handJoints[HandIndex.lthumb_ip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal);

        handJoints[HandIndex.lindex_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
        handJoints[HandIndex.lindex_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
        handJoints[HandIndex.lindex_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal);

        handJoints[HandIndex.lmiddle_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        handJoints[HandIndex.lmiddle_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
        handJoints[HandIndex.lmiddle_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);

        handJoints[HandIndex.lring_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftRingProximal);
        handJoints[HandIndex.lring_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
        handJoints[HandIndex.lring_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftRingDistal);

        handJoints[HandIndex.lpinky_mcp.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
        handJoints[HandIndex.lpinky_pip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
        handJoints[HandIndex.lpinky_dip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal);

        // Parent, Child 관계
        handJoints[HandIndex.rthumb_cmc.Int()].Child = handJoints[HandIndex.rthumb_mcp.Int()];
        handJoints[HandIndex.rthumb_mcp.Int()].Child = handJoints[HandIndex.rthumb_ip.Int()];
        handJoints[HandIndex.rthumb_mcp.Int()].Parent = handJoints[HandIndex.rthumb_cmc.Int()];

        handJoints[HandIndex.lthumb_cmc.Int()].Child = handJoints[HandIndex.lthumb_mcp.Int()];
        handJoints[HandIndex.lthumb_mcp.Int()].Child = handJoints[HandIndex.lthumb_ip.Int()];
        handJoints[HandIndex.lthumb_mcp.Int()].Parent = handJoints[HandIndex.lthumb_cmc.Int()];

        handJoints[HandIndex.rindex_mcp.Int()].Child = handJoints[HandIndex.rindex_pip.Int()];
        handJoints[HandIndex.rindex_pip.Int()].Child = handJoints[HandIndex.rindex_dip.Int()];
        handJoints[HandIndex.rindex_mcp.Int()].Parent = handJoints[HandIndex.rindex_mcp.Int()];

        handJoints[HandIndex.lindex_mcp.Int()].Child = handJoints[HandIndex.lindex_pip.Int()];
        handJoints[HandIndex.lindex_pip.Int()].Child = handJoints[HandIndex.lindex_dip.Int()];
        handJoints[HandIndex.lindex_mcp.Int()].Parent = handJoints[HandIndex.lindex_mcp.Int()];

        handJoints[HandIndex.rmiddle_mcp.Int()].Child = handJoints[HandIndex.rmiddle_pip.Int()];
        handJoints[HandIndex.rmiddle_pip.Int()].Child = handJoints[HandIndex.rmiddle_dip.Int()];
        handJoints[HandIndex.rmiddle_mcp.Int()].Parent = handJoints[HandIndex.rmiddle_mcp.Int()];

        handJoints[HandIndex.lmiddle_mcp.Int()].Child = handJoints[HandIndex.lmiddle_pip.Int()];
        handJoints[HandIndex.lmiddle_pip.Int()].Child = handJoints[HandIndex.lmiddle_dip.Int()];
        handJoints[HandIndex.lmiddle_mcp.Int()].Parent = handJoints[HandIndex.lmiddle_mcp.Int()];

        handJoints[HandIndex.rring_mcp.Int()].Child = handJoints[HandIndex.rring_pip.Int()];
        handJoints[HandIndex.rring_pip.Int()].Child = handJoints[HandIndex.rring_dip.Int()];
        handJoints[HandIndex.rring_mcp.Int()].Parent = handJoints[HandIndex.rring_mcp.Int()];

        handJoints[HandIndex.lring_mcp.Int()].Child = handJoints[HandIndex.lring_pip.Int()];
        handJoints[HandIndex.lring_pip.Int()].Child = handJoints[HandIndex.lring_dip.Int()];
        handJoints[HandIndex.lring_mcp.Int()].Parent = handJoints[HandIndex.lring_mcp.Int()];

        handJoints[HandIndex.rpinky_mcp.Int()].Child = handJoints[HandIndex.rpinky_pip.Int()];
        handJoints[HandIndex.rpinky_pip.Int()].Child = handJoints[HandIndex.rpinky_dip.Int()];
        handJoints[HandIndex.rpinky_mcp.Int()].Parent = handJoints[HandIndex.rpinky_mcp.Int()];

        handJoints[HandIndex.lpinky_mcp.Int()].Child = handJoints[HandIndex.lpinky_pip.Int()];
        handJoints[HandIndex.lpinky_pip.Int()].Child = handJoints[HandIndex.lpinky_dip.Int()];
        handJoints[HandIndex.lpinky_mcp.Int()].Parent = handJoints[HandIndex.lpinky_mcp.Int()];

        // Inverse 구하기
        foreach (var handJoint in handJoints)
        {
            if(handJoint.Transform != null)
            {
                handJoint.InitRotation = handJoint.Transform.localRotation;
            }
            
            if(handJoint.Child != null)
            {
                handJoint.Inverse = Quaternion.Inverse(Quaternion.LookRotation(handJoint.Transform.position - handJoint.Child.Transform.position,Vector3.up));
                handJoint.InverseRotation = handJoint.InitRotation * handJoint.Inverse;
            }
        }

        return HandJoints;
    }
}