using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TradePlay.Editor
{
    public sealed class CardConfigWindow : EditorWindow
    {
        const string CardsFolder = "Assets/Data/Cards";

        Vector2 _listScroll;
        Vector2 _detailScroll;
        string _search = string.Empty;
        int _selectedIndex;
        List<CardData> _cards = new List<CardData>();
        UnityEditor.Editor _cachedEditor;

        [MenuItem("TradePlay/卡牌配置工具")]
        public static void Open()
        {
            CardConfigWindow window = GetWindow<CardConfigWindow>("卡牌配置");
            window.minSize = new Vector2(880f, 520f);
            window.RefreshCardList();
            window.Show();
        }

        void OnEnable()
        {
            RefreshCardList();
        }

        void OnDisable()
        {
            DestroyCachedEditor();
        }

        void OnFocus()
        {
            RefreshCardList();
        }

        void OnGUI()
        {
            DrawToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawCardList();
            DrawDetail();
            EditorGUILayout.EndHorizontal();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("新建卡牌", EditorStyles.toolbarButton, GUILayout.Width(88f)))
            {
                CreateCard();
            }

            if (GUILayout.Button("刷新列表", EditorStyles.toolbarButton, GUILayout.Width(88f)))
            {
                RefreshCardList();
            }

            GUILayout.Space(8f);
            GUILayout.Label("搜索", GUILayout.Width(36f));
            string nextSearch = GUILayout.TextField(_search, EditorStyles.toolbarSearchField);
            if (nextSearch != _search)
            {
                _search = nextSearch;
                _selectedIndex = 0;
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawCardList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(240f));
            _listScroll = EditorGUILayout.BeginScrollView(_listScroll);

            int visibleIndex = 0;
            for (int i = 0; i < _cards.Count; i++)
            {
                CardData card = _cards[i];
                if (card == null || !MatchesSearch(card))
                {
                    continue;
                }

                bool selected = visibleIndex == _selectedIndex;
                if (GUILayout.Toggle(selected, DisplayName(card), "Button"))
                {
                    if (_selectedIndex != visibleIndex)
                    {
                        _selectedIndex = visibleIndex;
                        DestroyCachedEditor();
                        GUI.FocusControl(null);
                    }
                }

                visibleIndex++;
            }

            if (visibleIndex == 0)
            {
                EditorGUILayout.HelpBox("还没有卡牌。点击左上角「新建卡牌」开始配置。", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawDetail()
        {
            EditorGUILayout.BeginVertical("box");
            CardData selected = GetSelectedCard();
            if (selected == null)
            {
                EditorGUILayout.HelpBox("请选择一张卡牌。", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            _detailScroll = EditorGUILayout.BeginScrollView(_detailScroll);
            if (_cachedEditor == null || _cachedEditor.target != selected)
            {
                DestroyCachedEditor();
                _cachedEditor = UnityEditor.Editor.CreateEditor(selected);
            }

            _cachedEditor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void CreateCard()
        {
            EnsureCardsFolder();

            CardData card = CreateInstance<CardData>();
            string path = AssetDatabase.GenerateUniqueAssetPath(CardsFolder + "/新卡牌.asset");
            AssetDatabase.CreateAsset(card, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            RefreshCardList();
            _search = string.Empty;
            _selectedIndex = Mathf.Max(0, _cards.IndexOf(card));
            DestroyCachedEditor();
            Selection.activeObject = card;
            EditorGUIUtility.PingObject(card);
        }

        void RefreshCardList()
        {
            _cards.Clear();
            string[] guids = AssetDatabase.FindAssets("t:CardData");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                CardData card = AssetDatabase.LoadAssetAtPath<CardData>(path);
                if (card != null)
                {
                    _cards.Add(card);
                }
            }

            _cards.Sort((a, b) => string.CompareOrdinal(DisplayName(a), DisplayName(b)));
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, Mathf.Max(0, _cards.Count - 1));
        }

        CardData GetSelectedCard()
        {
            int visibleIndex = 0;
            for (int i = 0; i < _cards.Count; i++)
            {
                CardData card = _cards[i];
                if (card == null || !MatchesSearch(card))
                {
                    continue;
                }

                if (visibleIndex == _selectedIndex)
                {
                    return card;
                }

                visibleIndex++;
            }

            return null;
        }

        bool MatchesSearch(CardData card)
        {
            if (string.IsNullOrEmpty(_search))
            {
                return true;
            }

            string keyword = _search.Trim();
            return card.CardName.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0
                   || card.CardId.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static string DisplayName(CardData card)
        {
            if (string.IsNullOrEmpty(card.CardName))
            {
                return card.CardId;
            }

            return card.CardName + "  (" + card.CardId + ")";
        }

        static void EnsureCardsFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }

            if (!AssetDatabase.IsValidFolder(CardsFolder))
            {
                AssetDatabase.CreateFolder("Assets/Data", "Cards");
            }
        }

        void DestroyCachedEditor()
        {
            if (_cachedEditor != null)
            {
                DestroyImmediate(_cachedEditor);
                _cachedEditor = null;
            }
        }
    }
}
