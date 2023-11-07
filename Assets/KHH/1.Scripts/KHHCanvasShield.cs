using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHCanvasShield : MonoBehaviour
{
    public static KHHCanvasShield Instance;
    Transform loadingImage;

    float rotateAngle = 45f;
    float rotateTime = 0f;
    float rotateDelay = 0.1f;

    void Awake()
    {
        Instance = this;
        loadingImage = transform.GetChild(0);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        rotateTime += Time.deltaTime;
        if (rotateTime > rotateDelay)
        {
            rotateTime = 0f;
            loadingImage.Rotate(0f, 0f, -rotateAngle);
        }
    }

    public void Show()
    {
        if (gameObject.activeSelf)
            return;
        gameObject.SetActive(true);
        loadingImage.rotation = Quaternion.identity;
    }

    public void Close()
    {
        if (!gameObject.activeSelf)
            return;
        gameObject.SetActive(false);
    }
}
