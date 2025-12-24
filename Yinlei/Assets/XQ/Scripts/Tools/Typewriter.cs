using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
namespace XQ
{
    public class Typewriter : MonoBehaviour
    {
        public static Typewriter Instance;
        private Dictionary<TMP_Text, Coroutine> activeCoroutines = new Dictionary<TMP_Text, Coroutine>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="content"></param>
        /// <param name="duration"></param>
        /// <param name="autoIndent">是否首行缩进</param>
        public void ShowText(TMP_Text text, string content, float duration, bool autoIndent = false)
        {
            if (activeCoroutines.ContainsKey(text))
            {
                StopCoroutine(activeCoroutines[text]);
            }

            // 如果需要自动缩进，添加首行缩进
            if (autoIndent)
            {
                content = AddFirstLineIndent(content);
            }

            // 解析富文本，获取可见字符和对应的富文本格式
            var parsedContent = ParseRichText(content);
            float typeSpeed = duration / parsedContent.visibleCharCount;

            Coroutine coroutine = StartCoroutine(IeShowText(text, parsedContent, typeSpeed));
            activeCoroutines[text] = coroutine;
        }

        /// <summary>
        /// 为文本添加首行缩进
        /// </summary>
        private string AddFirstLineIndent(string content)
        {
            // 使用全角空格进行缩进，视觉效果更好
            string indent = "　　"; // 两个全角空格

            // 如果内容以富文本标签开始，需要在标签后添加缩进
            if (content.StartsWith("<"))
            {
                // 找到第一个非标签字符的位置
                int firstCharIndex = FindFirstNonTagCharacter(content);
                if (firstCharIndex > 0)
                {
                    return content.Insert(firstCharIndex, indent);
                }
            }

            return indent + content;
        }

        /// <summary>
        /// 找到第一个非富文本标签字符的位置
        /// </summary>
        private int FindFirstNonTagCharacter(string content)
        {
            bool inTag = false;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] == '<')
                {
                    inTag = true;
                }
                else if (content[i] == '>')
                {
                    inTag = false;
                }
                else if (!inTag)
                {
                    return i;
                }
            }
            return 0;
        }

        IEnumerator IeShowText(TMP_Text text, ParsedRichText parsedContent, float typeSpeed)
        {
            WaitForSeconds wait = new WaitForSeconds(typeSpeed);

            for (int i = 0; i <= parsedContent.visibleCharCount; i++)
            {
                text.text = BuildTextWithRichTags(parsedContent, i);
                yield return wait;
            }

            // 移除已完成的协程
            activeCoroutines.Remove(text);
        }

        /// <summary>
        /// 解析富文本内容
        /// </summary>
        private ParsedRichText ParseRichText(string content)
        {
            ParsedRichText result = new ParsedRichText();
            result.characters = new List<CharacterInfo>();

            // 用于跟踪当前活动的富文本标签
            Stack<string> activeTagStack = new Stack<string>();

            // 正则表达式匹配富文本标签
            string pattern = @"<(/?)(\w+)(?:\s*=\s*([^>]*))?>|([^<]+)|<";
            MatchCollection matches = Regex.Matches(content, pattern);

            foreach (Match match in matches)
            {
                if (match.Groups[2].Success) // 富文本标签
                {
                    string tagName = match.Groups[2].Value;
                    bool isClosingTag = match.Groups[1].Success;
                    string tagValue = match.Groups[3].Success ? match.Groups[3].Value : "";

                    if (isClosingTag)
                    {
                        // 结束标签，从栈中移除
                        if (activeTagStack.Count > 0 && activeTagStack.Peek().StartsWith($"<{tagName}"))
                        {
                            activeTagStack.Pop();
                        }
                    }
                    else
                    {
                        // 开始标签，添加到栈中
                        string fullTag = string.IsNullOrEmpty(tagValue) ?
                            $"<{tagName}>" : $"<{tagName}={tagValue}>";
                        activeTagStack.Push(fullTag);
                    }
                }
                else if (match.Groups[4].Success) // 普通文本
                {
                    string text = match.Groups[4].Value;
                    foreach (char c in text)
                    {
                        CharacterInfo charInfo = new CharacterInfo
                        {
                            character = c,
                            tags = new List<string>(activeTagStack.ToArray())
                        };
                        // 因为栈是后进先出，需要反转顺序
                        charInfo.tags.Reverse();
                        result.characters.Add(charInfo);
                    }
                }
            }

            result.visibleCharCount = result.characters.Count;
            return result;
        }

        /// <summary>
        /// 根据显示的字符数量构建带富文本标签的字符串
        /// </summary>
        private string BuildTextWithRichTags(ParsedRichText parsedContent, int visibleCharCount)
        {
            if (visibleCharCount <= 0) return "";
            if (visibleCharCount > parsedContent.characters.Count)
                visibleCharCount = parsedContent.characters.Count;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            List<string> currentTags = new List<string>();

            for (int i = 0; i < visibleCharCount; i++)
            {
                CharacterInfo charInfo = parsedContent.characters[i];

                // 检查是否需要关闭一些标签
                for (int j = currentTags.Count - 1; j >= 0; j--)
                {
                    if (!charInfo.tags.Contains(currentTags[j]))
                    {
                        // 需要关闭这个标签
                        string tagName = ExtractTagName(currentTags[j]);
                        sb.Append($"</{tagName}>");
                        currentTags.RemoveAt(j);
                    }
                }

                // 检查是否需要打开新标签
                foreach (string tag in charInfo.tags)
                {
                    if (!currentTags.Contains(tag))
                    {
                        sb.Append(tag);
                        currentTags.Add(tag);
                    }
                }

                // 添加字符
                sb.Append(charInfo.character);
            }

            // 关闭所有未关闭的标签
            for (int i = currentTags.Count - 1; i >= 0; i--)
            {
                string tagName = ExtractTagName(currentTags[i]);
                sb.Append($"</{tagName}>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 从标签字符串中提取标签名
        /// </summary>
        private string ExtractTagName(string tag)
        {
            Match match = Regex.Match(tag, @"<(\w+)");
            return match.Success ? match.Groups[1].Value : "";
        }

        public void Stop()
        {
            foreach (var item in activeCoroutines)
            {
                StopCoroutine(item.Value);
            }
            activeCoroutines.Clear();
        }
    }

    /// <summary>
    /// 字符信息，包含字符和对应的富文本标签
    /// </summary>
    [System.Serializable]
    public class CharacterInfo
    {
        public char character;
        public List<string> tags = new List<string>();
    }

    /// <summary>
    /// 解析后的富文本内容
    /// </summary>
    [System.Serializable]
    public class ParsedRichText
    {
        public List<CharacterInfo> characters = new List<CharacterInfo>();
        public int visibleCharCount;
    }
}