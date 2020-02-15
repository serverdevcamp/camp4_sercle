using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelected : MonoBehaviour
{
    // 스킬 번호
    public int skillIndex;

    // 스킬 선택 컨트롤러
    private SkillSelectionController selectionController;

    // 패널
    private GameObject descriptionPanel;

    private void Awake()
    {
        selectionController = GameObject.Find("SkillSelectionController").GetComponent<SkillSelectionController>();
        descriptionPanel = GameObject.Find("SkillDescription").gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        skillIndex = -1;
    }

    // 선택된 스킬중에서 교체 원하는 경우
    public void SelectedSkillClick()
    {
        // 처음 클릭하는 아이콘이라면
        if(selectionController.changeCandidate == -1)
        {
            selectionController.changeCandidate = skillIndex;
        }
        else
        {
            // 자기 자신을 그대로 클릭했다면
            if(selectionController.changeCandidate == skillIndex)
            {
                selectionController.changeCandidate = -1;
            }
            // 다른 스킬을 클릭했다면 교체
            else
            {
                selectionController.SwapSelectedSkillIcon(skillIndex);
            }
        }
    }

    // 이미지 위에 마우스 포인터가 있을 경우 설명문 창을 띄웁니다.
    public void ShowDescription()
    {
        if (descriptionPanel.activeSelf == false && skillIndex >= 0)
            StartCoroutine(CreateDescriptionPanel());
    }

    // 스킬 설명문 등장 코루틴
    public IEnumerator CreateDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
        descriptionPanel.transform.GetChild(0).GetComponent<Text>().text = selectionController.skill.skillInfo[skillIndex].skillName; // selectionController.skills[skillIndex].skillName;
        descriptionPanel.transform.GetChild(1).GetComponent<Text>().text = selectionController.skill.skillInfo[skillIndex].skillDesc;

        Vector2 origin = (Vector2)Input.mousePosition;
        descriptionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y - descriptionPanel.GetComponent<RectTransform>().anchoredPosition.y * 0.25f);

        while (descriptionPanel.activeSelf)
        {
            if (origin != (Vector2)Input.mousePosition)
            {
                descriptionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y - descriptionPanel.GetComponent<RectTransform>().anchoredPosition.y * 0.25f);
                origin = (Vector2)Input.mousePosition;
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    // 이미지 위에 마우스 포인터가 없을 경우 설명문 제거
    public void CloseDescription()
    {
        descriptionPanel.SetActive(false);
    }
}
