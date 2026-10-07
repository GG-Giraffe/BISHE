using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayActorSlotSetup
    {
        const string PlayerPrefabPath = "Assets/Prefab/Player.prefab";
        const string AntiquePrefabPath = "Assets/Prefab/Antique.prefab";
        const string GuestPrefabPath = "Assets/Prefab/Guest.prefab";

        static TradePlayActorSlotSetup()
        {
            EditorApplication.delayCall += TryCreateSlots;
        }

        [MenuItem("TradePlay/创建角色站位")]
        public static void CreateSlotsMenu()
        {
            CreateSlots(true);
        }

        static void TryCreateSlots()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            Scene scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded || scene.name != "SampleScene")
            {
                return;
            }

            if (GameObject.Find("ActorSlots") != null)
            {
                return;
            }

            CreateSlots(false);
        }

        static void CreateSlots(bool force)
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("场景里没有 Canvas，无法创建角色站位。");
                return;
            }

            Transform existing = canvas.transform.Find("ActorSlots");
            if (existing != null && !force)
            {
                return;
            }

            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            GameObject root = new GameObject("ActorSlots", typeof(RectTransform));
            root.transform.SetParent(canvas.transform, false);
            Stretch(root.GetComponent<RectTransform>());

            RectTransform playerSlot = CreateSlot(root.transform, "PlayerSlot", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(48f, 0f), new Vector2(360f, 520f));
            RectTransform antiqueSlot = CreateSlot(root.transform, "AntiqueSlot", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(280f, 420f));
            RectTransform guestSlot = CreateSlot(root.transform, "GuestSlot", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-48f, 0f), new Vector2(360f, 520f));

            PlaceActor(playerSlot, PlayerPrefabPath);
            PlaceActor(antiqueSlot, AntiquePrefabPath);
            PlaceActor(guestSlot, GuestPrefabPath);

            BattleLayout layout = root.AddComponent<BattleLayout>();
            SerializedObject so = new SerializedObject(layout);
            so.FindProperty("playerSlot").objectReferenceValue = playerSlot;
            so.FindProperty("antiqueSlot").objectReferenceValue = antiqueSlot;
            so.FindProperty("guestSlot").objectReferenceValue = guestSlot;
            so.FindProperty("playerPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<PlayerView>(PlayerPrefabPath);
            so.FindProperty("antiquePrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AntiqueView>(AntiquePrefabPath);
            so.FindProperty("guestPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GuestView>(GuestPrefabPath);
            so.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(root, "Create Actor Slots");
            EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
            Selection.activeGameObject = root;
            Debug.Log("已创建角色站位。拖动 PlayerSlot、AntiqueSlot、GuestSlot 调整位置。");
        }

        static RectTransform CreateSlot(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            GameObject slotObject = new GameObject(name, typeof(RectTransform));
            slotObject.transform.SetParent(parent, false);
            RectTransform rect = slotObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMin;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            GameObject frameObject = new GameObject("SlotFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            frameObject.transform.SetParent(slotObject.transform, false);
            RectTransform frameRect = frameObject.GetComponent<RectTransform>();
            Stretch(frameRect);
            Image image = frameObject.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);
            image.raycastTarget = false;
            return rect;
        }

        static void PlaceActor(RectTransform slot, string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, slot);
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
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
