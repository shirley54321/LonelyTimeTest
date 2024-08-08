using System;
using TMPro;
using UnityEngine;

namespace SlotTemplate
{
    public class ED_NextRoundBonusConuts : MonoBehaviour
    {
        [SerializeField] ED_GameStatesManager ED_gameStatesManager;
        
        public TextMeshProUGUI _textMeshPro;

        RoundResultData CurrentData;

        int BonusIconCount;

        public Animator BonusSceneZoom;

        [SerializeField] Animator[] ShakeObject;

        public void CalculateBonusQuantity(RoundResultData roundResultData) 
        {
            CurrentData = roundResultData;
            foreach (var Icon in roundResultData.showedIconsId) 
            {
                if (Icon == 1) 
                {
                    BonusIconCount++;
                    Debug.LogWarning("Bonus Scratch:" + BonusIconCount);
                }
            }
        }


        public void ShowBonusCount()
        {
            if (CurrentData == null) 
            {
                BonusSceneZoom.speed = 0;
                return;
            }

            if (CurrentData.gainedBonusRounds == 0 && _textMeshPro.text != Convert.ToString(BonusIconCount))
            {
                _textMeshPro.text = $"<sprite={Convert.ToString(BonusIconCount)}>";
                BonusSceneZoom.speed = 1;
                BonusSceneZoom.Play(0);
            }
            else if (CurrentData.gainedBonusRounds == 0)
            {
                _textMeshPro.text = $"<sprite={Convert.ToString(BonusIconCount)}>" ;
            }
            else 
            {
                _textMeshPro.text = $"<sprite={Convert.ToString(0)}>";
                BonusSceneZoom.speed = 0;
                BonusIconCount = 0;
            }

        }


        public void OpenShake() 
        {
            foreach (var a in ShakeObject)
            {
                a.speed = 1;
                a.Play(0);
            }
        }

        public void CloseShake() 
        {
            foreach (var a in ShakeObject)
            {
                a.speed = 0;
            }
        }
    }
}