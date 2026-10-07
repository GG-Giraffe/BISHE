using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TradePlay.Editor
{
    [CustomEditor(typeof(BuffData))]
    public sealed class BuffDataEditor : UnityEditor.Editor
    {
        SerializedProperty _buffId;
        SerializedProperty _buffName;
        SerializedProperty _icon;
        SerializedProperty _description;
        SerializedProperty _duration;
        SerializedProperty _modifiers;
        SerializedProperty _triggers;
        SerializedProperty _effects;
        ReorderableList _effectList;

        void OnEnable()
        {
            _buffId = serializedObject.FindProperty("buffId");
            _buffName = serializedObject.FindProperty("buffName");
            _icon = serializedObject.FindProperty("icon");
            _description = serializedObject.FindProperty("description");
            _duration = serializedObject.FindProperty("duration");
            _modifiers = serializedObject.FindProperty("modifiers");
            _triggers = serializedObject.FindProperty("triggers");
            _effects = serializedObject.FindProperty("effects");

            _effectList = new ReorderableList(serializedObject, _effects, true, true, true, true);
            _effectList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "触发效果（按执行顺序，数值会乘层数）");
            _effectList.elementHeightCallback = index =>
            {
                SerializedProperty element = _effects.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 6f;
            };
            _effectList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty element = _effects.GetArrayElementAtIndex(index);
                rect.y += 2f;
                rect.height -= 4f;
                EditorGUI.PropertyField(rect, element, new GUIContent("效果 " + (index + 1)), true);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("基础信息", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_buffId, new GUIContent("ID"));
            EditorGUILayout.PropertyField(_buffName, new GUIContent("名字"));
            EditorGUILayout.PropertyField(_icon, new GUIContent("图标"));
            EditorGUILayout.PropertyField(_duration, new GUIContent("持续回合"));
            EditorGUILayout.PropertyField(_description, new GUIContent("描述"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("数值加成（每层）", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_modifiers, new GUIContent("加成"), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("触发时机", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_triggers, new GUIContent("时机"), true);
            EditorGUILayout.HelpBox("回合结束时先执行触发效果，再减少持续回合。还剩 1 回合的 Buff 这次仍会触发。", MessageType.None);

            EditorGUILayout.Space();
            _effectList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
