using System.Text.RegularExpressions;
using UnityEngine;

namespace Share.Tool
{
    /// <summary>
    /// Validate whether the string matches the specified format.
    /// </summary>
    public class StringFormatValidator : MonoBehaviour
    {
        private int _minWeight;
        private int _maxWeight;
        private int _chineseCharacterWeight;
        private int _englishAndDigitalCharacterWeight;


        [SerializeField] private SensitiveWordFilter sensitiveWordFilter;
        

        #region Set Validate Format
        
        public void SetValidateFormat(int minCharacter, int maxCharacter)
        {
            _minWeight = minCharacter;
            _maxWeight = maxCharacter;
            _chineseCharacterWeight = 1;
            _englishAndDigitalCharacterWeight = 1;
        }
        
        public void SetValidateFormat(int minWeight, int maxWeight, int chineseCharacterWeight, int englishAndDigitalCharacterWeight)
        {
            _minWeight = minWeight;
            _maxWeight = maxWeight;
            _chineseCharacterWeight = chineseCharacterWeight;
            _englishAndDigitalCharacterWeight = englishAndDigitalCharacterWeight;
        }

        #endregion

        #region Validate
        public ValidateResult ValidateInput(string input)
        {
            if (ContainsSpecialCharacters(input))
            {
                return ValidateResult.ContainSpecialCharacters;
            }

            if (IsTooLong(input))
            {
                return ValidateResult.TooLong;
            }

            if (IsTooShort(input))
            {
                return ValidateResult.TooShort;
            }
            
            if (ContainSensitiveWord(input))
            {
                return ValidateResult.ContainSensitiveWord;
            }

            return ValidateResult.Pass;
        }
        
        #endregion
        
        #region Private

        private bool ContainSensitiveWord(string input)
        {
            return sensitiveWordFilter.StringCheckAndReplace(input, out _);
        }
        
        private bool ContainsSpecialCharacters(string input)
        {
            // Regular expression pattern to match any character that is not English or Chinese
            string pattern = @"[^a-zA-Z0-9\u4E00-\u9FFF]";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        private bool IsTooLong(string input)
        {
            int totalWeight = CalculateStringWeight(input);
            return totalWeight > _maxWeight;
        }
        
        private bool IsTooShort(string input)
        {
            int totalWeight = CalculateStringWeight(input);
            return totalWeight < _minWeight;
        }
        
        private int CalculateStringWeight(string input)
        {
            int weight = 0;
        
            foreach (char c in input)
            {
                if (IsChineseCharacter(c))
                {
                    weight += _chineseCharacterWeight;
                }
                else if (IsEnglishOrDigitalCharacter(c))
                {
                    weight += _englishAndDigitalCharacterWeight;
                }
            }
        
            return weight;
        }
    
        private static bool IsChineseCharacter(char c)
        {
            // 0x4E00 and 0x9FFF are hexadecimal representations of numbers that represent the range of
            // Chinese characters in the Unicode character set.
            return c >= 0x4E00 && c <= 0x9FFF;
        }
    
        private static bool IsEnglishOrDigitalCharacter(char c)
        {
            return char.IsLetter(c) || char.IsDigit(c);
        }

        #endregion
    }
}