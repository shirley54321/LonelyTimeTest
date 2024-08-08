using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace UserAccount
{
    public class QuickLoginIdHandler
    {
        
        private const string QuickLoginIDKey = "PlayFabQuickLoginID";
        /// <summary>
        /// This ID is used for quick device login.
        /// If the ID exists locally and is also recorded on the server, the user can log in directly.
        /// Pass Null for a value to have one auto-generated.
        /// </summary>
        public string QuickLoginID
        {
            get
            {
                return PlayerPrefs.GetString(QuickLoginIDKey, "");
            }
            private set
            {
                var id = value ;    
                PlayerPrefs.SetString(QuickLoginIDKey, id);
            }
        }

        public void LinkWithQuickLoginID()
        {
            CheckThenGenerateQuickLoginId();
            
            var request = new LinkCustomIDRequest()
            {
                CustomId = QuickLoginID,
                ForceLink = true
            };

            PlayFabClientAPI.LinkCustomID(request,
                (result) => { Debug.Log($"<color=green>Link account success</color>"); },
                (error) =>
                {
                    Debug.Log($"<color=red>Link account Fail</color>\n{error.Error}:  {error}");
                    throw new NotImplementedException();
                });

        }

        public bool CheckDeviceHaveQuickLoginId()
        {
            return QuickLoginID != "";
        }

        private void CheckThenGenerateQuickLoginId()
        {
            if (!CheckDeviceHaveQuickLoginId())
            {
                QuickLoginID = Guid.NewGuid().ToString();  
            }
        }

        [ContextMenu("ClearQuickLoginID")]
        public void ClearGuID()
        {
            Debug.Log($"Clear PlayPrefs key: {QuickLoginIDKey} ,value: {QuickLoginID }");
            PlayerPrefs.DeleteKey(QuickLoginIDKey);
        }

    }
}