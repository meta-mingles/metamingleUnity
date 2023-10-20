using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shape : MonoBehaviour
{
    [SerializeField] private Transform innerShape, outerShape;
    [SerializeField] private float cycleLength = 2;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(new Vector3(10, 0, 0), cycleLength).SetEase(Ease.InBounce);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
