using UnityEngine;

namespace SlotTemplate {

    public class ReelsContainerInfoManager : MonoBehaviour {

        public Vector3 GetReelPosition (int reelIndex) {
            if (reelIndex < transform.childCount) {
                return transform.GetChild(reelIndex).position;
            }
            return Vector3.zero;
        }

        public Vector3 GetReelIconPosition (Vector2Int reelIconCoord) {
            if (reelIconCoord.x < transform.childCount) {
                return transform.GetChild(reelIconCoord.x).gameObject.GetComponent<ReelInfoManager>().GetIconPosition(reelIconCoord.y);
            }
            return Vector3.zero;
        }

        public Vector3 GetLeftEdge (int positionIndexOnColumn) {
            Vector3 iconPosAtLeftReel = GetReelIconPosition(new Vector2Int(0, positionIndexOnColumn));
            Vector3 iconPosAt2ndLeftReel = GetReelIconPosition(new Vector2Int(1, positionIndexOnColumn));

            return iconPosAtLeftReel + (iconPosAtLeftReel - iconPosAt2ndLeftReel) / 2;
        }

        public Vector3 GetRightEdge (int positionIndexOnColumn) {
            Vector3 iconPosAtRightReel = GetReelIconPosition(new Vector2Int(transform.childCount - 1, positionIndexOnColumn));
            Vector3 iconPosAt2ndRightReel = GetReelIconPosition(new Vector2Int(transform.childCount - 2, positionIndexOnColumn));

            return iconPosAtRightReel + (iconPosAtRightReel - iconPosAt2ndRightReel) / 2;
        }

    }
}
