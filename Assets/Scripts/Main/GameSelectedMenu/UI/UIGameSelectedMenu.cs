using System;
using System.Collections.Generic;
using Games.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameSelectedMenu
{
    public class GameSelectorComparer : IComparer<UIGameSelector>
    {
        public int Compare(UIGameSelector x, UIGameSelector y)
        {
            // 比較兩個GameSelector的GameID
            return y.gameSelector.GameProfile.Mark.CompareTo(x.gameSelector.GameProfile.Mark);
        }
    }
    
    /// <summary>
    /// The menu for the game selected in the lobby
    /// Provide filter game method with gameType, isHot, isNew, isFavorite
    /// </summary>
    public class UIGameSelectedMenu : MonoBehaviour
    {
        [SerializeField] private Transform spawnTransform;

        [SerializeField] private UIGameSelector [] _gameSelectorUIs;
        
        public GameObject myText;

        private Dictionary<GameId, UIGameSelector> _gameSelectorDict;

        private void Update() {
            if (spawnTransform.childCount > 0)
            {
                myText.SetActive(false);
            }
            else
            {
                myText.SetActive(true);
            }
        }

        private void Awake()
        {
            _gameSelectorUIs = spawnTransform.GetComponentsInChildren<UIGameSelector>();
            
            _gameSelectorDict = new Dictionary<GameId, UIGameSelector>();
            foreach (var uiGameSelector in _gameSelectorUIs)
            {
                if (_gameSelectorDict.ContainsKey(uiGameSelector.GameId))
                {
                    Debug.LogError($"Has repeat gameSelectorUI for {uiGameSelector.GameId} in {nameof(_gameSelectorUIs)}");
                }
                else
                {
                    _gameSelectorDict.Add(uiGameSelector.GameId, uiGameSelector);
                }
            }
        }

        #region Create Game Selecter UI

        private void OnEnable()
        {
            GameSelectedMenuManager.OnGameMenuDataHasFetched.AddListener(CreateUIGameSelector);
        }
        
        private void OnDisable()
        {
            GameSelectedMenuManager.OnGameMenuDataHasFetched.RemoveListener(CreateUIGameSelector);
        }

        private void CreateUIGameSelector(List<GameSelector> gameSelectorList)
        {
            foreach (var gameSelector in gameSelectorList)
            {
                if (_gameSelectorDict.TryGetValue(gameSelector.GameID, out var uiGameSelector))
                {
                    uiGameSelector.UpdateUI(gameSelector);
                }
                else
                {
                    Debug.LogError($"Does contain gameSelectorUI for {gameSelector.GameID} in {nameof(_gameSelectorUIs)}.\n" +
                                   $"Please go to Assets/Prefabs/Main/GameSelectedMenu/GameSelectedMenuUI.prefab to set.");
                }
            }

            AdjustHierarchyOrder();
        }

        private void AdjustHierarchyOrder()
        {
            // 使用自定義的比較器對GameSelector陣列進行排序
            Array.Sort(_gameSelectorUIs, new GameSelectorComparer());
            
            for (var index = 0; index < _gameSelectorUIs.Length; index++)
            {
                var selectorUI = _gameSelectorUIs[index];
                selectorUI.transform.SetSiblingIndex(index);
            }
        }
        

        #endregion

        #region Filter Game Selector


        public void FilterAllGame()
        {
            UpdateAllSelectorData();
            foreach (var gameSelector in _gameSelectorUIs)
            {
                gameSelector.gameObject.SetActive(true);
            }
        }

        [ContextMenu("FilterHotGame")]
        public void FilterHotGame()
        {
            UpdateAllSelectorData();
            foreach (var gameSelector in _gameSelectorUIs)
            {
                bool setActive = gameSelector.gameSelector.GameProfile.Mark == Mark.Hot;
                gameSelector.gameObject.SetActive(setActive);
            }
        }


        [ContextMenu("FilterNewGame")]
        public void FilterNewGame()
        {
            UpdateAllSelectorData();
            foreach (var gameSelector in _gameSelectorUIs)
            {
                bool setActive = gameSelector.gameSelector.GameProfile.Mark == Mark.New;
                gameSelector.gameObject.SetActive(setActive);
            }
        }

        public void FilterFavoriteGame()
        {
            UpdateAllSelectorData();
            foreach (var gameSelector in _gameSelectorUIs)
            {
                bool setActive = gameSelector.gameSelector.FavoriteGame.IsFavorite;
                gameSelector.gameObject.SetActive(setActive);
            }
        }

        public void FilterCardGame()
        {
            FilterGame(GameType.Card);
        }

        public void FilterFishGame()
        {
            FilterGame(GameType.Fish);
        }

        public void FilterTriggerGame()
        {
            FilterGame(GameType.SlotMachine);
        }

        public void FilterGame(GameType gameType)
        {
            UpdateAllSelectorData();
            foreach (var gameSelector in _gameSelectorUIs)
            {
                bool setActive = gameSelector.gameSelector.GameProfile.GameType == gameType;
                gameSelector.gameObject.SetActive(setActive);
            }
        }


        private void UpdateAllSelectorData()
        {
            var gameMenuDataList = GameSelectedMenuManager.Instance.GameSelectors;
            foreach (var gameMenuData in gameMenuDataList)
            {
                GameId gameID = gameMenuData.GameProfile.GameId;
                _gameSelectorDict[gameID].UpdateUI(gameMenuData);
            }
        }


        #endregion
    }
}