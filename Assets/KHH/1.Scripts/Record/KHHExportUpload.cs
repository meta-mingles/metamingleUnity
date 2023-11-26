using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KHHExportUpload : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [Header("Check")]
    [SerializeField] CanvasGroup check;
    [SerializeField] Button okButton;
    [SerializeField] Button cancelButton;

    [Header("Uploading")]
    [SerializeField] CanvasGroup uploading;
    [SerializeField] Image uploadingImage;

    [Header("Complete")]
    [SerializeField] CanvasGroup complete;
    [SerializeField] Button closeButton;

    UnityAction uploadAction;

    private void Awake()
    {
        Init();

        if (okButton != null) okButton.onClick.AddListener(Upload);
        if (cancelButton != null) cancelButton.onClick.AddListener(Close);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    void Init()
    {
        canvasGroup.transform.localPosition = new Vector3(0, -100, 0);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        check.gameObject.SetActive(true);
        uploading.gameObject.SetActive(false);
        complete.gameObject.SetActive(false);

        check.alpha = 1;
        uploading.alpha = 0;
        complete.alpha = 0;
    }

    public void Open(UnityAction action)
    {
        Init();
        uploadAction = action;

        gameObject.SetActive(true);
        canvasGroup.transform.DOKill();
        canvasGroup.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.Linear);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, 0.5f).SetEase(Ease.Linear);
        canvasGroup.blocksRaycasts = true;
    }

    void Close()
    {
        uploadAction = null;

        canvasGroup.transform.DOKill();
        canvasGroup.transform.DOLocalMoveY(-100, 0.5f).SetEase(Ease.Linear);
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => { gameObject.SetActive(false); });
        canvasGroup.blocksRaycasts = false;
    }

    void Upload()
    {
        uploadAction?.Invoke();

        uploadingImage.transform.DOKill();
        uploadingImage.transform.DOLocalRotate(new Vector3(0, 0, -180), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

        check.DOKill();
        check.DOFade(0, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            check.gameObject.SetActive(false);
            uploading.gameObject.SetActive(true);
            uploading.DOKill();
            uploading.DOFade(1, 0.2f).SetEase(Ease.Linear);
        });
    }

    public void Complete()
    {
        uploadingImage.transform.DOKill();

        uploading.DOKill();
        uploading.DOFade(0, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            uploading.gameObject.SetActive(false);
            complete.gameObject.SetActive(true);
            complete.DOKill();
            complete.DOFade(1, 0.2f).SetEase(Ease.Linear);
        });
    }
}
