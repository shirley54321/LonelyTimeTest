using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class RedirectManager:MonoBehaviour {
    public string OfficialWebSite; /* = "https://www.lonelytime777.com"; */

    public string FanPage;  /* = "https://www.facebook.com/??????" ; */   //後續自行改網址

    public string OfficialLine; /* https://liff.line.me/??? */   //後續自行改網址

    public string CommentUsAndroid; /* = https://play.google.com/store/apps/details?id=com.facebook.katana */  //後續自行改網址

    public string CommentUsIOS;  /* = https://apps.apple.com/tw/app/facebook/id284882215 */   //後續自行改網址

    public void RedirectToURL( string urlType ) {
        string url = string.Empty;

        switch ( urlType ) {

            case "OfficialWebSite":
                url = OfficialWebSite;
                break;

            case "FanPage":
                url = FanPage;
                break;

            case "OfficialLine":
                url = OfficialLine;
                break;

            case "CommentUs":
                RuntimePlatform platform = Application.platform;  // 檢查當前平台

                if ( platform == RuntimePlatform.Android )
                    url = CommentUsAndroid;

                else if ( platform == RuntimePlatform.IPhonePlayer )
                     url = CommentUsIOS;

                Debug.Log(platform);
                break;

        }
        Debug.Log("Opening URL: " + url) ;  
        Application.OpenURL(url);
    }


   
}
