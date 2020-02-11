using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    private void OnEnable()
    {
        progressBar.fillAmount = 1f;
    }

    private void Update()
    {
        progressBar.fillAmount -= Time.deltaTime / 10f;
    }
}
