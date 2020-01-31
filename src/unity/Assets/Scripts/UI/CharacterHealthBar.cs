using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Image bar;

    private Character character;

    private void Start()
    {
        character = transform.parent.GetComponent<Character>();

        if (character.isFriend == false)
        {
            bar.color = new Color(255, 100, 100);
        }
    }

    private void Update()
    {
        healthBar.rotation = Camera.main.transform.rotation;
        bar.fillAmount = (float)character.status.CHP / character.status.MHP;
    }
}
