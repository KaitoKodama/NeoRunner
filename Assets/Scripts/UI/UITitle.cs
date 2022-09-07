using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITitle : MonoBehaviour
{
    [SerializeField] Text tapText = default;
    [SerializeField] Button startBtn = default;


    void Start()
    {
        tapText.DOFade(0.2f, 1f).SetLoops(-1, LoopType.Yoyo);
        startBtn.onClick.AddListener(OnStartButtonDown);
    }

    private void OnStartButtonDown()
    {
        GameManager.instance.RequestSceneTransition(BuildScene.Battle);
    }
}
