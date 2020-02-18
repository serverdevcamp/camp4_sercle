using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Image bar;

    private Robot character;

    private void Start()
    {
        character = transform.parent.GetComponent<Robot>();

        if (character.CampNum != GameManager.instance.MyCampNum)
        {
            bar.color = new Color(255, 100, 100);
        }
    }

    private void Update()
    {
        healthBar.rotation = Camera.main.transform.rotation;
        bar.fillAmount = (float)character.GetStatus.CHP / character.GetStatus.MHP;
    }
}
