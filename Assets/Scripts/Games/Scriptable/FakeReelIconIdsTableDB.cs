using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Fake Reel Icon Ids Table DB", menuName = "ScriptableObject/Fake Reel Icon Ids Table DB")]
    public class FakeReelIconIdsTableDB : ScriptableObject
    {
        [SerializeField] TextAsset _sourceTextAsset;

        int[][] _tableOfBaseGame;
        public int[][] TableOfBaseGame
        {
            get
            {
                if (_tableOfBaseGame == null)
                    TryLoad();
                return _tableOfBaseGame;
            }
        }

        int[][] _tableOfBonusGame;
        public int[][] TableOfBonusGame
        {
            get
            {
                if (_tableOfBonusGame == null)
                    TryLoad();
                return _tableOfBonusGame;
            }
        }

        private bool TryLoad()
        {
            List<int[]> baseGameIconsIdOfReelList = new List<int[]>();
            List<int[]> bonusGameIconsIdOfReelList = new List<int[]>();

            string[] lines = _sourceTextAsset.text.Split(new char[] { '\n' });
            Type currentType = Type.None;

            foreach (string line in lines)
            {
                //Ignor comment 
                string activeTextInLine = line.Split(new char[] { '#' })[0].Replace("\0", "").Replace("\r", "");
                if (activeTextInLine == "")
                    currentType = Type.None;
                else if (activeTextInLine == "BS")
                    currentType = Type.BaseGame;
                else if (activeTextInLine == "FS")
                    currentType = Type.BonusGame;
                else
                {
                    string[] elements = activeTextInLine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    List<int> iconIdOnReelList = new List<int>();

                    if(currentType != Type.None)
                    {
                        foreach (string element in elements)
                        {
                            int iconId; 
                            if(int.TryParse(element, out iconId))
                                iconIdOnReelList.Add(iconId);
                            else
                            {
                                Debug.LogError("Invalid fake-reel-icon-IDs-table data!");
                                return false;
                            }
                        }

                        if (currentType == Type.BaseGame)
                            baseGameIconsIdOfReelList.Add(iconIdOnReelList.ToArray());
                        else if (currentType == Type.BonusGame)
                            bonusGameIconsIdOfReelList.Add(iconIdOnReelList.ToArray());
                    }
                }
            }

            _tableOfBaseGame = baseGameIconsIdOfReelList.ToArray();
            _tableOfBonusGame = bonusGameIconsIdOfReelList.ToArray();

            return true;
        }

        public enum Type
        {
            None,
            BaseGame,
            BonusGame
        }
    }
}
