using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFadeIO : MonoBehaviour
{
    [SerializeField] Image fadeImage = default;
    private float duration = 0.5f;

    public float Duration => duration;
    public void OnFadeBegin(float alpha)
    {
        fadeImage.DOFade(alpha, duration);
    }
}
