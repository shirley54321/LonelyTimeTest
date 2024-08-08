using System.Collections.Generic;
using System.Linq;
using Games.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameSelectedMenu
{
    [CreateAssetMenu(fileName = "Game Media List", menuName = "LongLaiTan/GameMediaList")]
    public class GameMediaList : ScriptableObject
    {
       public List<GameMedia> GameMedias;

        public GameMedia GetData(GameId gameID)
        {
            var data = GameMedias.FirstOrDefault(x => x.GameID == gameID);
            if (data == null)
            {
                Debug.LogError($"GameId: {gameID} has not set {nameof(GameMedia)}\n" +
                               $"Please go to Assets/ScriptableObject/Main/GameMedia/Game Media Clip.asset set data");
            }

            return data;
        }
        
        public override string ToString()
        {
            string combinedString = string.Join("\n, ", GameMedias);
            return $"{nameof(GameMedias)}: \n{combinedString}";
        }
    }
}