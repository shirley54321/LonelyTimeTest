using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    public class RoundResultData
    {
        public int[,] showedIconsId = null;
        public decimal totalWin = 0;
        public WonSituationInfo[] wonSituationsInfo = new WonSituationInfo[0];
        public int bonusIconId = -1;
        //公版沒有swild
        public int swildIconId = -1;
        public SwildIconWinSituationInfo swildIconWinSituationInfo = new SwildIconWinSituationInfo();

        public int gainedBonusRounds = 0;
        public BonusGameInfo bonusGameInfo = new BonusGameInfo();


        public int BetRate
        {
            get
            {
                decimal totalBaseWinOfWonSituations = WonSituationInfo.GetTotalBaseWinOfWonSituations(wonSituationsInfo);

                if (totalBaseWinOfWonSituations > 0)
                {
                    if (bonusGameInfo.IsBonusRound)
                    {
                        return (int)(totalWin / totalBaseWinOfWonSituations) / WonBonusSituationsInfo.Length;
                    }
                    return (int)(totalWin / totalBaseWinOfWonSituations);
                }
                return 0;
            }
        }
        public WonSituationInfo[] WonBonusSituationsInfo => GetWonSituationsInfoOfSpecificIconId(bonusIconId);
        public WonSituationInfo[] WonNonBonusSituationsInfo => GetWonSituationsInfoWithoutSpecificIconId(bonusIconId);



        public bool HasWon => wonSituationsInfo.Length > 0;
        public int WonBonusSituationsCount => WonBonusSituationsInfo.Length;
        public bool IsBonusOccurred => WonBonusSituationsInfo.Length > 0;
        //公版沒有swild
        public bool WonSwildOccurred => (swildIconWinSituationInfo != null && swildIconWinSituationInfo.reelsIndexWithWonIconAppeared.Length > 0) ? true : false;

        public int[] GetContainedWonIconId(SlotLinesDB slotLinesDB)
        {
            return WonSituationInfo.GetIconsIdOfWonSituations(wonSituationsInfo, showedIconsId, slotLinesDB);
        }

        public WonSituationInfo[] GetWonSituationsInfoOfSpecificIconId(int iconId)
        {
            List<WonSituationInfo> result = new List<WonSituationInfo>();
            foreach (WonSituationInfo info in wonSituationsInfo)
            {
                if (info.wonIconId == iconId)
                {
                    result.Add(info);
                }
            }
            //Debug.Log(result.ToArray().Length);
            return result.ToArray();
        }

        public WonSituationInfo[] GetWonSituationsInfoWithoutSpecificIconId(int iconId)
        {
            List<WonSituationInfo> result = new List<WonSituationInfo>();

            foreach (WonSituationInfo info in wonSituationsInfo)
            {
                if (info.wonIconId != iconId)
                {
                    result.Add(info);
                }
            }
            return result.ToArray();
        }


        public override string ToString()
        {

            string result = "RoundResultData: TO_STRING\n";

            result += "- showedIconsId: [";

            for (int i = 0; i < showedIconsId.GetLength(0); i++)
            {
                string showedOnReel = "[";

                for (int j = 0; j < showedIconsId.GetLength(1); j++)
                {
                    showedOnReel += showedIconsId[i, j];

                    if (j < showedIconsId.GetLength(1) - 1)
                    {
                        showedOnReel += ", ";
                    }
                }
                showedOnReel += "]";
                result += showedOnReel;

                if (i < showedIconsId.GetLength(0) - 1)
                {
                    result += ", ";
                }
            }
            result += "]\n";

            result += "- totalWin: " + totalWin + "\n";

            result += $"- wonSituationsInfo: [{wonSituationsInfo.Length}]\n";
            for (int i = 0; i < wonSituationsInfo.Length; i++)
            {
                result += "- - " + wonSituationsInfo[i].ToString() + "\n";
            }
            //公版沒有
            if (WonSwildOccurred)
            {
                result += "- swildIconWinSituationInfo:\n";
                result += "- - " + swildIconWinSituationInfo.ToString() + "\n";
                for (int i = 0; i < swildIconWinSituationInfo.SwildIconCount; i++)
                {
                    result += "- - reelsIndexWithWonIconAppeared:" + swildIconWinSituationInfo.reelsIndexWithWonIconAppeared[i] + "\n";
                }

            }
            result += "- IsBonusOccurred: " + IsBonusOccurred + "\n";
            result += "- gainedBonusRounds: " + gainedBonusRounds + "\n";
            result += "- bonusGameInfo: " + bonusGameInfo.ToString();

            return result;
        }



        // == Nested Classes ==
        // Situation when win a line or non-on-line-bonus occurred
        public abstract class WonSituationInfo
        {
            public int wonIconId;
            public int[] reelsIndexWithWonIconAppeared;
            public decimal baseWinScore;

            public int WonIconCount => reelsIndexWithWonIconAppeared.Length;

            //給IconID
            public static int[] GetIconsIdOfWonSituations(WonSituationInfo[] wonSituationsInfo, int[,] showedIconsId, SlotLinesDB slotLinesDB)
            {
                List<int> result = new List<int>();

                Vector2Int[] coords = GetTotalWonIconsCoord(wonSituationsInfo, showedIconsId, slotLinesDB);

                foreach (var coord in coords)
                {
                    int iconId = showedIconsId[coord.x, coord.y];
                    if (!result.Contains(iconId))
                    {
                        result.Add(iconId);
                    }
                }

                return result.ToArray();
            }

            public static decimal GetTotalBaseWinOfWonSituations(WonSituationInfo[] wonSituationsInfo)
            {
                decimal result = 0;
                foreach (WonSituationInfo info in wonSituationsInfo)
                {
                    result += info.baseWinScore;
                }
                return result;
            }
            //給位置
            public static Vector2Int[] GetTotalWonIconsCoord(WonSituationInfo[] wonSituationInfos, int[,] showedIconsId, SlotLinesDB slotLinesDB)
            {
                var result = new List<Vector2Int>();

                foreach (WonSituationInfo info in wonSituationInfos)
                {
                    Vector2Int[] coords = new Vector2Int[0];

                    if (info is WonLineInfo)
                    {
                        coords = info.GetWonIconsCoord(new WonLineInfo.GettingWonIconsCoordRequiredArgs
                        {
                            slotLinesDB = slotLinesDB
                        });
                    }
                    else if (info is ScatteredIconWinSituationInfo)
                    {
                        coords = info.GetWonIconsCoord(new ScatteredIconWinSituationInfo.GettingWonIconsCoordRequiredArgs
                        {
                            showedIconsId = showedIconsId
                        });
                    }

                    foreach (var coord in coords)
                    {
                        if (!result.Contains(coord))
                        {
                            result.Add(coord);
                        }
                    }
                }

                return result.ToArray();
            }


            // Get the coordinates of won icons
            public abstract Vector2Int[] GetWonIconsCoord(object args);



            public override string ToString()
            {
                return $"wonIconId: {wonIconId}, wonIconCount: {WonIconCount}, baseWinScore: {baseWinScore}";
            }

        }

        public class WonLineInfo : WonSituationInfo
        {
            public int lineIndex;

            public static WonLineInfo[] GetContainedWonLineInfos(WonSituationInfo[] wonSituationInfos)
            {
                var result = new List<WonLineInfo>();

                foreach (var info in wonSituationInfos)
                {
                    if (info is WonLineInfo)
                    {
                        result.Add((WonLineInfo)info);
                    }
                }
                return result.ToArray();
            }

            public static int[] GetLinesIndexArray(WonLineInfo[] wonLinesInfo)
            {
                int[] result = new int[wonLinesInfo.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = wonLinesInfo[i].lineIndex;
                }
                return result;
            }


            public override Vector2Int[] GetWonIconsCoord(object args)
            {
                if (args is GettingWonIconsCoordRequiredArgs)
                {
                    SlotLinesDB slotLineDB = ((GettingWonIconsCoordRequiredArgs)args).slotLinesDB;

                    Vector2Int[] coords = new Vector2Int[WonIconCount];
                    int[] positionOnColumns = slotLineDB.lines[lineIndex].positionOnColumns;

                    for (int i = 0; i < coords.Length; i++)
                    {
                        int reelIndex = reelsIndexWithWonIconAppeared[i];
                        coords[i] = new Vector2Int(reelIndex, positionOnColumns[reelIndex]);
                    }
                    return coords;
                }
                Debug.LogError("Invalid argument! ( RoundResultData.WonLineInfo.GetWonIconsCoord(object) )");
                return new Vector2Int[0];
            }


            public override string ToString()
            {
                return $"lineIndex: {lineIndex}, wonIconId: {wonIconId}, wonIconCount: {WonIconCount}, baseWinScore: {baseWinScore}";
            }


            public class GettingWonIconsCoordRequiredArgs
            {
                public SlotLinesDB slotLinesDB;
            }
        }

        public class ScatteredIconWinSituationInfo : WonSituationInfo
        {

            public ScatteredIconWinSituationInfo[] GetContainedScatteredIconWinSituationInfos(WonSituationInfo[] wonSituationInfos)
            {
                var result = new List<ScatteredIconWinSituationInfo>();

                foreach (var info in wonSituationInfos)
                {
                    if (info is ScatteredIconWinSituationInfo)
                    {
                        result.Add((ScatteredIconWinSituationInfo)info);
                    }
                }
                return result.ToArray();
            }

            // Get the coordinates of won icons
            public override Vector2Int[] GetWonIconsCoord(object args)
            {
                if (args is GettingWonIconsCoordRequiredArgs)
                {
                    int[,] showedIconsId = ((GettingWonIconsCoordRequiredArgs)args).showedIconsId;

                    Vector2Int[] coords = new Vector2Int[WonIconCount];
                    for (int i = 0; i < coords.Length; i++)
                    {

                        int reelIndex = reelsIndexWithWonIconAppeared[i];
                        coords[i] = new Vector2Int(reelIndex, -1); // default value

                        for (int j = 0; j < showedIconsId.GetLength(1); j++)
                        {
                            if (showedIconsId[reelIndex, j] == wonIconId)
                            {
                                coords[i] = new Vector2Int(reelIndex, j);
                                break;
                            }
                        }
                    }
                    return coords;
                }
                Debug.LogError("Invalid argument! ( RoundResultData.ScatteredIconWinSituationInfo.GetWonIconsCoord(object) )");
                return new Vector2Int[0];
            }



            public class GettingWonIconsCoordRequiredArgs
            {
                public int[,] showedIconsId;
            }
        }


        public class BonusGameInfo
        {
            public int totalRoundsCount = 0;
            public int roundNumber = 0;
            public bool isFinalRound = false;

            public bool IsBonusRound => roundNumber > 0 && roundNumber <= totalRoundsCount;

            public override string ToString()
            {
                return $"totalRoundsCount: {totalRoundsCount}, roundNumber: {roundNumber}, isFinalRound: {isFinalRound}";
            }
        }

        public class SwildIconWinSituationInfo
        {
            public int swildIconId;
            public int[] reelsIndexWithWonIconAppeared;

            public int SwildIconCount => reelsIndexWithWonIconAppeared.Length;
            //給位置
            public Vector2Int[] GetTotalWonIconsCoord(int[,] showedIconsId)
            {
                var result = new List<Vector2Int>();

                Vector2Int coord = new Vector2Int();
                //可以優化(利用reelsIndexWithWonIconAppeared)
                for (int i = 0; i < showedIconsId.GetLength(0); i++)
                {
                    int reelIndex = i;
                    for (int j = 0; j < showedIconsId.GetLength(1); j++)
                    {
                        if (showedIconsId[reelIndex, j] == swildIconId)
                        {
                            coord = new Vector2Int(reelIndex, j);
                            result.Add(coord);
                            break;
                        }
                    }

                }

                return result.ToArray();
            }

            public override string ToString()
            {
                return $"swildIconId: {swildIconId}, SwildIconCount: {SwildIconCount}";
            }
        }
    }
}

