using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlayCardZoneSetup
    {
        const string CardPrefabPath = "Assets/Prefab/Card.prefab";
        const string CountdownPrefabPath = "Assets/Prefab/Countdown.prefab";

        static TradePlayCardZoneSetup()
        {
            EditorApplication.delayCall += TryCreate;
        }

        [MenuItem("TradePlay/创建牌区和倒计时")]
        public static void CreateMenu()
        {
            Create(true);
        }

        static bool _retried;

        static void TryCreate()
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

            if (!File.Exists(CountdownPrefabPath) && !_retried)
            {
                _retried = true;
                EditorApplication.delayCall += TryCreate;
                return;
            }

            if (Object.FindObjectOfType<CardZone>() != null && Object.FindObjectOfType<CountdownView>() != null)
            {
                return;
            }

            Create(false);
        }

        static void Create(bool force)
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            if (force || Object.FindObjectOfType<CardZone>() == null)
            {
                CreateHand(canvas.transform);
            }

            if (force || Object.FindObjectOfType<CountdownView>() == null)
            {
                CreateCountdown(canvas.transform);
            }

            if (Object.FindObjectOfType<CardBattleFlow>() == null)
            {
                GameObject flowObject = new GameObject("CardBattleFlow");
                flowObject.AddComponent<CardBattleFlow>();
                Undo.RegisterCreatedObjectUndo(flowObject, "Create Card Battle Flow");
            }

            EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        }

        static void CreateHand(Transform canvas)
        {
            Transform existing = canvas.Find("HandArea");
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            GameObject hand = new GameObject("HandArea", typeof(RectTransform));
            hand.transform.SetParent(canvas, false);
            RectTransform rect = hand.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 110f);
            rect.sizeDelta = new Vector2(1100f, 220f);

            CardZone zone = hand.AddComponent<CardZone>();
            SerializedObject so = new SerializedObject(zone);
            so.FindProperty("handArea").objectReferenceValue = rect;
            so.FindProperty("cardPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<CardView>(CardPrefabPath);
            SerializedProperty deck = so.FindProperty("startingDeck");
            string[] guids = AssetDatabase.FindAssets("t:CardData");
            deck.arraySize = guids.Length;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                deck.GetArrayElementAtIndex(i).objectReferenceValue = AssetDatabase.LoadAssetAtPath<CardData>(path);
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            Undo.RegisterCreatedObjectUndo(hand, "Create Hand Area");
        }

        static void CreateCountdown(Transform canvas)
        {
            Transform existing = canvas.Find("Countdown");
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CountdownPrefabPath);
            if (prefab == null)
            {
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas);
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -24f);
            instance.AddComponent<CountdownState>();
            Undo.RegisterCreatedObjectUndo(instance, "Create Countdown");
        }
    }
}
