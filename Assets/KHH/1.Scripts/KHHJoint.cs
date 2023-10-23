using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHJoint : MonoBehaviour
{
    public enum JointType
    {
        LHand,
        RHand,
        LFoot,
        RFoot,
    }

    public JointType jointType;
    public Transform rotTarget;
    public Transform[] lookTargets;

    // Update is called once per frame
    void Update()
    {
        Vector3 lookTarget = Vector3.zero;
        foreach (var lt in lookTargets)
            lookTarget += lt.position;
        lookTarget /= lookTargets.Length;

        //y축방향으로 child를 바라본다
        switch (jointType)
        {
            case JointType.LHand:
                transform.LookAt(lookTarget, Vector3.up);
                transform.Rotate(0, 90, 90);
                break;
            case JointType.RHand:
                transform.LookAt(lookTarget, Vector3.up);
                transform.Rotate(0, -90, -90);
                break;
            case JointType.LFoot:
                transform.LookAt(lookTarget, Vector3.right);
                transform.Rotate(-90, 180, 0);
                break;
            case JointType.RFoot:
                transform.LookAt(lookTarget, Vector3.right);
                transform.Rotate(-90, 180, 0);
                break;
        }

        rotTarget.transform.rotation = Quaternion.Lerp(rotTarget.transform.rotation, transform.rotation, 0.1f);
    }
}
