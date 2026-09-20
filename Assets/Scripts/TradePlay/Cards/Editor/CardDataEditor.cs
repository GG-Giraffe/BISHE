using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TradePlay.Editor
{
    [CustomEditor(typeof(CardData))]
    public sealed class CardDataEditor : UnityEditor.Editor
    {
        SerializedProperty _cardId;
        SerializedProperty _cardName;
        SerializedProperty _rarity;
        SerializedProperty _artwork;
        SerializedProperty _character;
        SerializedProperty _cardType;
        SerializedProperty _countdownCost;
        SerializedProperty _description;
        SerializedProperty _level;
        SerializedProperty _damage;
        SerializedProperty _shield;
        SerializedProperty _effects;
        ReorderableList _effectList;

        void OnEnable()
        {
            _cardId = serializedObject.FindProperty("cardId");
            _cardName = serializedObject.FindProperty("cardName");
            _rarity = serializedObject.FindProperty("rarity");
            _artwork = serializedObject.FindProperty("artwork");
            _character = serializedObject.FindProperty("character");
            _cardType = serializedObject.FindProperty("cardType");
            _countdownCost = serializedObject.FindProperty("countdownCost");
            _description = serializedObject.FindProperty("description");
            _level = serializedObject.FindProperty("level");
            _damage = serializedObject.FindProperty("damage");
            _shield = serializedObject.FindProperty("shield");
            _effects = serializedObject.FindProperty("effects");

            _effectList = new ReorderableList(serializedObject, _effects, true, true, true, true);
            _effectList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "卡牌效果（按执行顺序）");
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
            EditorGUILayout.PropertyField(_cardId, new GUIContent("ID"));
            EditorGUILayout.PropertyField(_cardName, new GUIContent("名字"));
            EditorGUILayout.PropertyField(_rarity, new GUIContent("稀有度"));
            EditorGUILayout.PropertyField(_artwork, new GUIContent("卡面"));
            EditorGUILayout.PropertyField(_character, new GUIContent("角色"));
            EditorGUILayout.PropertyField(_cardType, new GUIContent("类型"));
            EditorGUILayout.PropertyField(_countdownCost, new GUIContent("倒计时消耗"));
            EditorGUILayout.PropertyField(_level, new GUIContent("等级"));
            EditorGUILayout.PropertyField(_description, new GUIContent("描述"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("数值预览（由效果自动汇总）", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_damage, new GUIContent("伤害值"));
                EditorGUILayout.PropertyField(_shield, new GUIContent("护盾值"));
            }

            EditorGUILayout.Space();
            _effectList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
