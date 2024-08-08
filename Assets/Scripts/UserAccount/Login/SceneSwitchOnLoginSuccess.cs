using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Loading;
namespace UserAccount
{
    /// <summary>
    /// Switches scenes upon successful user login and subscribes to the OnLoginSuccessful event.
    /// </summary>
    public class SceneSwitchOnLoginSuccess : MonoBehaviour
    {
        private LoadingScreen Loading_Screen;
        private void Awake()
        {
            // Subscribe to the OnLoginSuccessful event
            UserAccountManager.OnLoginSuccessful.AddListener(LoadScene);
        }
        // private void Start() {
        //     Loading_Screen =FindObjectOfType<LoadingScreen>();
        // }

        /// <summary>
        /// Loads the desired scene upon successful user login.
        /// </summary>
        /// <param name="result">The login result containing relevant information.</param>
        private void LoadScene(LoginResult result)
        {
            // Log a message indicating that the scene is being loaded
            Debug.Log("Load Scene");
            // TODO: Add logic to switch scenes based on the successful login result if needed
            LoadingManger.instance.SetLoadingNormolScreen("GameLobby");
        }
    }
}