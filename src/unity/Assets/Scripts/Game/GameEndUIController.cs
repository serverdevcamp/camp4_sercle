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
        gadget.anchoredPosition += new Vector2(2000, 0);
        leftKnob.anchoredPosition += new Vector2(-1500, 0);
        rightKnob.anchoredPosition += new Vector2(1500, 0);

        Sequence endGame = DOTween.Sequence();
        endGame.Append(panel.DOMove(panelPos, 1f).SetEase(Ease.OutQuart));
        endGame.Append(gadget.DOMove(gadgetPos, 0.6f).SetEase(Ease.InExpo));
        endGame.Insert(1, leftKnob.DOMove(leftKnobPos, 0.6f).SetEase(Ease.InExpo));
        endGame.Insert(1, rightKnob.DOMove(rightKnobPos, 0.6f).SetEase(Ease.InExpo));
        endGame.AppendCallback(() => TextChange(win));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) Initialize(true);
    }

    private void TextChange(bool win)
    {
        if (win) endText.DOText("V I C T O R Y", 1f).SetEase(Ease.Linear);
        else endText.DOText("D E F E A T", 1f).SetEase(Ease.Linear);
    }
}
