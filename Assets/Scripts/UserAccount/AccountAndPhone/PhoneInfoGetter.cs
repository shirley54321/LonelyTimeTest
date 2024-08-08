using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace UserAccount.AccountAndPhone
{
    [Serializable]
    public class PhoneInfoGetter
    {
        [SerializeField] private PhoneInfo phoneInfo;

        /// <summary>
        /// Warning : Only can call after GetPhoneInfoCoroutine() complete
        /// </summary>
        /// <returns></returns>
        public bool HaveLinkPhone()
        {
            return phoneInfo.haveLinkPhone;
        }
        
        /// <summary>
        /// Warning : Only can call after GetPhoneInfoCoroutine() complete
        /// </summary>
        /// <returns></returns>
        public string GetPhoneNumber()
        {
            return phoneInfo.phoneNumber;
        }

        public PhoneInfo GetPhoneInfo()
        {
            return phoneInfo;
        }

        /// <summary>
        /// Coroutine to check whether the user has linked their phone.
        /// </summary>
        public IEnumerator GetPhoneInfoCoroutine()
        {
            bool isResponseReceived = false;
            phoneInfo = new PhoneInfo();

            var request = new ExecuteFunctionRequest()
            {
                FunctionName = "TakePlayerInfomation",
                FunctionParameter = new { PLayerInfo = new List<string>{ "Phone_Number" } }
            
            };
            PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
            {
                string phone = result.FunctionResult.ToString().Trim(new char[] { '"','[',']'});
                if(phone =="")
                {
                    phoneInfo.haveLinkPhone = false;// Placeholder condition, modify accordingly
                }
                else
                {
                    phoneInfo.phoneNumber = phone;
                    phoneInfo.haveLinkPhone = true;
                }
                isResponseReceived = true;
            }, error =>
            {
                Debug.Log(error.ErrorMessage);
                isResponseReceived = true;
            });
             
            //isResponseReceived = true;

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }
    }
}