using System;
using UnityEngine;

namespace UserAccount.Tool
{
    /// <summary>
    /// Tool : Validate Account Format
    /// </summary>
    public static class AccountValidator
    {
        /// <summary>
        /// Validate account and password format
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ValidateAccountAndPasswordFormat(string accountName, string password)
        {
            bool accountNameValidator = ValidateFormat(accountName);
            bool passwordValidator = ValidateFormat(password);

            if (!accountNameValidator)
            {
                Debug.Log($"accountName {accountName} does not match the format");
            }

            if (!passwordValidator)
            {
                Debug.Log($"password {password} does not match the format");
            }


            return accountNameValidator && passwordValidator;
        }
        

        /// <summary>
        /// Validate password and confirm password equal or not
        /// </summary>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        public static bool ValidatePasswordAndConfirmPassword(string password, string confirmPassword)
        {
            bool passwordEqual = password.Equals(confirmPassword);

            if (!passwordEqual)
            {
                Debug.Log($"Password {password} and confirmPassword {confirmPassword} unequal");
            }

            return passwordEqual;
        }

        public static bool ValidateFormat(string str)
        {
            // Check if the length is between 8 and 12 characters.
            if (str.Length < 8 || str.Length > 12)
            {
                return false;
            }

            // Check contained Letter and number
            bool hasLetter = false;
            bool hasDigit = false;
            bool hasNotLetterOrDigit = false;

            foreach (char c in str)
            {
                if (Char.IsLetter(c))
                {
                    hasLetter = true;
                }
                else if (Char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else
                {
                    hasNotLetterOrDigit = true;
                }
            }

            if (hasLetter && hasDigit && !hasNotLetterOrDigit)
            {
                return true;
            }

            return false;
        }
    }
}