using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Text state;

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

        state.text = State();
    }

    private string State()
    {
        string str = "";

        str += "name : " + transform.parent.name + System.Environment.NewLine;
        str += "state : " + robot.GetState.ToString() + System.Environment.NewLine;
        str += "CHP : " + robot.GetStatus.CHP;

        return str;
    }
}
