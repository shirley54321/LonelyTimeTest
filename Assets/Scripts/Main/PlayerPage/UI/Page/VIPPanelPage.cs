using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.Page
{
    public class VIPPanelPage : MonoBehaviour
    {
        
        [SerializeField] private GameObject vipInstructionPanel;
        [SerializeField] private GameObject privilegePanel;
        [SerializeField] private Image userVIPCard;
        [SerializeField] private Slider vipExpBar;
        [SerializeField] private Text nextLevelRequirement;
        //[SerializeField] private Text vipTimeLeft;

        [SerializeField] private GameObject nowLevelInfo;
        [SerializeField] private GameObject nextLevelInfo;
        [SerializeField] private PlayerInfoManager playerInfo;

        [SerializeField] private Sprite[] VipIconCandidates;
        [SerializeField] private VipIconData _vipIconData;

        //[SerializeField]
        private System.UInt64 getVIPLevel;
        //[SerializeField]
        private int accumulateTopUp;

        private string playFabId;

        private string[] levelThreshold = { "創角", "綁定門號", "首次儲值", "1,000", "3,000", "7,000", "15,000", "30,000", "50,000" };
        private string[] giftValue = { "-", "-", "10萬", "50萬", "200萬", "1000萬", "1200萬", "1500萬", "2000萬" };
        private string[] exp = { "-", "-", "1", "1", "2", "2", "4", "8", "10" };

        // Start is called before the first frame update
        public IEnumerator Start()
        {
            GetPlayerProfileRequest request = new GetPlayerProfileRequest();

            yield return StartCoroutine(GetPlayerProfileCoroutine(request));

            yield return StartCoroutine(ShowPanelCoroutine());
        }

        // �w�q ShowPanelCoroutine ��{
        public IEnumerator ShowPanelCoroutine()
        {
            showPanel(); // �ե� showPanel ���
            yield return null; // ���ݤ@�V�H�T�O showPanel ���槹��
        }

        // Update is called once per frame
        void Update()
        {

        }

        public async void showPanel()
        {

            gameObject.SetActive(true);
            vipInstructionPanel.SetActive(true);
            privilegePanel.SetActive(false);

            Debug.Log($"Read PlayFabId: {playFabId}");
            await UpdateVIP(playFabId);
            await GetAccumulateTopUp(playFabId);

            Debug.Log($"VIP level: {getVIPLevel}");
            Debug.Log($"accumulate Top Up: {accumulateTopUp}");

            UpdateVIPCard();
            UpdateNowLevelInfo();
            UpdateNextLevelInfo();
            UpdateNextLevelRequirement();
            UpdateExpBar();

        }


        #region Update VIP Instruction Information
        private async Task<object> CallCloudScriptFunctionAsync_UpdateVIP(string playFabId)
        {
            Debug.Log("Async - Call cloud script UpdateVIP in VIP Instruction");

            var tcs = new TaskCompletionSource<object>();

            ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
            {
                FunctionName = "UpdateVIP",
                FunctionParameter = new { playFabId = playFabId },
                GeneratePlayStreamEvent = true
            };

            PlayFabCloudScriptAPI.ExecuteFunction(cloudFunction,
            (result) =>
            {
                if (result != null && result.FunctionResult != null)
                {
                    tcs.SetResult(result.FunctionResult);
                }
                else
                {
                    tcs.SetException(new Exception("CloudScript function did not return a valid result."));
                }
            },
            (error) =>
            {
                tcs.SetException(new Exception("CloudScript function failed with error: " + error.GenerateErrorReport()));
            },
            null);

            return await tcs.Task;
        }

        private async Task UpdateVIP(string playFabId)
        {
            Debug.Log("Update VIP in VIP Instruction");
            try
            {
                object cloudScriptResult = await CallCloudScriptFunctionAsync_UpdateVIP(playFabId);

                Debug.Log("VIP Instruction CloudScript result: " + cloudScriptResult);
                getVIPLevel = (System.UInt64)cloudScriptResult;

                Debug.Log($"VIP Instruction level Get: {getVIPLevel}");
            }

            catch (Exception ex)
            {
                Debug.Log("VIP Instructon Error: " + ex.Message);
            }


        }

        private async Task<GetUserDataResult> GetAccumulateTopUpAsync(string playFabId)
        {
            var tcs = new TaskCompletionSource<GetUserDataResult>();
            string userDataKey = "AccumulateTopUp";

            Debug.Log($"Get user playfabid: {playFabId}");
            var request = new GetUserDataRequest
            {
                PlayFabId = playFabId,
                Keys = new List<string> { userDataKey },
            };

            PlayFabClientAPI.GetUserReadOnlyData(request,
                (result) => tcs.SetResult(result),
                (error) => tcs.SetException(new Exception($"PlayFab Error: {error.ErrorMessage}"))
            );

            //PlayFabClientAPI.GetUserData(request,
            //    (result) => tcs.SetResult(result),
            //    (error) => tcs.SetException(new Exception($"PlayFab Error: {error.ErrorMessage}"))
            //);

            return await tcs.Task;
        }

        private async Task GetAccumulateTopUp(string playFabId)
        {
            var userDataResult = await GetAccumulateTopUpAsync(playFabId);

            if (userDataResult != null && userDataResult.Data.TryGetValue("AccumulateTopUp", out var userData)) 
            {   
                accumulateTopUp = int.Parse(userData.Value);
                Debug.Log($"User Data: {userData.Value}");
            }
            else
            {   
                Debug.LogWarning("User data not found.");
            }

        }

        private void UpdateVIPCard()
        {
            //Debug.Log("Update VIP Card");
            // userVIPCard.sprite = VipIconCandidates[Math.Min(getVIPLevel - 1, 8)];
           
            userVIPCard.sprite = _vipIconData.GetIcon((int)Math.Min(getVIPLevel, 9));

        }

        private void UpdateNowLevelInfo()
        {
            //Debug.Log("Update Now level Information");
            string[] childrenName = { "Cumulative_Stored_Value", "Gift_Value", "Exp", "Jackpot", "Take_Gift", "NowLevel"};


            for (int i = 0; i < childrenName.Length; i++)
            {
                string childName = childrenName[i];
                Transform childTrans = nowLevelInfo.transform.Find(childName);

                if (childName == "Cumulative_Stored_Value")
                {
                    Transform valObject = childTrans.transform.Find("Value");

                    Text textComponent = valObject.GetComponent<Text>();
                    textComponent.text = accumulateTopUp.ToString();
                    if (getVIPLevel <= 9)
                    {
                        textComponent.text = levelThreshold[getVIPLevel - 1].ToString();
                    }
                    else
                    {
                        int val = 50000 + 20000 * ((int)getVIPLevel - 9);
                        string valString = FormatNumberWithCommas(val);
                        textComponent.text = valString;
                    }

                    //Debug.Log($"Child [{childName}]: {textComponent.text}");
                }
                else if (childName == "Gift_Value")
                {
                    Transform valObject = childTrans.transform.Find("Value");
                    Text textComponent = valObject.GetComponent<Text>();

                    int idx = Math.Min((int)getVIPLevel - 1, giftValue.Length - 1);
                    textComponent.text = giftValue[idx];

                    //Debug.Log($"Child [{childName}]: {textComponent.text}");
                }
                else if (childName == "Exp")
                {
                    Transform valObject = childTrans.transform.Find("Value");
                    Text textComponent = valObject.GetComponent<Text>();

                    int idx = Math.Min((int)getVIPLevel - 1, exp.Length - 1);
                    textComponent.text = exp[idx];

                    //Debug.Log($"Child [{childName}]: {textComponent.text}");
                }
                else if (childName == "Take_Gift")
                {
                    Transform valObject = childTrans.transform.Find("YesOrNo");
                    Text textComponent = valObject.GetComponent<Text>();

                    if (getVIPLevel > 1)
                    {
                        textComponent.text = "YES";
                    }
                    else
                    {
                        textComponent.text = "NO";
                    }

                    //Debug.Log($"Child [{childName}]: {textComponent.text}");
                }
                else if (childName == "NowLevel")
                {
                    Transform vipCard = childTrans.transform.Find("VIPcard");
                    Image imageComponent = vipCard.GetComponent<Image>();

                    imageComponent.sprite = _vipIconData.GetIcon((int)Math.Min(getVIPLevel, 9));//VipIconCandidates[Math.Min(getVIPLevel, 8)];
                }

                //Debug.Log($"child name: {childName}, next level information: {textComponent.text}")
            }

            
        }

        private void UpdateNextLevelInfo()
        {
            //Debug.Log("Update Next level Infomation");
            string[] childrenName = { "Cumulative_Stored_Value", "Gift_Value", "Exp", "Jackpot", "Take_Gift", "NextLevel"};


            for (int i = 0; i < childrenName.Length; i++)
            {
                string childName = childrenName[i];
                Transform childTrans = nextLevelInfo.transform.Find(childName);

                if (childName == "Cumulative_Stored_Value")
                {
                    Transform valObject = childTrans.transform.Find("Value");

                    Text textComponent = valObject.GetComponent<Text>();
                    textComponent.text = accumulateTopUp.ToString();
                    if (getVIPLevel < 9)
                    {
                        textComponent.text = levelThreshold[getVIPLevel].ToString();
                    }
                    else
                    {
                        int val = 50000 + 20000 * ((int)getVIPLevel - 8);
                        string valString = FormatNumberWithCommas(val);
                        textComponent.text = valString;
                    }
                }
                else if (childName == "Gift_Value")
                {
                    Transform valObject = childTrans.transform.Find("Value");
                    Text textComponent = valObject.GetComponent<Text>();

                    int idx = Math.Min((int)getVIPLevel, giftValue.Length - 1);
                    textComponent.text = giftValue[idx];
                }
                else if (childName == "Exp")
                {
                    Transform valObject = childTrans.transform.Find("Value");
                    Text textComponent = valObject.GetComponent<Text>();

                    int idx = Math.Min((int)getVIPLevel, exp.Length - 1);
                    textComponent.text = exp[idx];
                }
                else if (childName == "Take_Gift")
                {
                    Transform valObject = childTrans.transform.Find("YesOrNo");
                    Text textComponent = valObject.GetComponent<Text>();


                    textComponent.text = "YES";

                }
                else if (childName == "NextLevel")
                {
                    Transform vipCard = childTrans.transform.Find("VIPcard");
                    Image imageComponent = vipCard.GetComponent<Image>();

                    imageComponent.sprite = _vipIconData.GetIcon((int)Math.Min(getVIPLevel + 1, 9));//VipIconCandidates[Math.Min(getVIPLevel, 8)];
                }

            }
        }

        private void UpdateNextLevelRequirement()
        {
            //Debug.Log("Update Next level Requirement");
            
            Text textComponent = nextLevelRequirement.GetComponent<Text>();

            if (textComponent!= null)
            {
                //Text textComponent = nextLevelRequirement.GetComponent<Text>();

                if (getVIPLevel == 1)
                {
                    textComponent.text = " <color=yellow>綁定門號</color> 即可升級";
                }
                else if (getVIPLevel == 2)
                {
                    textComponent.text = " <color=yellow>首次儲值</color> 即可升級";
                }
                else if (getVIPLevel >= 3 && getVIPLevel < 9)
                {
                    int threshold = int.Parse(levelThreshold[getVIPLevel].Replace(",", ""));
                    string recharge = (threshold - accumulateTopUp).ToString();
                    textComponent.text = $"再儲值 <color=yellow>{recharge}</color> 即可升級";

                }
                else
                {
                    int max_threshold = int.Parse(levelThreshold[levelThreshold.Length - 1].Replace(",", ""));
                    int remain = accumulateTopUp - max_threshold;
                    int second_max_threshold = int.Parse(levelThreshold[levelThreshold.Length - 2].Replace(",", ""));

                    int thresholdDist = max_threshold - second_max_threshold;
                    remain %= thresholdDist;
                    string recharge = (thresholdDist - remain).ToString();
                    textComponent.text = $"再儲值 <color=yellow>{recharge}</color> 即可升級";

                }

                //Debug.Log($"Requirement: {textComponent.text}");
            }
        }

        private void UpdateExpBar()
        {
            //Debug.Log("Update Exp Bar");
            if (getVIPLevel < 3)
            {
                vipExpBar.value = 0f;
            }
            else if (getVIPLevel >= 3 && getVIPLevel < 9)
            {
                int threshold = int.Parse(levelThreshold[getVIPLevel].Replace(",", ""));
                int recharge = 0;// threshold - accumulateTopUp;

                if (getVIPLevel > 3)
                {
                    //threshold -= int.Parse(levelThreshold[getVIPLevel - 1].Replace(",", ""));
                    recharge = accumulateTopUp - int.Parse(levelThreshold[getVIPLevel - 1].Replace(",", ""));
                    threshold -= int.Parse(levelThreshold[getVIPLevel - 1].Replace(",", ""));
                }
                else
                {
                    recharge = accumulateTopUp;
                }

                vipExpBar.value = Mathf.Clamp01((float)recharge / (float)threshold);

                //Debug.Log($"rechkarge: {recharge}, threshold: {threshold}");
            }
            else
            {
                int max_threshold = int.Parse(levelThreshold[levelThreshold.Length - 1].Replace(",", ""));
                int remain = accumulateTopUp - max_threshold;
                int second_max_threshold = int.Parse(levelThreshold[levelThreshold.Length - 2].Replace(",", ""));

                int thresholdDist = max_threshold - second_max_threshold;
                remain %= thresholdDist;

                vipExpBar.value = Mathf.Clamp01((float)remain / (float)(thresholdDist));
                //Debug.Log($"recharge: {remain}, threshold: {thresholdDist}");
            }

            //Debug.Log("Bar value: " + vipExpBar.value);

        }
        #endregion

        #region Helper

        private IEnumerator GetPlayerProfileCoroutine(GetPlayerProfileRequest request)
        {
            bool isRequestCompleted = false;

            PlayFabClientAPI.GetPlayerProfile(
                request,
                (result) =>
                {
                    if (result.PlayerProfile != null)
                    {
                        // �����F���e���a�� PlayFab ID�C
                        playFabId = result.PlayerProfile.PlayerId;
                    }
                    isRequestCompleted = true;
                },
                (error) =>
                {
                    Debug.LogError("GetPlayerProfile request failed: " + error.GenerateErrorReport());
                    isRequestCompleted = true;
                }
            );

            // ���� GetPlayerProfile �����C
            while (!isRequestCompleted)
            {
                yield return null;
            }
        }

        private string FormatNumberWithCommas(int number)
        {
            string numberStr = number.ToString();

            // �p�G�Ʀr�p�� 1000�A�h�L�ݥ[�r��
            if (number < 1000)
            {
                return numberStr;
            }

            // �إߤ@�ӥΩ�s�񵲪G���r��
            string result = "";

            // �j��q�r�ꪺ�̫�@�Ӧr���}�l�B�z
            for (int i = numberStr.Length - 1; i >= 0; i--)
            {
                // �N���e�r���[�쵲�G�r�ꪺ�}�Y
                result = numberStr[i] + result;

                // �ˬd�O�_�ݭn�[�r��
                if ((numberStr.Length - i) % 3 == 0 && i > 0)
                {
                    result = "," + result;
                }
            }

            return result;
        }
        #endregion

    }
}