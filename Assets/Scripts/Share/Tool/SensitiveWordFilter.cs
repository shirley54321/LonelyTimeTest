using System;
using System.IO;

namespace Share.Tool
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// * Writer: June
    /// * Date: 2021.11.10
    /// * Function: DFA Algorithm
    /// * Remarks: Used for filtering sensitive words
    ///
    /// Rule: https://hackmd.io/@Cobra3279/rkk6yPki3
    /// Reference: https://blog.csdn.net/qq_36374904/article/details/121292995
    /// </summary>
    public class SensitiveWordFilter : MonoBehaviour
    {
        private Hashtable hashtable;

        public List<string> filterList = new List<string>();

        [TextArea(3, 20)] public string testStr;

        [SerializeField] private bool loadSensitiveTextFile;
        [SerializeField] private TextAsset sensitiveTextFile;

        #region Build Hash Table

        private void Awake()
        {
            if (loadSensitiveTextFile)
            {
                ReadTextFile();
            }
            InitFilter(filterList);
        }

        private void ReadTextFile()
        {
            filterList = new List<string>();
            if (sensitiveTextFile != null)
            {
                string[] delimiters = { "\n", "\r\n" };
                string[] lines = sensitiveTextFile.text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                // string[] lines = new[] { "d" };
                foreach (string line in lines)
                {
                    filterList.Add(line);
                }
            }
            else
            {
                Debug.LogError("txtFile is not assigned. Please assign the txt file in the Inspector.");
            }
        }

        /// <summary>
        /// Initialize the filter
        /// </summary>
        /// <param name="wordList">Target word list container (List)</param>
        private void InitFilter(List<string> wordList)
        {
            // Initialize the hashtable
            hashtable = new Hashtable(wordList.Count);

            // Determine the outer loop iterations based on the word list container
            for (int i = 0; i < wordList.Count; i++)
            {
                // Local temporary hashtable
                Hashtable tmpHs = hashtable;

                for (int j = 0; j < wordList[i].Length; j++)
                {
                    // Split the string into individual characters
                    char ch = wordList[i][j];

                    // Check if the current character already exists as a key in the hashtable
                    if (tmpHs.ContainsKey(ch))
                    {
                        tmpHs = (Hashtable)tmpHs[ch];
                    }
                    else
                    {
                        Hashtable newHs = new Hashtable();
                        newHs.Add("IsEnd", 0); // Add 0 by default to indicate that the current character is not the last character
                        tmpHs.Add(ch, newHs); // Add the new hashtable as the value to the hashtable with the current character as the key (i.e., nested hashtable in the hashtable)
                        // Modify the value based on whether it is the last character by accessing the hashtable with IsEnd
                        tmpHs = newHs;
                    }
                    if (j == (wordList[i].Length - 1))
                    {
                        if (tmpHs.ContainsKey("IsEnd"))
                        {
                            tmpHs["IsEnd"] = 1;
                        }
                        else
                        {
                            tmpHs.Add("IsEnd", 1);
                        }
                    }
                }
            }
        }

        #endregion

        #region Sensitive Word Filter

        [ContextMenu("Test Sensitive Word")]
        private void TestSensitiveWord()
        {
            bool hasSensitiveWord = StringCheckAndReplace(testStr, out var filterStr);

            Debug.Log($"Sensitive Words: {hasSensitiveWord}, " +
                      $"Filtered Result: {filterStr}");
        }

        /// <summary>
        /// Check and replace sensitive words in a string
        /// </summary>
        /// <param name="targetStr">Target string</param>
        /// <param name="filterStr">Output string</param>
        /// <returns>is contain sensitive word</returns>
        public bool StringCheckAndReplace(string targetStr, out string filterStr)
        {
            StringBuilder stringBuilder = new StringBuilder(targetStr);

            int len = 0;
            bool hasSensitiveWord = false;
            for (int i = 0; i < targetStr.Length;)
            {
                len = SensitiveWordsLength(targetStr, i);
                // Skip processing if no filtered words found
                if (len == 0)
                {
                    i++;
                    continue;
                }

                hasSensitiveWord = true;
                for (int j = 0; j < len; j++)
                {
                    stringBuilder[i + j] = '*';
                }
                i += len;
            }

            filterStr = stringBuilder.ToString();
            return hasSensitiveWord;
        }

        /// <summary>
        /// Length of sensitive words
        /// </summary>
        /// <param name="targetStr">Target string</param>
        /// <param name="beginIndex">Starting index for iteration</param>
        /// <returns></returns>
        private int SensitiveWordsLength(string targetStr, int beginIndex)
        {
            // Current hashtable (node)
            Hashtable curHs = hashtable;
            // Record length
            int len = 0;
            // Start iterating from the given index
            for (int i = beginIndex; i < targetStr.Length; i++)
            {
                char ch = targetStr[i];
                
                // Determine if the current character is valid using ASCII
                if (IsValidityCharacter(ch))
                {
                    ch = CharToHalfWidth(ch);
                    ch = Char.ToLower(ch);

                    // Create a temporary hashtable, pointing to the child hashtable (child node)
                    Hashtable newtmpHs = (Hashtable)curHs[ch];

                    if (newtmpHs != null)
                    {
                        // Check if it is the last node
                        if ((int)newtmpHs["IsEnd"] == 1) len = i + 1 - beginIndex;

                        else curHs = newtmpHs; // Point to the child node (child hashtable)
                    }
                    else break;
                }
            }
            return len;
        }


        private static bool IsValidityCharacter(char c)
        {
            // 0x4E00 and 0x9FFF are hexadecimal representations of numbers that represent the range of
            // Chinese characters in the Unicode character set.
            return char.IsLetter(c) || char.IsDigit(c) || IsChineseCharacter(c) || IsZhuyinCharacter(c);
        }
        
        // Check if the character is a Chinese character
        private static bool IsChineseCharacter(char c)
        {
            // Unicode code point range: 0x4E00 ~ 0x9FFF
            return (c >= 0x4E00 && c <= 0x9FFF);
        }

        // Check if the character is a Zhuyin symbol
        private static bool IsZhuyinCharacter(char c)
        {
            // Unicode code point range: 0x3100 ~ 0x312F
            return (c >= 0x3100 && c <= 0x312F);
        }
        
        private static char CharToHalfWidth(char c)
        {
            if (c >= 0xFF01 && c <= 0xFF5E) // Full-width character range
            {
                return (char)(c - 0xFEE0);
            }
            else
            {
                return c;
            }
        }
        
        #endregion
    }
}
