using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayGuestPrefabBuilder
    {
        const string PrefabFolder = "Assets/Prefab";
        const string PrefabPath = "Assets/Prefab/Guest.prefab";

        static TradePlayGuestPrefabBuilder()
        {
            EditorApplication.delayCall += TryCreatePrefab;
        }

        [MenuItem("TradePlay/创建客人预制体")]
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
            GameObject root = new GameObject("Guest", typeof(RectTransform));
            root.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 520f);

            Image portrait = CreateImage(root.transform, "Portrait", new Color(0.35f, 0.38f, 0.45f, 1f));
            Place(portrait.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(240f, 280f), new Vector2(0.5f, 1f));
            portrait.raycastTarget = true;

            Text nameText = CreateLabel(root.transform, "NameText", "示例客人", 24, TextAnchor.MiddleCenter, font);
            Place(nameText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -312f), new Vector2(240f, 32f), new Vector2(0.5f, 1f));

            GameObject intentArea = CreateArea(root.transform, "IntentArea");
            Place(intentArea.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(16f, -16f), new Vector2(96f, 72f), new Vector2(0f, 1f));
            Image intentIcon = CreateImage(intentArea.transform, "IntentIcon", new Color(0.85f, 0.35f, 0.3f, 1f));
            Place(intentIcon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -4f), new Vector2(40f, 40f), new Vector2(0.5f, 1f));
            Text intentValue = CreateLabel(intentArea.transform, "IntentValueText", "8", 20, TextAnchor.MiddleCenter, font);
            Place(intentValue.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 4f), new Vector2(72f, 24f), new Vector2(0.5f, 0f));
            GameObject intentPopup = CreateArea(intentArea.transform, "IntentPopup");
            Place(intentPopup.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0f, -8f), new Vector2(120f, 48f), new Vector2(0.5f, 1f));
            Image popupBg = CreateImage(intentPopup.transform, "Background", new Color(0.08f, 0.08f, 0.1f, 0.9f));
            Stretch(popupBg.rectTransform);
            Text popupText = CreateLabel(intentPopup.transform, "PopupText", "即将攻击", 14, TextAnchor.MiddleCenter, font);
            Stretch(popupText.rectTransform);
            intentPopup.SetActive(false);

            GameObject interestArea = CreateArea(root.transform, "InterestArea");
            Place(interestArea.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(16f, -20f), new Vector2(200f, 72f), new Vector2(0f, 0.5f));
            Image interestBarBg = CreateImage(interestArea.transform, "InterestBarBackground", new Color(0.18f, 0.18f, 0.2f, 1f));
            Place(interestBarBg.rectTransform, new Vector2(0f, 1f), new Vector2(36f, -8f), new Vector2(160f, 16f), new Vector2(0f, 1f));
            Image interestBarFill = CreateFilledImage(interestArea.transform, "InterestBarFill", new Color(0.95f, 0.55f, 0.2f, 1f));
            Place(interestBarFill.rectTransform, new Vector2(0f, 1f), new Vector2(36f, -8f), new Vector2(160f, 16f), new Vector2(0f, 1f));
            Image heartBg = CreateImage(interestArea.transform, "InterestHeartBackground", new Color(0.25f, 0.16f, 0.18f, 1f));
            Place(heartBg.rectTransform, new Vector2(0f, 1f), new Vector2(4f, -4f), new Vector2(28f, 28f), new Vector2(0f, 1f));
            Image heartFill = CreateFilledImage(interestArea.transform, "InterestHeartFill", new Color(0.86f, 0.22f, 0.28f, 1f));
            Place(heartFill.rectTransform, new Vector2(0f, 1f), new Vector2(4f, -4f), new Vector2(28f, 28f), new Vector2(0f, 1f));
            Text interestValue = CreateLabel(interestArea.transform, "InterestValueText", "30/50", 14, TextAnchor.MiddleLeft, font);
            Place(interestValue.rectTransform, new Vector2(0f, 0f), new Vector2(36f, 24f), new Vector2(120f, 20f), new Vector2(0f, 0f));
            Text interestStage = CreateLabel(interestArea.transform, "InterestStageText", "1/3", 14, TextAnchor.MiddleLeft, font);
            Place(interestStage.rectTransform, new Vector2(0f, 0f), new Vector2(36f, 4f), new Vector2(120f, 20f), new Vector2(0f, 0f));

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
            GuestBuffSlotView[] slots = new GuestBuffSlotView[3];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = CreateBuffSlot(buffArea.transform, "BuffSlot_" + (i + 1), font, i * 48f);
            }

            GameObject effectArea = CreateArea(root.transform, "EffectArea");
            Place(effectArea.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(80f, 80f), new Vector2(0.5f, 0.5f));

            GuestView guestView = root.AddComponent<GuestView>();
            GuestNameHover hover = portrait.gameObject.AddComponent<GuestNameHover>();
            hover.Bind(guestView);

            SerializedObject so = new SerializedObject(guestView);
            so.FindProperty("portraitImage").objectReferenceValue = portrait;
            so.FindProperty("nameText").objectReferenceValue = nameText;
            so.FindProperty("intentArea").objectReferenceValue = intentArea;
            so.FindProperty("intentIconImage").objectReferenceValue = intentIcon;
            so.FindProperty("intentValueText").objectReferenceValue = intentValue;
            so.FindProperty("intentPopup").objectReferenceValue = intentPopup;
            so.FindProperty("interestArea").objectReferenceValue = interestArea;
            so.FindProperty("interestBarBackground").objectReferenceValue = interestBarBg;
            so.FindProperty("interestBarFill").objectReferenceValue = interestBarFill;
            so.FindProperty("interestHeartBackground").objectReferenceValue = heartBg;
            so.FindProperty("interestHeartFill").objectReferenceValue = heartFill;
            so.FindProperty("interestValueText").objectReferenceValue = interestValue;
            so.FindProperty("interestStageText").objectReferenceValue = interestStage;
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

            SerializedObject hoverSo = new SerializedObject(hover);
            hoverSo.FindProperty("guestView").objectReferenceValue = guestView;
            hoverSo.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Object prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log("已创建客人预制体：" + PrefabPath);
        }

        static GuestBuffSlotView CreateBuffSlot(Transform parent, string name, Font font, float x)
        {
            GameObject slotObject = CreateArea(parent, name);
            Place(slotObject.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(x, 0f), new Vector2(40f, 40f), new Vector2(0f, 0.5f));

            Image icon = CreateImage(slotObject.transform, "Icon", new Color(0.4f, 0.45f, 0.55f, 1f));
            Stretch(icon.rectTransform);

            Text stacks = CreateLabel(slotObject.transform, "StackText", "", 12, TextAnchor.LowerRight, font);
            Place(stacks.rectTransform, new Vector2(1f, 0f), new Vector2(-2f, 2f), new Vector2(24f, 18f), new Vector2(1f, 0f));

            GuestBuffSlotView slot = slotObject.AddComponent<GuestBuffSlotView>();
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
            image.fillAmount = 0.6f;
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
