using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSkill : MonoBehaviour
{
    [SerializeField] private Text skillNameText;
    [SerializeField] private Image skillImage;

    public void Initialize(bool isFirstCamp, string skillName, Sprite skillSprite)
    {
        ChangeAnchor(isFirstCamp);
        skillNameText.text = skillName;
        skillImage.sprite = skillSprite;
    }

    private void ChangeAnchor(bool isFirstCamp)
    {
        RectTransform rect = GetComponent<RectTransform>();

        if (isFirstCamp)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
        }
        else
        {
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
        }
    }
}
