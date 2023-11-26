using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHCanvasShield : MonoBehaviour
{
    public static KHHCanvasShield Instance;
    Transform loadingImage;

    //float rotateSpeed = 180f;

    void Awake()
    {
        Instance = this;
        loadingImage = transform.GetChild(0);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        loadingImage.DOKill();
    }

    // Update is called once per frame
    void Update()
    {
        loadingImage.DOLocalRotate(new Vector3(0, 0, -180), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
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
