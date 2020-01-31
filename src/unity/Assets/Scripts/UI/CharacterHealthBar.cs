using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthBar : MonoBehaviour
{
    private void Update()
    {
        GetComponent<RectTransform>().rotation = Camera.main.transform.rotation;
    }
}
