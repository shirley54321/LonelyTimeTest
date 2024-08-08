using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlotTemplateK2B.DebugTools {

    public class NextSceneLoader : MonoBehaviour {

        [SerializeField] int _nextSceneBuildIndex;

        void OnEnable () {
            SceneManager.LoadScene(_nextSceneBuildIndex);
        }
    }
}
