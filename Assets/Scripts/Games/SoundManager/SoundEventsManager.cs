using System;
using UnityEngine;

namespace SlotTemplate
{

    [RequireComponent(typeof(SoundBehavingManager))]
    public class SoundEventsManager : MonoBehaviour, IDBCollectionReceiver
    {

        protected int WildIconId => _gameInfoDB != null ? _gameInfoDB.wildIconId : -1;
        protected int BonusIconId => _gameInfoDB != null ? _gameInfoDB.bonusIconId : -1;


        SoundBehavingManager _soundBehavingManager;
        public SoundBehavingManager soundBehavingManager
        {
            get
            {
                if (_soundBehavingManager == null)
                {
                    _soundBehavingManager = GetComponent<SoundBehavingManager>();
                }
                return _soundBehavingManager;
            }
        }


        [Header("DBs")]
        [SerializeField] protected GameInfoDB _gameInfoDB;
        [SerializeField] protected AudioClipsDB _SpecialIconAnimtingAudioClipsDB;

        [Header("REFS")]
        [SerializeField] protected GameStatesManager _gameStatesManager;
        [SerializeField] protected GUIDisplayManager _guiDisplayManager;
        [SerializeField] protected SlotMainBoard _slotMainBoard;
        [SerializeField] protected SlotMainBoardAnimationManager _slotMainBoardAnimManager;




        public virtual void OnDBsAssignedByDBCollection(GameDBManager.DBCollection dbCollection)
        {
            _gameInfoDB = dbCollection.gameInfoDB ?? _gameInfoDB;
            _SpecialIconAnimtingAudioClipsDB = dbCollection.SpecialIconAnimtingAudioClipsDB ?? _SpecialIconAnimtingAudioClipsDB;

        }
        public virtual void OnDBsAssignedByDBCollectionIncludeEmpty(GameDBManager.DBCollection dbCollection)
        {
            _gameInfoDB = dbCollection.gameInfoDB;
            _SpecialIconAnimtingAudioClipsDB = dbCollection.SpecialIconAnimtingAudioClipsDB;
        }



        protected virtual void Awake()
        {
            _soundBehavingManager = GetComponent<SoundBehavingManager>();
        }

        protected virtual void OnEnable()
        {
            if (_gameStatesManager != null)
            {
                _gameStatesManager.BigWinAnimationStarted += OnBigWinAnimationStart;
                _gameStatesManager.BigWinAnimationEnded += OnBigWinAnimationEnded;
                _gameStatesManager.BonusGameTransitionInStarted += OnBonusGameTransitionInStart;
                _gameStatesManager.BonusGameTransitionOutStarted += OnBonusGameTransitionOutStart;
                //新增功能?
                _gameStatesManager.EnteringBonusGameStart += OnEnteringBonusGameStart;
                _gameStatesManager.EnteringBonusGameEnd += OnEnteringBonusGameEnd;
                _gameStatesManager.BonusGameTotalWinsDisplay += OnBonusGameTotalWinsDisplay;


            }

            if (_guiDisplayManager != null)
            {
                _guiDisplayManager.WinScoreRaisingEffectStarted += OnScoreRaisingStarted;
                _guiDisplayManager.WinScoreRaisingEffectEnded += OnScoreRaisingEnded;
            }

            if (_slotMainBoard != null)
            {
                _slotMainBoard.SpinningStarted += OnSlotMainBoardSpinningStart;
                _slotMainBoard.ReelStoppingStarted += OnReelStoppingStart;
                _slotMainBoard.WaitingForBonusIconEffectsPlayed += OnWaitingForBonusIconEffectsPlay;
                _slotMainBoard.ReelsStopped += OnWaitingForBonusIconEffectsEnded;
                _slotMainBoard.BonusGameTriggered += OnBonusGameTriggered;
                _slotMainBoard.TotalWonAnimtaionStarted += OnSlotMainBoardTotalWonAnimationStart;
            }

            if (_slotMainBoardAnimManager != null)
            {
                _slotMainBoardAnimManager.BonusGameMultiplierAnimatingStarted += OnBonusGameMultiplierAnimatingStart;
            }
        }

        protected virtual void OnDisable()
        {
            if (_gameStatesManager != null)
            {
                _gameStatesManager.BigWinAnimationStarted -= OnBigWinAnimationStart;
                _gameStatesManager.BigWinAnimationEnded -= OnBigWinAnimationEnded;
                _gameStatesManager.BonusGameTransitionInStarted -= OnBonusGameTransitionInStart;
                _gameStatesManager.BonusGameTransitionOutStarted -= OnBonusGameTransitionOutStart;
                //新增功能?
                _gameStatesManager.EnteringBonusGameStart -= OnEnteringBonusGameStart;
                _gameStatesManager.EnteringBonusGameEnd -= OnEnteringBonusGameEnd;
                _gameStatesManager.BonusGameTotalWinsDisplay -= OnBonusGameTotalWinsDisplay;

            }

            if (_guiDisplayManager != null)
            {
                _guiDisplayManager.WinScoreRaisingEffectStarted -= OnScoreRaisingStarted;
                _guiDisplayManager.WinScoreRaisingEffectEnded -= OnScoreRaisingEnded;
            }

            if (_slotMainBoard != null)
            {
                _slotMainBoard.SpinningStarted -= OnSlotMainBoardSpinningStart;
                _slotMainBoard.ReelStoppingStarted -= OnReelStoppingStart;
                _slotMainBoard.WaitingForBonusIconEffectsPlayed -= OnWaitingForBonusIconEffectsPlay;
                _slotMainBoard.ReelsStopped -= OnWaitingForBonusIconEffectsEnded;
                _slotMainBoard.BonusGameTriggered -= OnBonusGameTriggered;
                _slotMainBoard.TotalWonAnimtaionStarted -= OnSlotMainBoardTotalWonAnimationStart;
            }

            if (_slotMainBoardAnimManager != null)
            {
                _slotMainBoardAnimManager.BonusGameMultiplierAnimatingStarted -= OnBonusGameMultiplierAnimatingStart;
            }
        }



        // Musics
        protected virtual void OnSlotMainBoardSpinningStart(object sender, SlotMainBoard.SpinningStartEventArgs args)
        {
            if (!args.isBonusGame)
            {
                soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.Rolling);
            }
        }

        protected virtual void OnBonusGameTransitionInStart(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BonusGamePlaying);
        }

        protected virtual void OnBonusGameTransitionOutStart(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BonusGamePlaying, SoundBehavingManager.EventType.Stopping);
        }

        protected virtual void OnBigWinAnimationStart(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BigWinAnimating);
        }

        protected virtual void OnBigWinAnimationEnded(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BigWinAnimating, SoundBehavingManager.EventType.Stopping);
        }


        // SFX
        protected virtual void OnReelStoppingStart(object sender, ReelController.StoppingStartedEventArgs args)
        {
            if (!args.isBonusIconAppeared)
            {
                soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.ReelStopping, SoundBehavingManager.EventType.Starting, args.reelIndex);
            }
            else
            {
                soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.ReelStoppingWithBonusIcon, SoundBehavingManager.EventType.Starting, args.reelIndex);
            }
        }

        protected virtual void OnScoreRaisingStarted(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.ScoreRaising);
        }

        protected virtual void OnScoreRaisingEnded(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.ScoreRaising, SoundBehavingManager.EventType.Stopping);
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.ScoreRaisingEnded);
        }

        protected virtual void OnSlotMainBoardTotalWonAnimationStart(object sender, SlotMainBoard.TotalWonAnimationStartedEventArgs args)
        {


            for (int i = 0; i < _SpecialIconAnimtingAudioClipsDB.items.Length; i++)
            {
                if (Array.Exists(args.containedAnimatedIconsId, id => id == _SpecialIconAnimtingAudioClipsDB.items[i].id))
                {
                    soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.SpecialIconAnimatinginTotalWinAnimation, SoundBehavingManager.EventType.Starting, _SpecialIconAnimtingAudioClipsDB.items[i].id);
                }
            }

            if (Array.Exists(args.containedAnimatedIconsId, id => id == BonusIconId))
            {
                soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BonusIconAnimatingInTotalWinAnimation);
            }
        }

        protected virtual void OnWaitingForBonusIconEffectsPlay(object sender, ReelController.WaitingForBonusIconEffectPlayedEventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.WaitingForBonusIconEffect);
        }

        protected virtual void OnWaitingForBonusIconEffectsEnded(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.WaitingForBonusIconEffect, SoundBehavingManager.EventType.Stopping);
        }

        protected virtual void OnBonusGameTriggered(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BonusGameTriggered);
        }

        // Not used yet
        protected virtual void OnEnteringBonusGameStart(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.EnteringBonusGameStart);
        }

        // Not used yet
        protected virtual void OnEnteringBonusGameEnd(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.EnteringBonusGameEnd);
        }

        protected virtual void OnBonusGameMultiplierAnimatingStart(object sender, EventArgs args)
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BonusGameMultiplierAnimating);
        }

        protected virtual void OnBonusGameTotalWinsDisplay(object sender, EventArgs args) 
        {
            soundBehavingManager.TriggerEvent(SoundBehavingManager.Event.BonusGameTotalWinsDisplay);
        }

    }
}
