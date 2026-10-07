using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TradePlay.Editor
{
    public sealed class BuffConfigWindow : EditorWindow
    {
        const string BuffsFolder = "Assets/Data/Buffs";

        Vector2 _listScroll;
        Vector2 _detailScroll;
        string _search = string.Empty;
        int _selectedIndex;
        readonly List<BuffData> _buffs = new List<BuffData>();
        UnityEditor.Editor _cachedEditor;

        [MenuItem("TradePlay/Buff配置工具")]
        public static void Open()
        {
            BuffConfigWindow window = GetWindow<BuffConfigWindow>("Buff配置");
            window.minSize = new Vector2(880f, 520f);
            window.RefreshList();
            window.Show();
        }

        void OnEnable()
        {
            RefreshList();
        }

        void OnDisable()
        {
            DestroyCachedEditor();
        }

        void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.BeginHorizontal();
            DrawList();
            DrawDetail();
            EditorGUILayout.EndHorizontal();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("新建Buff", EditorStyles.toolbarButton, GUILayout.Width(88f)))
            {
                CreateBuff();
            }

            if (GUILayout.Button("刷新列表", EditorStyles.toolbarButton, GUILayout.Width(88f)))
            {
                RefreshList();
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

        void DrawList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(240f));
            _listScroll = EditorGUILayout.BeginScrollView(_listScroll);
            int visibleIndex = 0;
            for (int i = 0; i < _buffs.Count; i++)
            {
                BuffData buff = _buffs[i];
                if (buff == null || !Matches(buff))
                {
                    continue;
                }

                if (GUILayout.Toggle(visibleIndex == _selectedIndex, DisplayName(buff), "Button") && _selectedIndex != visibleIndex)
                {
                    _selectedIndex = visibleIndex;
                    DestroyCachedEditor();
                    GUI.FocusControl(null);
                }

                visibleIndex++;
            }

            if (visibleIndex == 0)
            {
                EditorGUILayout.HelpBox("还没有 Buff。点击「新建Buff」。", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawDetail()
        {
            EditorGUILayout.BeginVertical("box");
            BuffData selected = GetSelected();
            if (selected == null)
            {
                EditorGUILayout.HelpBox("请选择一个 Buff。", MessageType.Info);
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

        void CreateBuff()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }

            if (!AssetDatabase.IsValidFolder(BuffsFolder))
            {
                AssetDatabase.CreateFolder("Assets/Data", "Buffs");
            }

            BuffData buff = CreateInstance<BuffData>();
            string path = AssetDatabase.GenerateUniqueAssetPath(BuffsFolder + "/新Buff.asset");
            AssetDatabase.CreateAsset(buff, path);
            AssetDatabase.SaveAssets();
            RefreshList();
            _search = string.Empty;
            _selectedIndex = Mathf.Max(0, _buffs.IndexOf(buff));
            DestroyCachedEditor();
            Selection.activeObject = buff;
        }

        void RefreshList()
        {
            _buffs.Clear();
            string[] guids = AssetDatabase.FindAssets("t:BuffData");
            for (int i = 0; i < guids.Length; i++)
            {
                BuffData buff = AssetDatabase.LoadAssetAtPath<BuffData>(AssetDatabase.GUIDToAssetPath(guids[i]));
                if (buff != null)
                {
                    _buffs.Add(buff);
                }
            }

            _buffs.Sort((a, b) => string.CompareOrdinal(DisplayName(a), DisplayName(b)));
            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, Mathf.Max(0, _buffs.Count - 1));
        }

        BuffData GetSelected()
        {
            int visibleIndex = 0;
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (_buffs[i] == null || !Matches(_buffs[i]))
                {
                    continue;
                }

                if (visibleIndex == _selectedIndex)
                {
                    return _buffs[i];
                }

                visibleIndex++;
            }

            return null;
        }

        bool Matches(BuffData buff)
        {
            if (string.IsNullOrEmpty(_search))
            {
                return true;
            }

            string keyword = _search.Trim();
            return buff.BuffName.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0
                   || buff.BuffId.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static string DisplayName(BuffData buff)
        {
            if (string.IsNullOrEmpty(buff.BuffName))
            {
                return buff.BuffId;
            }

            return buff.BuffName + "  (" + buff.BuffId + ")";
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
