using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHBodyIK : MonoBehaviour
{
    public Transform LS;
    public Transform RS;
    public Transform LP;
    public Transform RP;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        transform.position = (LS.position + RS.position + LP.position + RP.position) / 4;
    }
}
