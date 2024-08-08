using UnityEngine;

namespace Majaja.Utilities {

    public static class UnityTransformExtensions {

        public static void SetPositionX (this Transform transform, float x)  {
            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }
        public static void SetPositionY (this Transform transform, float y)  {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }
        public static void SetPositionZ (this Transform transform, float z)  {
            Vector3 pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }
        public static void SetLocalPositionX (this Transform transform, float x)  {
            Vector3 localPos = transform.localPosition;
            localPos.x = x;
            transform.localPosition = localPos;
        }
        public static void SetLocalPositionY (this Transform transform, float y)  {
            Vector3 localPos = transform.localPosition;
            localPos.y = y;
            transform.localPosition = localPos;
        }
        public static void SetLocalPositionZ (this Transform transform, float z)  {
            Vector3 localPos = transform.localPosition;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        public static void DestroyAllChildren (this Transform transform) {
            for (int i = transform.childCount - 1 ; i >= 0 ; i--) {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyImmediateAllChildren (this Transform transform) {
            for (int i = transform.childCount - 1 ; i >= 0 ; i--) {
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }

}
