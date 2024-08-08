using UnityEngine;

namespace Main.PlayerPage
{
    public class OtherPlayerInfoPageTester : MonoBehaviour
    {
        public string playFabId;

        [ContextMenu("OpenPanel")]
        public void OpenPanel()
        {
            OtherPlayerInfoManager.Instance.OpenOtherPlayerPanel(playFabId);
        }
    }
}