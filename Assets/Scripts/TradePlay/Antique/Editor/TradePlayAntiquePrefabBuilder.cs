using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayAntiquePrefabBuilder
    {
        const string PrefabFolder = "Assets/Prefab";
        const string PrefabPath = "Assets/Prefab/Antique.prefab";

        static TradePlayAntiquePrefabBuilder()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("TradePlay/创建古玩预制体")]
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
            GameObject root = new GameObject("Antique", typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(280f, 420f);

            Image portrait = CreateImage(root.transform, "Portrait", new Color(0.45f, 0.36f, 0.24f, 1f));
            Place(portrait.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(220f, 240f), new Vector2(0.5f, 1f));
            portrait.raycastTarget = true;

            Text nameText = CreateLabel(root.transform, "NameText", "示例古玩", 22, TextAnchor.MiddleCenter, font);
            Place(nameText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -268f), new Vector2(220f, 32f), new Vector2(0.5f, 1f));

            GameObject priceTag = CreateArea(root.transform, "PriceTag");
            Place(priceTag.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0f, 72f), new Vector2(220f, 64f), new Vector2(0.5f, 0f));
            Image tagBackground = CreateImage(priceTag.transform, "Background", new Color(0.16f, 0.13f, 0.1f, 0.92f));
            Stretch(tagBackground.rectTransform);
            Text priceText = CreateLabel(priceTag.transform, "PriceText", "1000", 24, TextAnchor.MiddleCenter, font);
            Place(priceText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -6f), new Vector2(200f, 28f), new Vector2(0.5f, 1f));
            Text markupText = CreateLabel(priceTag.transform, "MarkupText", "+0%", 16, TextAnchor.MiddleCenter, font);
            Place(markupText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 6f), new Vector2(200f, 22f), new Vector2(0.5f, 0f));
            markupText.color = new Color(1f, 0.82f, 0.4f, 1f);

            GameObject effectArea = CreateArea(root.transform, "EffectArea");
            Place(effectArea.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(80f, 80f), new Vector2(0.5f, 0.5f));

            AntiqueView antiqueView = root.AddComponent<AntiqueView>();
            AntiqueNameHover hover = portrait.gameObject.AddComponent<AntiqueNameHover>();
            hover.Bind(antiqueView);

            SerializedObject so = new SerializedObject(antiqueView);
            so.FindProperty("portraitImage").objectReferenceValue = portrait;
            so.FindProperty("nameText").objectReferenceValue = nameText;
            so.FindProperty("priceText").objectReferenceValue = priceText;
            so.FindProperty("markupText").objectReferenceValue = markupText;
            so.FindProperty("effectAnchor").objectReferenceValue = effectArea.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject hoverSo = new SerializedObject(hover);
            hoverSo.FindProperty("antiqueView").objectReferenceValue = antiqueView;
            hoverSo.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("已创建古玩预制体：" + PrefabPath);
        }

        static GameObject CreateArea(Transform parent, string name)
        {
            GameObject area = new GameObject(name, typeof(RectTransform));
            area.transform.SetParent(parent, false);
            return area;
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
