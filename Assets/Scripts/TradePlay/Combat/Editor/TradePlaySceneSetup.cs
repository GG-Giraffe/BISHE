using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TradePlay.Editor
{
    [InitializeOnLoad]
    public static class TradePlaySceneSetup
    {
        static TradePlaySceneSetup()
        {
            EditorApplication.delayCall += CleanupIncompleteHud;
        }

        [MenuItem("TradePlay/重建战斗UI")]
        public static void RebuildHudMenu()
        {
            RemoveAllHud();
            CreateHud();
        }

        static void CleanupIncompleteHud()
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

            bool removed = RemoveIncompleteCanvases();
            if (removed)
            {
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        static bool RemoveIncompleteCanvases()
        {
            bool removed = false;
            List<GameObject> complete = new List<GameObject>();

            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            for (int i = 0; i < canvases.Length; i++)
            {
                GameObject canvasObject = canvases[i].gameObject;
                if (canvasObject.name != "TradePlayCanvas")
                {
                    continue;
                }

                bool isComplete = canvasObject.transform.Find("LogText") != null
                                  && canvasObject.transform.Find("EndTurnButton") != null;
                if (isComplete)
                {
                    complete.Add(canvasObject);
                    continue;
                }

                Object.DestroyImmediate(canvasObject);
                removed = true;
            }

            for (int i = 1; i < complete.Count; i++)
            {
                Object.DestroyImmediate(complete[i]);
                removed = true;
            }

            return removed;
        }

        static void RemoveAllHud()
        {
            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i].gameObject.name == "TradePlayCanvas")
                {
                    Object.DestroyImmediate(canvases[i].gameObject);
                }
            }

            TurnManager[] managers = Object.FindObjectsOfType<TurnManager>();
            for (int i = 0; i < managers.Length; i++)
            {
                Object.DestroyImmediate(managers[i].gameObject);
            }
        }

        static void CreateHud()
        {
            EnsureEventSystem();

            Canvas canvas = CreateCanvas();
            Text status = CreateLabel(canvas.transform, "StatusText", "第 1 回合  |  玩家回合", 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 1f), new Vector2(0f, -48f), new Vector2(900f, 64f));
            Text log = CreateLabel(canvas.transform, "LogText", "战斗日志会显示在这里\n进入 Play 后开始回合", 22, TextAnchor.UpperLeft, new Vector2(0f, 0.5f), new Vector2(48f, 20f), new Vector2(640f, 520f));
            Button endTurn = CreateEndTurnButton(canvas.transform);

            GameObject root = new GameObject("TradePlay");
            CombatLogView combatLog = root.AddComponent<CombatLogView>();
            PlayerTurnHandler player = root.AddComponent<PlayerTurnHandler>();
            EnemyTurnHandler enemy = root.AddComponent<EnemyTurnHandler>();
            TurnManager turnManager = root.AddComponent<TurnManager>();

            SerializedObject logSo = new SerializedObject(combatLog);
            logSo.FindProperty("statusText").objectReferenceValue = status;
            logSo.FindProperty("logText").objectReferenceValue = log;
            logSo.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject turnSo = new SerializedObject(turnManager);
            turnSo.FindProperty("log").objectReferenceValue = combatLog;
            turnSo.FindProperty("playerHandler").objectReferenceValue = player;
            turnSo.FindProperty("enemyHandler").objectReferenceValue = enemy;
            turnSo.FindProperty("endTurnButton").objectReferenceValue = endTurn;
            turnSo.ApplyModifiedPropertiesWithoutUndo();

            Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Create TradePlay HUD");
            Undo.RegisterCreatedObjectUndo(root, "Create TradePlay HUD");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = canvas.gameObject;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("TradePlayCanvas");
            canvasObject.layer = 5;

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        static Text CreateLabel(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Vector2 anchor, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject textObject = new GameObject(name);
            textObject.layer = 5;
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        static Button CreateEndTurnButton(Transform parent)
        {
            GameObject buttonObject = new GameObject("EndTurnButton");
            buttonObject.layer = 5;
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 40f);
            rect.sizeDelta = new Vector2(240f, 56f);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.45f, 0.78f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;

            GameObject labelObject = new GameObject("Label");
            labelObject.layer = 5;
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Text label = labelObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = "结束回合";
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.raycastTarget = false;

            return button;
        }
    }
}
