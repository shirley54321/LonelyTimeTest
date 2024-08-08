using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Slot Lines DB", menuName = "ScriptableObject/Slot Lines DB")]
    public class SlotLinesDB : ScriptableObject
    {
        public SlotLine[] lines;

        [System.Serializable]
        public class SlotLine
        {
            public int[] positionOnColumns;
        }
    }
}

