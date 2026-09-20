using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TradePlay
{
    /// <summary>
    /// 把战斗过程打印到屏幕上。把 Status / Log 的 Text 拖到这里即可。
    /// </summary>
    public sealed class CombatLogView : MonoBehaviour
    {
        const int MaxLines = 24;

        [SerializeField] Text statusText;
        [SerializeField] Text logText;

        readonly List<string> _lines = new List<string>();
        readonly StringBuilder _builder = new StringBuilder();

        void Awake()
        {
            Font font = Font.CreateDynamicFontFromOSFont(
                new[] { "Microsoft YaHei", "微软雅黑", "SimHei", "Arial" },
                24);

            if (statusText != null)
            {
                statusText.font = font;
            }

            if (logText != null)
            {
                logText.font = font;
            }
        }

        public void Bind(Text status, Text log)
        {
            statusText = status;
            logText = log;
        }

        public void SetStatus(string text)
        {
            if (statusText != null)
            {
                statusText.text = text;
            }
        }

        public void Print(string message)
        {
            _lines.Add(message);
            while (_lines.Count > MaxLines)
            {
                _lines.RemoveAt(0);
            }

            _builder.Length = 0;
            for (int i = 0; i < _lines.Count; i++)
            {
                if (i > 0)
                {
                    _builder.Append('\n');
                }

                _builder.Append(_lines[i]);
            }

            if (logText != null)
            {
                logText.text = _builder.ToString();
            }

            Debug.Log("[TradePlay] " + message);
        }
    }
}
