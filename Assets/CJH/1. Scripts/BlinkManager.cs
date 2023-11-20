using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkManager : MonoBehaviour
{
    public LoopType loopType;
    public Button startButton;

    
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
        if (startButton.interactable == true)
        {
            startButton.GetComponent<CanvasGroup>().DOFade(0.1f, 1f).SetLoops(-1, loopType);
        }
    }
}
