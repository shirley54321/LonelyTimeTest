using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Player
{
    /// <summary>
    /// Manages the player's inventory
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        #region Instance(Singleton)
        private static InventoryManager instance;

        public static InventoryManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InventoryManager>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject with {typeof(InventoryManager)} does not exist in the scene, " +
                                       $"yet its method is being called.\n" +
                                       $"Please add {typeof(InventoryManager)} to the scene.");
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }


        #endregion

        public static readonly UnityEvent<int> OnDragonCoinChange = new UnityEvent<int>();
        public static readonly UnityEvent<int> OnDragonBallChange = new UnityEvent<int>();

        private int dragonCoin => PlayerInfoManager.Instance.PlayerInfo.dragonCoin;
        private int dragonBall => PlayerInfoManager.Instance.PlayerInfo.dragonBall;

        #region Subscribe Event
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Call all UI to update local player info on scene loaded
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnDragonCoinChange.Invoke(dragonCoin);
            StartCoroutine(DelayedInvoke());
        }
        public IEnumerator DelayedInvoke()
        {
            yield return new WaitForSeconds(8f);
            InventoryManager.Instance.UpdateDragonCoin();
            OnDragonCoinChange.Invoke(dragonCoin);
        }

        #endregion

        #region Dragon Coin

        /// <summary>
        /// Call When Dragon Coin Update
        /// </summary>
        [ContextMenu("Update Dragon Coin")]
        public void UpdateDragonCoin()
        {
            var request1 = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(request1, result =>
            {
                //Debug.Log($"result.VirtualCurrency {result.VirtualCurrency.Count}");
                foreach (var item in result.VirtualCurrency)
                {
                    //Debug.Log($"item.Key {item.Key}");
                    if (item.Key == "DC")
                    {
                        if (item.Value > 0)
                        {
                            SetLocalDragonCoin(item.Value);
                        }
                        else
                        {
                            SetLocalDragonCoin(item.Value);
                            AddVCoin(Math.Abs(item.Value));
                        }

                    }
                }


            }, error =>
            {
                Debug.Log("Error retrieving user inventory: " + error.ErrorMessage);
            });
        }
        private void AddVCoin(int amount)
        {
            var req = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = "DC",
                Amount = amount
            };

            PlayFabClientAPI.AddUserVirtualCurrency(req, OnAddSuccess, OnAddError);
        }
        private void OnAddError(PlayFabError error)
        {
            Debug.Log(error.ToString());
        }

        private void OnAddSuccess(ModifyUserVirtualCurrencyResult result)
        {
            //Success
        }
        public void InvokeUpdateDragonCoinEvent()
        {
            OnDragonCoinChange.Invoke(PlayerInfoManager.Instance.PlayerInfo.dragonCoin);
        }

        /// <summary>
        /// Set the local dragon coin, then invoke ui
        /// </summary>
        private void SetLocalDragonCoin(int coin)
        {
            PlayerInfoManager.Instance.SetDragonCoin(coin);
        }

        /// <summary>
        /// Get the local dragon coin count
        /// </summary>
        public int GetDragonCoin()
        {
            return dragonCoin;
        }


        #endregion

        #region Dragon Ball

        /// <summary>
        /// Call When Dragon Ball Update
        /// </summary>
        [ContextMenu("Update Dragon Ball")]
        public void UpdateDragonBall()
        {
            var request1 = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(request1, result =>
            {
                Debug.Log($"result.VirtualCurrency {result.VirtualCurrency.Count}");
                foreach (var item in result.VirtualCurrency)
                {
                    Debug.Log($"item.Key {item.Key}");
                    if (item.Key == "DB")
                    {
                        SetLocalDragonBall(item.Value);

                    }
                }


            }, error =>
            {
                Debug.Log("Error retrieving user inventory: " + error.ErrorMessage);
            });
        }

        /// <summary>
        /// Set the local dragon Ball, then invoke ui
        /// </summary>
        public void SetLocalDragonBall(int ball)
        {
            PlayerInfoManager.Instance.SetDragonBall(ball);
        }


        /// <summary>
        /// Get the local dragon Ball count
        /// </summary>
        public int GetDragonBall()
        {
            return dragonBall;
        }

        #endregion
    }
}