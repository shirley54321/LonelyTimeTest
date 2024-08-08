
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Games.Data;
using Games.SelectedMachine;
using LitJson;
using Loading;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using PlayFab.CloudScriptModels;
using UnityEngine.SceneManagement;
using UserAccount;
using Unity.Services.Authentication;

namespace GameSelectedMenu
{
    /// <summary>
    /// Player can select the game he want to play
    /// This Manager can fetch GameSelectorList from PlayFab 
    /// </summary>
    public class GameSelectedMenuManager : MonoBehaviour
    {
        public static readonly UnityEvent<List<GameSelector>> OnGameMenuDataHasFetched = new UnityEvent<List<GameSelector>>();
        private LoadingScreen loadingScreen;

        #region Instance (Singleton)
        private static GameSelectedMenuManager instance;
        
        public static GameSelectedMenuManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameSelectedMenuManager>();
    
                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject of type {typeof(GameSelectedMenuManager)} is not present in the scene, " +
                                       $"yet its method is being called. Please add {typeof(GameSelectedMenuManager)} to the scene.");
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
            }
        }

        
        #endregion


        #region Variable
        [SerializeField] private GameMediaList gameMediaList;
        [SerializeField] private List<GameProfile> gameProfileList;
        [SerializeField] private FavoriteGameList favoriteGameList;
        [SerializeField] private GameSelectorList gameSelectorList;
        
        public List<GameSelector> GameSelectors => gameSelectorList.GameSelectors;

        private const string FavoriteGameKey = "FavoriteGameKey";
        private const string GameProfileKey = "GameProfileKey";
        
        private static bool gameProfileListHasFetched = false;

        #endregion
        

        #region Get GameSelectorList on enter scene
        
        void Start()
        {
            gameProfileListHasFetched = false;
            GetGameSelectorList();
        }

        [ContextMenu("StartFetchGameSelectorList")]
        public void GetGameSelectorList()
        {
            StartCoroutine(GetGameSelectorListCoroutine());
        }
        
        private IEnumerator GetGameSelectorListCoroutine()
        {
            LoadingManger.Instance.Open_Loading_animator();
            yield return new WaitUntil(() => UserAccountManager.Instance.HaveLogin());
            
            GetGameProfile();
            LoadFavoriteGame();

            // Wait gameProfile fetch finish
            yield return new WaitUntil(()=> gameProfileListHasFetched);
            
            // Combine Data
            gameSelectorList.CreateGameMenuList(gameProfileList,  favoriteGameList);
            OnGameMenuDataHasFetched.Invoke(gameSelectorList.GameSelectors);
            
            LoadingManger.Instance.Close_Loading_animator();
        }
        
        #endregion
        
        
        #region Handle Game Profile Data (Save, Load, Fetch)
        /// <summary>
        /// Fetch Game Profile if gameProfile has not fetch from PlayFab
        /// </summary>
        private void GetGameProfile()
        {
            LoadingManger.Instance.Open_Loading_animator(); //等待add to online list
            // Construct the CloudScript request to retrieve machine list
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "GetGameProfileList"
            };
            
            // Execute the CloudScript function to get machine list
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest, 
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }
                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    Debug.Log($"Result: {result.FunctionResult.ToString()}");
            
                    // Deserialize the CloudScript response into MachineList object
                    gameProfileList = JsonMapper.ToObject<List<GameProfile>>(result.FunctionResult.ToString());
                    gameProfileListHasFetched = true;

                    //加入online list
                    OnlineList onlineList = gameObject.AddComponent<OnlineList>();
                    onlineList.enterOnlineList();
                    LoadingManger.Instance.Close_Loading_animator(); //完成add to online list
                }, (PlayFabError error) =>
                {
                    Debug.Log($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }





        #endregion

        #region Handle Favorite Game Data (Save, Load, Fetch, Push)

        private void OnDisable()
        {
            SaveFavoriteGame();
            Debug.Log("=========");

        }

        [ContextMenu("Clear Favorite Data")]
        public void ClearFavoriteData()
        {
            PlayerPrefs.DeleteKey(FavoriteGameKey);
        }

        private void LoadFavoriteGame()
        {
            var value = PlayerPrefs.GetString(FavoriteGameKey, "");
            favoriteGameList = JsonUtility.FromJson<FavoriteGameList> (value) ?? new FavoriteGameList();

            Debug.Log($"{nameof(LoadFavoriteGame)} : {favoriteGameList}");
        }

        private void SaveFavoriteGame()
        {
            // Prepare
            favoriteGameList = new FavoriteGameList();
            foreach (var gameMenu in gameSelectorList.GameSelectors)
            {
                favoriteGameList.FavoriteGames.Add(gameMenu.FavoriteGame);
            }
            // Save
            var value = JsonUtility.ToJson(favoriteGameList);
            PlayerPrefs.SetString(FavoriteGameKey, value);
            Debug.Log($"{nameof(SaveFavoriteGame)} : {favoriteGameList}");

            //移出online list
            OnlineList onlineList = gameObject.AddComponent<OnlineList>();
            onlineList.leaveOnlineList();
            //OnlineList onlineList = gameObject.AddComponent<OnlineList>();
            //OnlineList.leaveOnlineListStaic();
        }
        
        public void SetFavoriteGame(GameId gameID, bool isFavorite)
        {
            var gameMenu = gameSelectorList.GameSelectors.FirstOrDefault(x => x.GameProfile.GameId == gameID);

            if (gameMenu != null) gameMenu.FavoriteGame.IsFavorite = isFavorite;
            SaveFavoriteGame();
            Debug.Log("=========");

        }


        #endregion


        #region EnterSelectedMachineLobby

        public void EnterSelectedMachineLobby(GameId gameID)
        {
            // TODO : Enter Scene
            MachineLobbyMediator.Instance.SelectedGameId = gameID;
            UserAccountManager.Instance.InvokeLoginSuccess();
            SceneManager.LoadScene("MachineLobby");
        }

        #endregion
        
    }
}

