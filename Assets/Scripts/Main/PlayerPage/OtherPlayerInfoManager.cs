using System;
using Loading;
using Player;
using UnityEngine;

namespace Main.PlayerPage
{
    /// <summary>
    /// Manages the information of other players in the game.
    /// </summary>
    public class OtherPlayerInfoManager : MonoBehaviour
    {
        #region Instance (Singleton)

        private static OtherPlayerInfoManager instance;

        /// <summary>
        /// Singleton instance of the OtherPlayerInfoManager.
        /// </summary>
        public static OtherPlayerInfoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<OtherPlayerInfoManager>();

                    if (instance == null)
                    {
                     //   Debug.LogError($"The GameObject of Type {typeof(OtherPlayerInfoManager)} is not present in the scene, " +
                     //                  $"yet its method is being called. Please add {typeof(OtherPlayerInfoManager)} to the scene.");
                    }

                   if(instance != null) DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        [SerializeField] private PlayerInfoBuilder playerInfoBuilder;

        /// <summary>
        /// Gets the information of the other player.
        /// </summary>
        public PlayerInfo OtherPlayerInfo => otherPlayerInfo;

        [SerializeField] private PlayerInfo otherPlayerInfo;

        private void OnEnable()
        {
            playerInfoBuilder.OnInfoBuilded.AddListener(OpenPanel);
        }

        private void OnDisable()
        {
            playerInfoBuilder.OnInfoBuilded.RemoveListener(OpenPanel);
        }

        /// <summary>
        /// Opens the panel displaying information about the other player.
        /// </summary>
        /// <param name="playFabId">The PlayFab ID of the other player.</param>
        public void OpenOtherPlayerPanel(string playFabId)
        {
            LoadingManger.Instance.Open_Loading_animator();
            playerInfoBuilder.BuildOtherPlayerInfo(playFabId);
        }

        private void OpenPanel(PlayerInfo playerInfo)
        {
            LoadingManger.Instance.Close_Loading_animator();
            
            otherPlayerInfo = playerInfo;
            UIManager.Instance.OpenPanel(UIConst.OtherPlayerPage);
        }
    }
}
