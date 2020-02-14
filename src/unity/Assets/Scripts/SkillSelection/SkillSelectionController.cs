/*
 * 스킬 선택을 담당합니다.
 * 
 * skills : 씬에 등장하는 모든 스킬아이콘
 * selectedSkillIndex : 플레이어가 선택한 스킬 아이콘의 스킬번호
 * selectedSkills : UI에 등장하는 플레이어가 선택한 스킬 아이콘
 * 
 * 02 13 현재 아이콘 선택시 Red, 그 외 White로 색상 변경.
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionController : MonoBehaviour
{
    // 스킬 이미지
    public List<Sprite> skillIconImages;

    // 선택된 스킬의 인덱스들
    public List<int> selectedSkillIndex;
    
    // 선택된 스킬 게임 오브젝트
    public List<SkillSelected> selectedSkills;

    // 스킬들
    public List<SkillImage> skills;

    
    // 교체 신청한 스킬 번호
    public int changeCandidate;

    // Start is called before the first frame update
    void Start()
    {
        selectedSkillIndex = new List<int>();
 
        changeCandidate = -1;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 선택된 스킬 인덱스에 따라 이미지 변경 
        ChangeSelectedSkillIcon();
    }
    
    // "선택된 스킬"의 이미지를 교체
    public void ChangeSelectedSkillIcon()
    {
        for(int i = 0; i < selectedSkills.Count; i++)
        {
            if(selectedSkills[i].skillIndex != -1)
            {
                selectedSkills[i].GetComponent<Image>().sprite = skillIconImages[selectedSkills[i].skillIndex];
            }
        }
    }

    // skillIndex가 이미 선택된 스킬인지 반환
    public bool IsSkillExist(int skillIndex)
    {
        for (int i = 0; i < selectedSkillIndex.Count; i++)
        {
            if (selectedSkillIndex[i] == skillIndex)
                return true;
        }
        return false;
    }

    // skillIndex가 현재 max까지 차있는지 반환
    public bool IsSkillFull()
    {
        // 이미 전부 선택된 경우도 true반환.
        if (selectedSkillIndex.Count >= 3) 
            return true;
        return false;
    }

    // "선택된 스킬"의 skillIndex의 리스트 내 인덱스를 찾는다.
    public int FindExistSkill(int skillIndex)
    {
        for(int i = 0; i < selectedSkillIndex.Count; i++)
        {
            if(selectedSkillIndex[i] == skillIndex)
            {
                return i;
            }
        }
        return -1;
    }

    // 스킬을 "선택된 스킬" 리스트에 추가
    public void AddSkillIcon(int skillIndex)
    {
        if(selectedSkillIndex.Count < 3)
        {
            selectedSkills[selectedSkillIndex.Count].skillIndex = skillIndex;
            skills[skillIndex].transform.GetComponent<Image>().color = Color.red;

            selectedSkillIndex.Add(skillIndex);
        }
    }

    // 선택되지 않은 스킬과 "선택된 스킬"을 교체하는 함수
    public void SubstituteSkillIcon(int toBeChangedSkillIndex)
    {
        // changeCanditate -> toBeChangedSkillIndex로 교체됨

        int index = FindExistSkill(changeCandidate);

        selectedSkillIndex[index] = toBeChangedSkillIndex;
        selectedSkills[index].skillIndex = toBeChangedSkillIndex;

        skills[toBeChangedSkillIndex].transform.GetComponent<Image>().color = Color.red;
        skills[changeCandidate].transform.GetComponent<Image>().color = Color.white;

        changeCandidate = -1;
    }


    // 이미 선택된 애들중 순서 바꾸고싶다면 교체
    public void SwapSelectedSkillIcon(int toBeChangedSkillIndex)
    {
        int s1 = -1, s2 = -1;

        for(int i = 0; i < selectedSkillIndex.Count; i++)
        {
            if(selectedSkillIndex[i] == toBeChangedSkillIndex)
            {
                s2 = i;
            }
            if(selectedSkillIndex[i] == changeCandidate)
            {
                s1 = i;
            }
        }

        Debug.Log(s1 + "에서 " + s2 + " 로 교체");

        // Swap
        selectedSkills[s2].skillIndex = changeCandidate;
        selectedSkills[s1].skillIndex = toBeChangedSkillIndex;

        // Swap
        int tmp = selectedSkillIndex[s1];
        selectedSkillIndex[s1] = selectedSkillIndex[s2];
        selectedSkillIndex[s2] = tmp;

        changeCandidate = -1;
    }
}
