using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameEndUIController : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Text endText;
    [SerializeField] private RectTransform gadget;
    [SerializeField] private RectTransform leftKnob;
    [SerializeField] private RectTransform rightKnob;

    public void Initialize(bool win)
    {
        Vector2 panelPos = panel.position;
        Vector2 gadgetPos = gadget.position;
        Vector2 leftKnobPos = leftKnob.position;
        Vector2 rightKnobPos = rightKnob.position;

        endText.text = "";
        panel.anchoredPosition += new Vector2(0, -1000);
        gadget.anchoredPosition += new Vector2(1000, 0);
        leftKnob.anchoredPosition += new Vector2(-1000, 0);
        rightKnob.anchoredPosition += new Vector2(1000, 0);

        Sequence endGame = DOTween.Sequence();
        endGame.Append(panel.DOMove(panelPos, 1f).SetEase(Ease.OutQuart));
        endGame.Append(gadget.DOMove(gadgetPos, 1f).SetEase(Ease.InOutElastic));
        endGame.Insert(1, leftKnob.DOMove(leftKnobPos, 1f).SetEase(Ease.InElastic));
        endGame.Insert(1, rightKnob.DOMove(rightKnobPos, 1f).SetEase(Ease.InElastic));
        endGame.AppendCallback(() => TextChange(win));
    }

    private void TextChange(bool win)
    {
        if (win) endText.DOText("V I C T O R Y", 3f).SetEase(Ease.Linear);
        else endText.DOText("D E F E A T", 3f).SetEase(Ease.Linear);
    }
}
