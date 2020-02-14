/*
 * SkillSelection 씬의 스킬 이미지들 관리 스크립트 입니다.
 * 
 * 스킬 이미지 위로 마우스가 있을때 설명문 등을 담당합니다.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillImage : MonoBehaviour
{
    // 스킬 고유 번호
    public int skillIndex;

    // 스킬 이름
    public string skillName;

    // 스킬 설명문
    public string description;

    // 스킬이 선택 되었는지 판단하는 변수


    // 패널
    private GameObject descriptionPanel;

    // 스킬 선택 컨트롤러
    private SkillSelectionController selectionController;

    private void Awake()
    {
        descriptionPanel = GameObject.Find("SkillDescription").gameObject;
        selectionController = GameObject.Find("SkillSelectionController").GetComponent<SkillSelectionController>();

    }
    private void Start()
    {
        if(descriptionPanel != null)
            descriptionPanel.SetActive(false);

        // skillIndex에 맞는 skillIconImages에 있는 스프라이트를 연결
        if (selectionController.skillIconImages[skillIndex] != null) 
            GetComponent<Image>().sprite = selectionController.skillIconImages[skillIndex];
    }

    // 이미지 위에 마우스 포인터가 있을 경우 설명문 창을 띄웁니다.
    public void ShowDescription()
    {
        if(descriptionPanel.activeSelf == false)
            StartCoroutine(CreateDescriptionPanel());
    }
    
    // 스킬 설명문 등장 코루틴
    public IEnumerator CreateDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
        descriptionPanel.transform.GetChild(0).GetComponent<Text>().text = skillName;
        descriptionPanel.transform.GetChild(1).GetComponent<Text>().text = description;

        Vector2 origin = (Vector2)Input.mousePosition;
        descriptionPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y - descriptionPanel.GetComponent<RectTransform>().anchoredPosition.y * 0.25f);

        while (descriptionPanel.activeSelf)
        {
            if(origin != (Vector2)Input.mousePosition)
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

    // 이 이미지가 클릭된 경우, SkillSelectionController 에게 알린다.
    public void ClickSkillIcon()
    {
        Debug.Log(skillName + " click");

        // 이 스킬이 기존에 선택된 스킬이 아니라면, 선택된 스킬에 추가.
        if (!selectionController.IsSkillExist(skillIndex))
        {
            // 지금까지 선택한 스킬의 개수가 3개 미만이라면, 혹은 교체 요청이 없는경우 등록.
            if (!selectionController.IsSkillFull() && selectionController.changeCandidate == -1)
            {
                selectionController.AddSkillIcon(skillIndex);
            }
            // 3개 초과라면
            else
            {
                // 등록된 스킬중 교체 원하는 스킬이 클릭된 경우
                if(selectionController.changeCandidate != -1)
                {
                    // 이 스킬과 교체한다.
                    selectionController.SubstituteSkillIcon(skillIndex);
                }
            }
        }
    }
    

}
