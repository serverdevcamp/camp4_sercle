using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image stateImage;

    private Robot robot;

    private void Start()
    {
        robot = transform.parent.GetComponent<Robot>();

        if (robot.CampNum != GameManager.instance.MyCampNum)
        {
            bar.color = new Color(255, 100, 100);
        }
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        bar.fillAmount = (float)robot.GetStatus.CHP / robot.GetStatus.MHP;

        ShowState();
    }

    private void ShowState()
    {
        if (robot.GetState == Robot.State.Idle) stateImage.color = Color.clear;
        else if (robot.GetState == Robot.State.Move) stateImage.color = Color.white;
        else if (robot.GetState == Robot.State.Attack) stateImage.color = Color.red;
    }
}
