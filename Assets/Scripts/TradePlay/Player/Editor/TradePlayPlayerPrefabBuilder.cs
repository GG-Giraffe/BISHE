using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayPlayerPrefabBuilder
    {
        const string PrefabFolder = "Assets/Prefab";
        const string PrefabPath = "Assets/Prefab/Player.prefab";

        static TradePlayPlayerPrefabBuilder()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("TradePlay/创建玩家角色预制体")]
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
            GameObject root = new GameObject("Player", typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 520f);

            Image portrait = CreateImage(root.transform, "Portrait", new Color(0.32f, 0.42f, 0.55f, 1f));
            Place(portrait.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(240f, 280f), new Vector2(0.5f, 1f));

            GameObject pressureArea = CreateArea(root.transform, "PressureArea");
            Place(pressureArea.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0f, -316f), new Vector2(240f, 36f), new Vector2(0.5f, 1f));
            Image pressureBg = CreateImage(pressureArea.transform, "PressureBarBackground", new Color(0.16f, 0.18f, 0.22f, 1f));
            Place(pressureBg.rectTransform, new Vector2(0f, 0.5f), new Vector2(8f, 0f), new Vector2(150f, 16f), new Vector2(0f, 0.5f));
            Image pressureFill = CreateFilledImage(pressureArea.transform, "PressureBarFill", new Color(0.75f, 0.28f, 0.32f, 1f));
            Place(pressureFill.rectTransform, new Vector2(0f, 0.5f), new Vector2(8f, 0f), new Vector2(150f, 16f), new Vector2(0f, 0.5f));
            Text pressureValue = CreateLabel(pressureArea.transform, "PressureValueText", "40  40%", 14, TextAnchor.MiddleRight, font);
            Place(pressureValue.rectTransform, new Vector2(1f, 0.5f), new Vector2(-8f, 0f), new Vector2(72f, 24f), new Vector2(1f, 0.5f));

            GameObject shieldArea = CreateArea(root.transform, "ShieldArea");
            Place(shieldArea.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(-16f, 40f), new Vector2(120f, 56f), new Vector2(1f, 0.5f));
            Image shieldIcon = CreateImage(shieldArea.transform, "ShieldIcon", new Color(0.45f, 0.7f, 0.95f, 1f));
            Place(shieldIcon.rectTransform, new Vector2(0f, 0.5f), new Vector2(4f, 0f), new Vector2(28f, 28f), new Vector2(0f, 0.5f));
            Image shieldBarBg = CreateImage(shieldArea.transform, "ShieldBarBackground", new Color(0.18f, 0.22f, 0.28f, 1f));
            Place(shieldBarBg.rectTransform, new Vector2(0f, 0.5f), new Vector2(36f, 0f), new Vector2(80f, 12f), new Vector2(0f, 0.5f));
            Image shieldBarFill = CreateFilledImage(shieldArea.transform, "ShieldBarFill", new Color(0.4f, 0.75f, 1f, 1f));
            Place(shieldBarFill.rectTransform, new Vector2(0f, 0.5f), new Vector2(36f, 0f), new Vector2(80f, 12f), new Vector2(0f, 0.5f));
            Text shieldValue = CreateLabel(shieldArea.transform, "ShieldValueText", "5", 14, TextAnchor.MiddleCenter, font);
            Place(shieldValue.rectTransform, new Vector2(1f, 0.5f), new Vector2(-4f, 16f), new Vector2(40f, 20f), new Vector2(1f, 0.5f));

            GameObject buffArea = CreateArea(root.transform, "BuffArea");
            Place(buffArea.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0f, 56f), new Vector2(220f, 44f), new Vector2(0.5f, 0f));
            PlayerBuffSlotView[] slots = new PlayerBuffSlotView[3];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = CreateBuffSlot(buffArea.transform, "BuffSlot_" + (i + 1), font, i * 48f);
            }

            GameObject effectArea = CreateArea(root.transform, "EffectArea");
            Place(effectArea.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(80f, 80f), new Vector2(0.5f, 0.5f));

            PlayerView playerView = root.AddComponent<PlayerView>();
            SerializedObject so = new SerializedObject(playerView);
            so.FindProperty("portraitImage").objectReferenceValue = portrait;
            so.FindProperty("pressureArea").objectReferenceValue = pressureArea;
            so.FindProperty("pressureBarBackground").objectReferenceValue = pressureBg;
            so.FindProperty("pressureBarFill").objectReferenceValue = pressureFill;
            so.FindProperty("pressureValueText").objectReferenceValue = pressureValue;
            so.FindProperty("shieldArea").objectReferenceValue = shieldArea;
            so.FindProperty("shieldBarFill").objectReferenceValue = shieldBarFill;
            so.FindProperty("shieldIcon").objectReferenceValue = shieldIcon;
            so.FindProperty("shieldValueText").objectReferenceValue = shieldValue;
            so.FindProperty("effectAnchor").objectReferenceValue = effectArea.transform;

            SerializedProperty slotProp = so.FindProperty("buffSlots");
            slotProp.arraySize = slots.Length;
            for (int i = 0; i < slots.Length; i++)
            {
                slotProp.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("已创建玩家角色预制体：" + PrefabPath);
        }

        static PlayerBuffSlotView CreateBuffSlot(Transform parent, string name, Font font, float x)
        {
            GameObject slotObject = CreateArea(parent, name);
            Place(slotObject.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(x, 0f), new Vector2(40f, 40f), new Vector2(0f, 0.5f));

            Image icon = CreateImage(slotObject.transform, "Icon", new Color(0.4f, 0.45f, 0.55f, 1f));
            Stretch(icon.rectTransform);

            Text stacks = CreateLabel(slotObject.transform, "StackText", "", 12, TextAnchor.LowerRight, font);
            Place(stacks.rectTransform, new Vector2(1f, 0f), new Vector2(-2f, 2f), new Vector2(24f, 18f), new Vector2(1f, 0f));

            PlayerBuffSlotView slot = slotObject.AddComponent<PlayerBuffSlotView>();
            SerializedObject so = new SerializedObject(slot);
            so.FindProperty("iconImage").objectReferenceValue = icon;
            so.FindProperty("stackText").objectReferenceValue = stacks;
            so.ApplyModifiedPropertiesWithoutUndo();
            return slot;
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

        static Image CreateFilledImage(Transform parent, string name, Color color)
        {
            Image image = CreateImage(parent, name, color);
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = 0.4f;
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
