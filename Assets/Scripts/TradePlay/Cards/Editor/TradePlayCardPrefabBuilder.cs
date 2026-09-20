using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayCardPrefabBuilder
    {
        const string PrefabFolder = "Assets/Prefab";
        const string PrefabPath = "Assets/Prefab/Card.prefab";

        static TradePlayCardPrefabBuilder()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("TradePlay/创建卡牌预制体")]
        public static void CreatePrefabMenu()
        {
            CreatePrefab(true);
        }

        static void TryCreatePrefab()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (File.Exists(PrefabPath))
            {
                return;
            }

            CreatePrefab(false);
        }

        static void CreatePrefab(bool overwrite)
        {
            if (!Directory.Exists(PrefabFolder))
            {
                Directory.CreateDirectory(PrefabFolder);
                AssetDatabase.Refresh();
            }

            if (!overwrite && File.Exists(PrefabPath))
            {
                return;
            }

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            GameObject root = new GameObject("Card", typeof(RectTransform));
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(260f, 400f);

            Image background = CreateImage(root.transform, "Background", new Color(0.12f, 0.14f, 0.18f, 1f));
            Stretch(background.rectTransform);

            Image artwork = CreateImage(root.transform, "Artwork", new Color(0.28f, 0.32f, 0.38f, 1f));
            Place(artwork.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -28f), new Vector2(220f, 160f), new Vector2(0.5f, 1f));

            Text nameText = CreateLabel(root.transform, "NameText", "示例卡", 26, TextAnchor.MiddleCenter, font);
            Place(nameText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -200f), new Vector2(220f, 36f), new Vector2(0.5f, 1f));

            Text costText = CreateLabel(root.transform, "CostText", "2", 28, TextAnchor.MiddleCenter, font);
            Place(costText.rectTransform, new Vector2(0f, 1f), new Vector2(18f, -12f), new Vector2(44f, 44f), new Vector2(0f, 1f));

            Text rarityText = CreateLabel(root.transform, "RarityText", "普通", 16, TextAnchor.MiddleRight, font);
            Place(rarityText.rectTransform, new Vector2(1f, 1f), new Vector2(-16f, -12f), new Vector2(90f, 28f), new Vector2(1f, 1f));

            Text typeText = CreateLabel(root.transform, "TypeText", "攻击", 16, TextAnchor.MiddleLeft, font);
            Place(typeText.rectTransform, new Vector2(0f, 1f), new Vector2(18f, -200f), new Vector2(80f, 24f), new Vector2(0f, 1f));

            Text characterText = CreateLabel(root.transform, "CharacterText", "角色", 16, TextAnchor.MiddleRight, font);
            Place(characterText.rectTransform, new Vector2(1f, 1f), new Vector2(-16f, -200f), new Vector2(80f, 24f), new Vector2(1f, 1f));

            Text levelText = CreateLabel(root.transform, "LevelText", "Lv.1", 16, TextAnchor.MiddleCenter, font);
            Place(levelText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -232f), new Vector2(80f, 24f), new Vector2(0.5f, 1f));

            Text damageText = CreateLabel(root.transform, "DamageText", "6", 22, TextAnchor.MiddleCenter, font);
            Place(damageText.rectTransform, new Vector2(0f, 0f), new Vector2(28f, 18f), new Vector2(56f, 32f), new Vector2(0f, 0f));

            Text shieldText = CreateLabel(root.transform, "ShieldText", "3", 22, TextAnchor.MiddleCenter, font);
            Place(shieldText.rectTransform, new Vector2(1f, 0f), new Vector2(-28f, 18f), new Vector2(56f, 32f), new Vector2(1f, 0f));

            Text descriptionText = CreateLabel(root.transform, "DescriptionText", "造成伤害并获得护盾。", 16, TextAnchor.UpperLeft, font);
            Place(descriptionText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 58f), new Vector2(220f, 72f), new Vector2(0.5f, 0f));

            Text idText = CreateLabel(root.transform, "IdText", "card_001", 12, TextAnchor.MiddleLeft, font);
            Place(idText.rectTransform, new Vector2(0f, 0f), new Vector2(12f, 4f), new Vector2(120f, 18f), new Vector2(0f, 0f));

            Text buffText = CreateLabel(root.transform, "BuffText", "BUFF", 12, TextAnchor.MiddleLeft, font);
            Place(buffText.rectTransform, new Vector2(0f, 0f), new Vector2(18f, 50f), new Vector2(110f, 18f), new Vector2(0f, 0f));

            Text effectText = CreateLabel(root.transform, "EffectText", "特效", 12, TextAnchor.MiddleRight, font);
            Place(effectText.rectTransform, new Vector2(1f, 0f), new Vector2(-18f, 50f), new Vector2(110f, 18f), new Vector2(1f, 0f));

            CardView cardView = root.AddComponent<CardView>();
            SerializedObject so = new SerializedObject(cardView);
            so.FindProperty("artworkImage").objectReferenceValue = artwork;
            so.FindProperty("idText").objectReferenceValue = idText;
            so.FindProperty("rarityText").objectReferenceValue = rarityText;
            so.FindProperty("characterText").objectReferenceValue = characterText;
            so.FindProperty("typeText").objectReferenceValue = typeText;
            so.FindProperty("nameText").objectReferenceValue = nameText;
            so.FindProperty("costText").objectReferenceValue = costText;
            so.FindProperty("descriptionText").objectReferenceValue = descriptionText;
            so.FindProperty("levelText").objectReferenceValue = levelText;
            so.FindProperty("damageText").objectReferenceValue = damageText;
            so.FindProperty("shieldText").objectReferenceValue = shieldText;
            so.FindProperty("buffText").objectReferenceValue = buffText;
            so.FindProperty("effectText").objectReferenceValue = effectText;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("已创建卡牌预制体：" + PrefabPath);
        }

        static Image CreateImage(Transform parent, string name, Color color)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        static Text CreateLabel(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Font font)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        static void Place(RectTransform rect, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, Vector2 pivot)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
