using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SlotTemplate
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameManager))]
    public class GameDBManager : MonoBehaviour
    {

        public DBCollection dbCollection;


        [Serializable]
        public class DBCollection
        {
            public GameInfoDB gameInfoDB;

            public PayTableDB payTableDB;

            public SpritesDB backgroundSpritesDB;

            public SpritesDB iconsSpriteDB;
            public AnimationClipsDB iconsAnimClipDB;
            public SlotLinesDB slotLinesDB;
            public FakeReelIconIdsTableDB fakeReelIconIdsTableDB;

            public BigWinAnimationClipsDB bigWinsAnimationClipsDB;
            public BigWinWinRateStepsDB bigWinWinRateStepsDB;
            public BigWinScoreRaisingDurationsDB bigWinScoreRaisingDurationsDB;

            public BonusGameInfoDB bonusGameInfoDB;
            public AnimationClipsDB bonusGameMultipliersAnimationClipDB;

            public AudioClipsDB SpecialIconAnimtingAudioClipsDB;
        }

    }
}

