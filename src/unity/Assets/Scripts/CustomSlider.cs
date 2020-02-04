using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    [SerializeField] private int max;
    [SerializeField] private int min;
    [SerializeField] private int value;

    private float minWidth = 100f;
    private float maxWidth = 320f;

    [Header("Element Holder")]
    [SerializeField] private RectTransform sliderBar;
    [SerializeField] private Text valueText;

    private void UpdateSlider()
    {
        float mappedValue = (float)(value - min) / (max - min);
        float targetWidth = minWidth + (maxWidth - minWidth) * mappedValue;
        float originHeight = sliderBar.sizeDelta.y;

        sliderBar.sizeDelta = new Vector2(targetWidth, originHeight);
        valueText.text = value.ToString();
    }

    public void SetValue(int value)
    {
        this.value = value;
        UpdateSlider();
    }
}
