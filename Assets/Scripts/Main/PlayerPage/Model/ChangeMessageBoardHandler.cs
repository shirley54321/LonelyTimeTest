using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LitJson;
using Loading;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using Share.Tool;
using UnityEngine;
using UnityEngine.Events;

namespace Main.PlayerPage.Model
{
    /// <summary>
    /// Handles the process of changing the player's message board entry, including validation and PlayFab API calls.
    /// </summary>
    public class ChangeMessageBoardHandler : MonoBehaviour
    {
        #region Tools

        [SerializeField] private SensitiveWordFilter sensitiveWordFilter;

        #endregion

        public readonly UnityEvent OnValidateMessageBoardFailed = new UnityEvent();
        public readonly UnityEvent OnChangeMessageBoardSuccess = new UnityEvent();

        /// <summary>
        /// Initiates the process of changing the player's message board entry.
        /// </summary>
        /// <param name="newMessage">The new message board entry to set.</param>
        public void ChangeMessageBoard(string newMessage)
        {
            LoadingManger.Instance.Open_Loading_animator();
            // Check Sensitive Word
            bool hasSensitiveWord = sensitiveWordFilter.StringCheckAndReplace(newMessage, out _);
            bool containsSpecialCharOrSpace = Regex.IsMatch(newMessage, @"[\s\W]");
            if (hasSensitiveWord||containsSpecialCharOrSpace)
            {
                OnValidateMessageBoardFailed.Invoke();
                LoadingManger.Instance.Close_Loading_animator();
                return;
            }
            if (newMessage.Length > 120)
            {
                InstanceRemindPanel.Instance.OpenPanel("¶W¥X¦r¼Æ¤W­­");
                LoadingManger.Instance.Close_Loading_animator();
                return;
            }
            var tempMessageBoards = new List<MessageBoard>();
            //tempMessageBoards.AddRange(PlayerInfoManager.Instance.PlayerInfo.messageBoards);

            tempMessageBoards.Add(new MessageBoard()
            {
                Message = newMessage,
                UpdateTime = DateTime.Now
            });

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {nameof(MessageBoard), JsonMapper.ToJson(tempMessageBoards)}
                },
                Permission = UserDataPermission.Public
            }, (result) =>
            {
                Debug.Log($"{nameof(ChangeMessageBoard)} Success");

                PlayerInfoManager.Instance.SetLocalMessageBoards(tempMessageBoards);
                OnChangeMessageBoardSuccess?.Invoke();
                
                LoadingManger.Instance.Close_Loading_animator();
            }, (error) =>
            {
                Debug.Log($"{nameof(ChangeMessageBoard)} Fail {error}");
                // TODO: Add Server Error
                ServerEventHandler.Call_Server_Error_Event(error);
                LoadingManger.Instance.Close_Loading_animator();
            });
        }
    }
}
