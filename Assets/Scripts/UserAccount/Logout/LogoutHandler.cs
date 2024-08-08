using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace UserAccount
{
    public class LogoutHandler : MonoBehaviour
    {
        
        [ContextMenu("Log out")]
        public void Logout()
        {
            PlayFabClientAPI.ForgetAllCredentials();
            UserAccountManager.Instance.ClearQuickLoginId();
            UIManager.Instance.ClearDictionary();
            Debug.Log("Player has log out");
            // TODO: Add switch scenes based on SceneLoader
            SceneManager.LoadScene("Login");
        }
        public void RestLobby()
        {
            UserAccountManager.Instance.InvokeLoginSuccess();
            SceneManager.LoadScene("GameLobby");

            //if(DownloadManager.CanSwitchScene()){
            //    UserAccountManager.Instance.InvokeLoginSuccess();
            //    SceneManager.LoadScene("GameLobby");
            //}
            //else{
            //    Debug.Log("有東西正在下載");
            //}
        }

        public void BackToSelectMachineHall()
        {
            UserAccountManager.Instance.InvokeLoginSuccess();
            SceneManager.LoadScene("MachineLobby");
        }
    }
}