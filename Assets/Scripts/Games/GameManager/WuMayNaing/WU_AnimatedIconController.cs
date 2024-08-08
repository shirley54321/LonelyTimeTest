using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SlotTemplate
{
    public class WU_AnimatedIconController : AnimatedIconsController
    {
        bool[] hasWild = new bool[5] { false, false, false, false, false };
        [SerializeField] GameObject[] wildExpansion = new GameObject[5];
        public override void PrepareAnimatedIcons(int[,] showedIconsId, Vector2Int[] coords)
        {
            hasWild = new bool[5] { false, false, false, false, false };
            for (int i = 0; i < coords.Length; i++)
            {
                if (showedIconsId[coords[i].x, coords[i].y] != 1 && (showedIconsId[coords[i].x, 0] == 0 || showedIconsId[coords[i].x, 1] == 0 || showedIconsId[coords[i].x, 2] == 0 || showedIconsId[coords[i].x, 3] == 0) )
                {
                    hasWild[coords[i].x] = true;
                }
            }
            for (int i = 0; i < coords.Length; i++)
            {
                int iconId = showedIconsId[coords[i].x, coords[i].y];
                Vector2 position = reelsContainerInfoManager.GetReelIconPosition(coords[i]);

                animatedIcons.Add(coords[i], GenerateAnimatedIcon(position, iconId, -spawnedZInterval * i));
            }

            IsPrepared = true;
        }

        public override void DisplayAnimatedIcons(Vector2Int[] coords)
        {
            foreach (var coordAnimatedIconPair in animatedIcons)
            {
                coordAnimatedIconPair.Value.SetActive(Array.Exists(coords, coord => coord == coordAnimatedIconPair.Key));
            }
            for (int i = 0; i < hasWild.Length; i++)
            {
                if (hasWild[i]) wildExpansion[i].SetActive(true);
            }
        }
        public override void ClearAnimatedIcons()
        {
            IsPrepared = false;

            animatedIcons.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < hasWild.Length; i++)
            {
                if (hasWild[i]) wildExpansion[i].SetActive(false);
            }
        }
    }
}
