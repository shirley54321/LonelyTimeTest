using UnityEngine;

namespace SlotTemplate {

    public class ReelInfoManager : MonoBehaviour {

        public Vector3 GetIconPosition (int iconIndex) {
            if (iconIndex < transform.childCount) {
                return transform.GetChild(iconIndex).position;
            }
            return Vector3.zero;
        }

    }
}
