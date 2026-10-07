using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayCountdownPrefabBuilder
    {
        const string PrefabFolder = "Assets/Prefab";
        const string PrefabPath = "Assets/Prefab/Countdown.prefab";

        static TradePlayCountdownPrefabBuilder()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("TradePlay/创建倒计时预制体")]
        public static void CreatePrefabMenu()
        {
            CreatePrefab(true);
        }

        static void TryCreatePrefab()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || File.Exists(PrefabPath))
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
            GameObject root = new GameObject("Countdown", typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(220f, 120f);

            Image background = CreateImage(root.transform, "Background", new Color(0.12f, 0.1f, 0.08f, 0.92f));
            Stretch(background.rectTransform);

            Text remaining = CreateLabel(root.transform, "RemainingText", "10", 36, TextAnchor.MiddleCenter, font);
            Place(remaining.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -16f), new Vector2(180f, 48f), new Vector2(0.5f, 1f));

            GameObject effectArea = new GameObject("EffectArea", typeof(RectTransform));
            effectArea.transform.SetParent(root.transform, false);
            Place(effectArea.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0f, 28f), new Vector2(180f, 28f), new Vector2(0.5f, 0f));
            Text effect = CreateLabel(effectArea.transform, "EffectText", string.Empty, 16, TextAnchor.MiddleCenter, font);
            Stretch(effect.rectTransform);
            effect.color = new Color(1f, 0.82f, 0.4f, 1f);

            Text fail = CreateLabel(root.transform, "FailText", "交易失败", 22, TextAnchor.MiddleCenter, font);
            Place(fail.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(180f, 36f), new Vector2(0.5f, 0.5f));
            fail.color = new Color(1f, 0.35f, 0.3f, 1f);
            fail.gameObject.SetActive(false);

            CountdownView view = root.AddComponent<CountdownView>();
            SerializedObject so = new SerializedObject(view);
            so.FindProperty("remainingText").objectReferenceValue = remaining;
            so.FindProperty("effectText").objectReferenceValue = effect;
            so.FindProperty("failText").objectReferenceValue = fail;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
