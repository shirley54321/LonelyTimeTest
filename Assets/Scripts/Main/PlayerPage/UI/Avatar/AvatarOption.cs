using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.Avatar
{
    /// <summary>
    /// Avatar Select Option for PickAvatarPanel
    /// </summary>
    public class AvatarOption : MonoBehaviour
    {
        [SerializeField] private Image avatarImage;
        public Player.Avatar avatar;
        public Toggle toggle;
    
        private PickAvatarPanel _pickAvatarPanel;
        [SerializeField] private GameObject usingAvatarBar, notUsingMask;

        #region Set up

        public void Init(PickAvatarPanel pickAvatarPanel, Player.Avatar avatar)
        {
            _pickAvatarPanel = pickAvatarPanel;
            this.avatar = avatar;
            avatarImage.sprite = avatar.Sprite;

            toggle.group = pickAvatarPanel.ToggleGroup;
            usingAvatarBar.SetActive(false);
            notUsingMask.SetActive(true);
        }

        public void SetIsUsing()
        {
            usingAvatarBar.SetActive(true);
            ChangeSelected();
        }

        #endregion

        #region Change Selected 

        public void ChangeSelected()
        {
            toggle.SetIsOnWithoutNotify(true);
            _pickAvatarPanel.SetSelectedAvatar(this);
            notUsingMask.SetActive(false);
        }

        public void OnChangedSelected(bool ison)
        {
            notUsingMask.SetActive(!ison);
            if (ison)
            {
                _pickAvatarPanel.SetSelectedAvatar(this);
            }
        }

        #endregion
    }
}

