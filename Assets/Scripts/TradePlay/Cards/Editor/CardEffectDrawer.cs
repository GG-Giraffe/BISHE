using UnityEditor;
using UnityEngine;

namespace TradePlay.Editor
{
    [CustomPropertyDrawer(typeof(CardEffect))]
    public sealed class CardEffectDrawer : PropertyDrawer
    {
        const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LineCount(property) * (EditorGUIUtility.singleLineHeight + Spacing);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty kind = property.FindPropertyRelative("kind");
            SerializedProperty target = property.FindPropertyRelative("target");
            SerializedProperty value = property.FindPropertyRelative("value");
            SerializedProperty duration = property.FindPropertyRelative("duration");
            SerializedProperty buffId = property.FindPropertyRelative("buffId");
            SerializedProperty vfxId = property.FindPropertyRelative("vfxId");
            SerializedProperty note = property.FindPropertyRelative("note");

            Rect line = FirstLine(position);
            EditorGUI.PropertyField(line, kind, new GUIContent("效果类型"));

            CardEffectKind kindValue = (CardEffectKind)kind.enumValueIndex;
            line = NextLine(line);
            EditorGUI.PropertyField(line, target, new GUIContent("作用目标"));

            switch (kindValue)
            {
                case CardEffectKind.伤害:
                    DrawInt(ref line, value, "伤害值");
                    break;
                case CardEffectKind.回复:
                    DrawInt(ref line, value, "回复值");
                    break;
                case CardEffectKind.抽牌:
                    DrawInt(ref line, value, "抽牌数");
                    break;
                case CardEffectKind.弃牌:
                    DrawInt(ref line, value, "弃牌数");
                    break;
                case CardEffectKind.获得护盾:
                    DrawInt(ref line, value, "护盾值");
                    break;
                case CardEffectKind.获得费用:
                    DrawInt(ref line, value, "费用变化");
                    break;
                case CardEffectKind.施加BUFF:
                    line = NextLine(line);
                    EditorGUI.PropertyField(line, buffId, new GUIContent("BUFF ID"));
                    DrawInt(ref line, duration, "持续回合");
                    break;
                case CardEffectKind.播放特效:
                    line = NextLine(line);
                    EditorGUI.PropertyField(line, vfxId, new GUIContent("特效 ID"));
                    break;
            }

            line = NextLine(line);
            EditorGUI.PropertyField(line, note, new GUIContent("策划备注"));

            EditorGUI.EndProperty();
        }

        static void DrawInt(ref Rect line, SerializedProperty property, string title)
        {
            line = NextLine(line);
            EditorGUI.PropertyField(line, property, new GUIContent(title));
        }

        static Rect FirstLine(Rect position)
        {
            return new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        }

        static Rect NextLine(Rect line)
        {
            return new Rect(line.x, line.y + EditorGUIUtility.singleLineHeight + Spacing, line.width, EditorGUIUtility.singleLineHeight);
        }

        static int LineCount(SerializedProperty property)
        {
            CardEffectKind kind = (CardEffectKind)property.FindPropertyRelative("kind").enumValueIndex;
            switch (kind)
            {
                case CardEffectKind.施加BUFF:
                    return 5;
                case CardEffectKind.播放特效:
                    return 4;
                default:
                    return 4;
            }
        }
    }
}
