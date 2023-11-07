using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkManager : MonoBehaviour
{
    public LoopType loopType;
    public TMP_Text startText;
    // Start is called before the first frame update
    void Start()
    {
        StartText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartText()
    {
        if (startText.GetComponent<Button>().interactable == true)
        {
            startText.DOFade(0.1f, 3).SetLoops(-1, loopType);
        }

    }

}
