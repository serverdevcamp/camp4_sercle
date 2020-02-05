using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private int max;
    [SerializeField] private int min;
    [SerializeField] private int targetValue;
    private float curValue;

    private float minWidth = 100f;
    private float maxWidth = 320f;
    private float changingTime = 1f;

    [Header("Element Holder")]
    [SerializeField] private RectTransform sliderBar;
    [SerializeField] private Text valueText;

    private void Start()
    {
        curValue = min;
    }

    private void Update()
    {
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        float mappedValue = (curValue - min) / (max - min);
        float targetWidth = minWidth + (maxWidth - minWidth) * mappedValue;
        float originHeight = sliderBar.sizeDelta.y;

        sliderBar.sizeDelta = new Vector2(targetWidth, originHeight);
        valueText.text = curValue.ToString("0");
    }

    public void SetValue(int value)
    {
        targetValue = value;
        DOTween.To(() => curValue, x => curValue = x, targetValue, changingTime);
    }
}
