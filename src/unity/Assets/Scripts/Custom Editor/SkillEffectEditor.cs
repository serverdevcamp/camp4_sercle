using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillEffect)), CanEditMultipleObjects]
public class SkillEffectEditor : Editor
{
    public SerializedProperty
        skillType_Prop,
        coefficient_Prop,
        ccType_Prop,
        statusType_Prop,
        hardCCType_Prop,
        amount_Prop,
        duration_Prop;

    private void OnEnable()
    {
        skillType_Prop = serializedObject.FindProperty("skillType");
        coefficient_Prop = serializedObject.FindProperty("coefficient");
        ccType_Prop = serializedObject.FindProperty("ccType");
        statusType_Prop = serializedObject.FindProperty("statusType");
        hardCCType_Prop = serializedObject.FindProperty("hardCCType");
        amount_Prop = serializedObject.FindProperty("amount");
        duration_Prop = serializedObject.FindProperty("duration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(skillType_Prop);

        SkillEffect.SkillType st = (SkillEffect.SkillType)skillType_Prop.enumValueIndex;

        switch (st)
        {
            case SkillEffect.SkillType.Attack:
                EditorGUILayout.PropertyField(coefficient_Prop, new GUIContent("Damage Coefficient"), true);
                break;
            case SkillEffect.SkillType.Heal:
                EditorGUILayout.PropertyField(coefficient_Prop, new GUIContent("Heal Coefficient"));
                break;
            case SkillEffect.SkillType.Temp:
                EditorGUILayout.PropertyField(ccType_Prop, new GUIContent("CC Type"));

                EditorGUI.indentLevel += 1;

                SkillEffect.CCType ct = (SkillEffect.CCType)ccType_Prop.enumValueIndex;

                switch (ct)
                {
                    case SkillEffect.CCType.Soft:
                        EditorGUILayout.PropertyField(statusType_Prop, new GUIContent("Target Status"));
                        EditorGUILayout.PropertyField(amount_Prop, new GUIContent("Change Amount"));
                        EditorGUILayout.PropertyField(duration_Prop, new GUIContent("Duration"));
                        break;
                    case SkillEffect.CCType.Hard:
                        EditorGUILayout.PropertyField(hardCCType_Prop, new GUIContent("Hard CC Type"));
                        EditorGUILayout.PropertyField(duration_Prop, new GUIContent("Duration"));
                        break;
                }

                EditorGUI.indentLevel -= 1;

                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
