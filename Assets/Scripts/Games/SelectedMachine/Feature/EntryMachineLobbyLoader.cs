using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserAccount;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Fetch Machine List When Entry machineLobby
    /// </summary>
    public class EntryMachineLobbyLoader : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Handles the scene loaded event and triggers machine list loading if the scene is "MachineLobby."
        /// </summary>
        void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
        {
            if (loadedScene.name == "MachineLobby")
            {
                StartCoroutine(LoadMachineListCoroutine());
            }
        }

        /// <summary>
        /// Coroutine to delay machine list loading until the user has logged in.
        /// </summary>
        IEnumerator LoadMachineListCoroutine()
        {
            yield return new WaitUntil(() => UserAccountManager.Instance.HaveLogin());
            MachineLobbyMediator.Instance.FetchMachineList();
        }
    }
}