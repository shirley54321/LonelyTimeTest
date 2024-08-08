using LitJson;
using PlayFab.CloudScriptModels;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Player;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
namespace VIPSetting
{   
    public class VipManager : MonoBehaviour
    {
        private static VipManager instance;
        public static VipManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<VipManager>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject with {typeof(VipManager)} does not exist in the scene, " +
                                       $"yet its method is being called.\n" +
                                       $"Please add {typeof(VipManager)} to the scene.");
                    }
                }

                return instance;
            }
        }

        // Usage:
        // Debug.Log("test VIP RIGHT");
        // VipRight vipRight = new VipRight();

        // await vipRight.UpdateVipRight(string playFabId);

        // Debug.Log("After");
        // vipRight.LogVip();

        public List<VipRight> vipRight = new List<VipRight>();
        public async void Initialization()
        {
            string vipTitleData = await GetTitleDataVipAsync();
            Debug.Log($"vip Title Data: {vipTitleData}");
            vipRight = JsonMapper.ToObject<List<VipRight>>(vipTitleData);
        }

        public void LogVip(VipRight vip)
        {
            Type type = vip.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                object value = property.GetValue(vip, null);
                Debug.Log($"Property Name: {name}, Property Value: {value}");
            }

        }

        public VipRight GetVipRight(int level)
        {
            //LogVip(vipRight[Math.Min(level - 1, vipRight.Count - 1)]);
            return vipRight[Math.Min(level - 1, vipRight.Count - 1)];
        }

        #region Get Title Data (vip) 
        private async Task<GetTitleDataResult> GetTitleDataVip()
        {
            var tcs = new TaskCompletionSource<GetTitleDataResult>();
            string titleDataKey = "vip";

            var request = new GetTitleDataRequest
            {
                Keys = new List<string> { titleDataKey },
            };

            PlayFabClientAPI.GetTitleData(request,
                (result) => tcs.SetResult(result),
                (error) => tcs.SetException(new Exception($"PlayFab Error: {error.ErrorMessage}"))
            );

            //PlayFabClientAPI.GetUserData(request,
            //    (result) => tcs.SetResult(result),
            //    (error) => tcs.SetException(new Exception($"PlayFab Error: {error.ErrorMessage}"))
            //);

            return await tcs.Task;
        }
        private async Task<string> GetTitleDataVipAsync()
        {
            var titleDataResult = await GetTitleDataVip();
            string titleDataKey = "vip";

            if (titleDataResult != null && titleDataResult.Data.TryGetValue(titleDataKey, out var vipTitleData))
            {
                //Debug.Log($"Vip Title Data: {vipTitleData}");
                return vipTitleData;
            }
            else
            {
                Debug.LogWarning("Vip title data not found.");
                return null;
            }

        }
        #endregion

    }
}
