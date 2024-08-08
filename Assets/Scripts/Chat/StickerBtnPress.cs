using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StickerBtnPress : MonoBehaviour
{
    [SerializeField] private PublicChatCommunication publicChatCommunication;
    [SerializeField] private GameObject scrollContainer;
    [SerializeField] private GameObject peopleSticker;
    [SerializeField] private GameObject mySticker;
    [SerializeField] private Sprite sticker0101;
    [SerializeField] private Sprite sticker0102;
    [SerializeField] private Sprite sticker0103;
    [SerializeField] private Sprite sticker0104;
    [SerializeField] private Sprite sticker0105;
    [SerializeField] private Sprite sticker0106;
    [SerializeField] private Sprite sticker0107;
    [SerializeField] private Sprite sticker0108;
    [SerializeField] private Sprite sticker0201;
    [SerializeField] private Sprite sticker0202;
    [SerializeField] private Sprite sticker0203;
    [SerializeField] private Sprite sticker0204;
    [SerializeField] private Sprite sticker0205;
    [SerializeField] private Sprite sticker0206;
    [SerializeField] private Sprite sticker0207;
    [SerializeField] private Sprite sticker0208;

    public void Sticker(string _Sticker, List<string> strings, Sprite avatarSprite, bool function)
    {
        Sprite stickerSprite = null;
        switch (_Sticker)
        {
            case "<0101>":
                stickerSprite = sticker0101;
                break;
            case "<0102>":
                stickerSprite = sticker0102;
                break;
            case "<0103>":
                stickerSprite = sticker0103;
                break;
            case "<0104>":
                stickerSprite = sticker0104;
                break;
            case "<0105>":
                stickerSprite = sticker0105;
                break;
            case "<0106>":
                stickerSprite = sticker0106;
                break;
            case "<0107>":
                stickerSprite = sticker0107;
                break;
            case "<0108>":
                stickerSprite = sticker0108;
                break;
            case "<0201>":
                stickerSprite = sticker0201;
                break;
            case "<0202>":
                stickerSprite = sticker0202;
                break;
            case "<0203>":
                stickerSprite = sticker0203;
                break;
            case "<0204>":
                stickerSprite = sticker0204;
                break;
            case "<0205>":
                stickerSprite = sticker0205;
                break;
            case "<0206>":
                stickerSprite = sticker0206;
                break;
            case "<0207>":
                stickerSprite = sticker0207;
                break;
            case "<0208>":
                stickerSprite = sticker0208;
                break;
        }
        if(stickerSprite!=null)
        {
            if (function)
            {
                displayMySticker(stickerSprite);
            }
            else
            {
                displayPeopleSticker(stickerSprite, strings, avatarSprite);
            }
        }
        
        //function? displayMySticker(stickerSprite) : displayPeopleSticker(stickerSprite, strings, avatarSprite);


    }
    private void displayMySticker(Sprite stickerSprite)
        {
            //modify sticker
            var objList = mySticker.GetComponentsInChildren<Image>(); //�j���]�t�o�Ӫ�component�A�]�t�ۤv�A�Ѧ�https://blog.csdn.net/virus2014/article/details/52964159
            for (int i = 0; i < 2; i++)
            {
                print(objList[i]);
            }
            objList[0].sprite = stickerSprite;

            //modify time
            var objList2 = mySticker.GetComponentsInChildren<TMP_Text>();
            objList2[0].text = $"{DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}";

            //add sticker to container
            Instantiate(mySticker, scrollContainer.transform); //�ͦ��@��sticker��prefab
        }
    private void displayPeopleSticker(Sprite stickerSprite, List<string> strings, Sprite avatarSprite)
        {
            

            //modify sticker
             var objList = peopleSticker.GetComponentsInChildren<Image>(); //�j���]�t�o�Ӫ�component�A�]�t�ۤv�A�Ѧ�https://blog.csdn.net/virus2014/article/details/52964159
            for (int i = 0; i < 3; i++)
            {
                print(objList[i]);
            }
            objList[0].sprite = avatarSprite;
            objList[2].sprite = stickerSprite;

            //modify time and people name
            var objList2 = peopleSticker.GetComponentsInChildren<TMP_Text>();
            for (int i = 0; i < 2; i++)
            {
                print(objList2[i]);
                objList2[i].text = strings[i];
            }

            //add sticker to container
            Instantiate(peopleSticker, scrollContainer.transform); //�ͦ��@��sticker��prefab
        }


        #region
        //public void Sticker0101()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0101>");
        //    displayMySticker(sticker0101);
        //}
        //public void Sticker0102()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0102>");
        //    displayMySticker(sticker0102);
        //}
        //public void Sticker0103()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0103>");
        //    displayMySticker(sticker0103);
        //}
        //public void Sticker0104()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0104>");
        //    displayMySticker(sticker0104);
        //}
        //public void Sticker0105()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0105>");
        //    displayMySticker(sticker0105);
        //}
        //public void Sticker0106()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0106>");
        //    displayMySticker(sticker0106);
        //}
        //public void Sticker0107()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0107>");
        //    displayMySticker(sticker0107);
        //}
        //public void Sticker0108()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0108>");
        //    displayMySticker(sticker0108);
        //}

        //public void Sticker0201()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0201>");
        //    displayMySticker(sticker0201);
        //}
        //public void Sticker0202()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0202>");
        //    displayMySticker(sticker0202);
        //}
        //public void Sticker0203()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0203>");
        //    displayMySticker(sticker0203);
        //}
        //public void Sticker0204()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0204>");
        //    displayMySticker(sticker0204);
        //}
        //public void Sticker0205()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0205>");
        //    displayMySticker(sticker0205);
        //}
        //public void Sticker0206()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0206>");
        //    displayMySticker(sticker0206);
        //}
        //public void Sticker0207()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0207>");
        //    displayMySticker(sticker0207);
        //}
        //public void Sticker0208()
        //{
        //    publicChatCommunication.SendPublicChatMessage("<0208>");
        //    displayMySticker(sticker0208);
        //}
        #endregion

        #region 
        //public void PeopleSticker0101(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0101, strings, avatarSprite);
        //}
        //public void PeopleSticker0102(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0102, strings, avatarSprite);
        //}
        //public void PeopleSticker0103(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0103, strings, avatarSprite);
        //}
        //public void PeopleSticker0104(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0104, strings, avatarSprite);
        //}
        //public void PeopleSticker0105(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0105, strings, avatarSprite);
        //}
        //public void PeopleSticker0106(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0106, strings, avatarSprite);
        //}
        //public void PeopleSticker0107(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0107, strings, avatarSprite);
        //}
        //public void PeopleSticker0108(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0108, strings, avatarSprite);
        //}
        //public void PeopleSticker0201(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0201, strings, avatarSprite);
        //}
        //public void PeopleSticker0202(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0202, strings, avatarSprite);
        //}
        //public void PeopleSticker0203(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0203, strings, avatarSprite);
        //}
        //public void PeopleSticker0204(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0204, strings, avatarSprite);
        //}
        //public void PeopleSticker0205(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0205, strings, avatarSprite);
        //}
        //public void PeopleSticker0206(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0206, strings, avatarSprite);
        //}
        //public void PeopleSticker0207(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0207, strings, avatarSprite);
        //}
        //public void PeopleSticker0208(List<string> strings, Sprite avatarSprite)
        //{
        //    displayPeopleSticker(sticker0208, strings, avatarSprite);
        //}
        #endregion
    }
