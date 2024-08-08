using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using System.Collections;
using System.Collections.Generic;
using Loading;
using UnityEngine;
using UnityEngine.Events;
using UserAccount.AccountAndPhone;

namespace UserAccount
{
    

    /// <summary>
    /// Manages user account-related operations, including login success and failure handling.
    /// </summary>
    public class UserAccountManager : MonoBehaviour
    {
        #region Instance(Singleton)

        private static UserAccountManager instance;

        /// <summary>
        /// Singleton instance of UserAccountManager.
        /// </summary>
        public static UserAccountManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UserAccountManager>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject of type {typeof(UserAccountManager)} does not exist in the scene, yet its method is being called.\n" +
                                       $"Please add {typeof(UserAccountManager)} to the scene.");
                    }

                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        /// <summary>
        /// Event triggered on successful user login.
        /// Subscribers can attach methods to this event.
        /// </summary>
        public static readonly UnityEvent<LoginResult> OnLoginSuccessful = new UnityEvent<LoginResult>();

        /// <summary>
        /// Event triggered on failed user login.
        /// Subscribers receive a specific failure code (LoginFailedCode) indicating the reason for the failure.
        /// Subscribers can attach methods to this event to handle different failure scenarios.
        /// </summary>
        public static readonly UnityEvent<LoginFailedCode> OnLoginFailed = new UnityEvent<LoginFailedCode>();

        
        [SerializeField] private LoginResult _loginResult;
        /// <summary>
        /// Indicates whether the user has successfully logged in.
        /// </summary>
        [SerializeField] private bool haveLogin;

        /// <summary>
        /// Handles the linking of Quick Login ID for user authentication.
        /// </summary>
        private readonly QuickLoginIdHandler quickLoginIdHandler = new QuickLoginIdHandler();

        /// <summary>
        /// Coroutine to handle actions after a successful PlayFab login.
        /// </summary>
        private Coroutine _handlePlayFabLoginSuccessfulCoroutine;
        
        #region Variable

        /// <summary>
        /// Checks if the user is logged in.
        /// </summary>
        /// <returns>True if the user is logged in, false otherwise.</returns>
        public bool HaveLogin()
        {
            return haveLogin;
        }

        /// <summary>
        /// Gets the Quick Login ID for the user.
        /// </summary>
        /// <returns>Quick Login ID.</returns>
        public string QuickLoginId()
        {
            return quickLoginIdHandler.QuickLoginID;
        }

        /// <summary>
        /// Clears the Quick Login ID.
        /// </summary>
        [ContextMenu("Clear QuickLoginId")]
        public void ClearQuickLoginId()
        {
            quickLoginIdHandler.ClearGuID();
        }

        #endregion

        #region Handle Login Success

        /// <summary>
        /// Handles the success of user login with PlayFab.
        /// </summary>
        /// <param name="result">Login result.</param>
        public void HandlePlayFabLoginSuccess(LoginResult result, bool isAccountLogin = false)
        {
            Debug.Log($"Handle Login Success {result}");
            _loginResult = result;
            _handlePlayFabLoginSuccessfulCoroutine = StartCoroutine(HandlePlayFabLoginSuccessCoroutine(isAccountLogin));
        }

        /// <summary>
        /// Coroutine to handle actions after successful PlayFab login.
        /// </summary>
        private IEnumerator HandlePlayFabLoginSuccessCoroutine(bool isAccountLogin)
        {
            // Wait until a response is received
            yield return CheckIfCancelAccount();

            if (isAccountLogin)
            {
                var CheckIsFirstLogin = new CheckIsFirstLogin();
                yield return CheckIsFirstLogin.CheckIsFirstTime(_loginResult.PlayFabId);

                if(CheckIsFirstLogin.IsFirstTime)
                {
                    LoadingManger.Instance.Close_Loading_animator();
                    yield return phoneInfoGetter.GetPhoneInfoCoroutine();
                    
                    if (phoneInfoGetter.HaveLinkPhone())
                    {
                        Debug.Log("have Phone");
                        OpenVerificationProvisional.Instance.OpenPanel();
                        
                        var sMSHandler = OpenVerificationProvisional.Instance.SMSHandler;
                        //yield return sMSHandler.SendSMS(phoneInfoGetter.GetPhoneNumber(), _loginResult.InfoResultPayload.AccountInfo.Username);
                        OpenVerificationProvisional.Instance.TakePhoneAndName(phoneInfoGetter.GetPhoneNumber(), _loginResult.InfoResultPayload.AccountInfo.Username);
                        yield return new WaitUntil(()=>sMSHandler.IsValidateSuccessful());
                        LoadingManger.Instance.Open_Loading_animator();
                        yield return CheckIsFirstLogin.SaveDeviceCode();
                    }
                }
            }

            InvokeLoginSuccess();
        }

        public void InvokeLoginSuccess()
        {
            // Link with Quick Login ID
            quickLoginIdHandler.LinkWithQuickLoginID();

            haveLogin = true;
            OnLoginSuccessful.Invoke(_loginResult);
            LoadingManger.Instance.Close_Loading_animator();
        }

        private bool CanelUser = false;
        private bool OverThirtyDays=false;

        /// <summary>
        /// Coroutine to check if the user account has been canceled.
        /// </summary>
        private IEnumerator CheckIfCancelAccount()
        {
            bool isResponseReceived = false;
            yield return GetPlayerCanelTag();
            Debug.Log(CanelUser);
            if (!CanelUser) // Replace with actual condition to check if the account is canceled
            {
                isResponseReceived = true;
            }
            else
            {
                yield return GetDeleteDate();
                if(OverThirtyDays)
                {
                    // If account is canceled, trigger login failed with appropriate code
                    LoginFailed(LoginFailedCode.AccountCanceled);
                }
                else
                {
                    LoginFailed(LoginFailedCode.AccountCanceledInConsiderationPeriod);
                }
            }

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        private readonly PhoneInfoGetter phoneInfoGetter = new PhoneInfoGetter();

        #endregion

        #region Account Is Delete Or Not
        /// <summary>
        /// Take tag to know if this account or not
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetPlayerCanelTag()
        {
            bool isFin = false;
            var request = new GetPlayerTagsRequest()
            {
                PlayFabId = _loginResult.PlayFabId
            };

            PlayFabClientAPI.GetPlayerTags(request, result =>
            {
                isFin = true;
                foreach (var tags in result.Tags)
                {
                    Debug.Log("get player canel tag" + tags.Contains("Delete"));
                    if (tags.Contains("Delete"))
                    {
                        CanelUser = true;
                        break;
                    }
                }
            }, error =>
            {
                Debug.Log($"Get Player Tag Error: {error.ErrorMessage}");
            });
            yield return new WaitUntil(()=>isFin);
        }

        /// <summary>
        /// If account is delete, check the deletion date of read only data to see if it is more than 30 days ago
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetDeleteDate()
        {
            bool isFin = false;
            var request = new GetUserDataRequest()
            {
                PlayFabId = _loginResult.PlayFabId
            };

            PlayFabClientAPI.GetUserReadOnlyData(request, result =>
            {
                Debug.Log(result.Data["DeleteTime"].Value);

                DateTime Today = DateTime.Now.Date;
                if (Today.Subtract(DateTime.Parse(result.Data["DeleteTime"].Value)).Days < 30)
                {
                    Debug.Log("刪除日期小於30天");
                    RemoveDeleteTag();
                }
                else
                {
                    Debug.Log("超過30天");
                    OverThirtyDays = true;
                }

                isFin = true;
            }, error =>
            {
                Debug.Log(error.ErrorMessage);
            });
           yield return new WaitUntil(()=>isFin);
        }

        private void RemoveDeleteTag()
        {
            var request = new ExecuteFunctionRequest()
            {
                FunctionName = "RemoveDeleteTag"
            };

            PlayFabCloudScriptAPI.ExecuteFunction(request,result=>
            {
                Debug.Log("Remove Delete Tag Fin");
            },error=>
            {
                Debug.Log("Remove Delete Tag Fail " + error.ErrorMessage);
            });
        }

        #endregion

        #region Handle Login Fail

        /// <summary>
        /// Handles the failure of user login.
        /// </summary>
        /// <param name="error">PlayFab error information.</param>
        public void HandlePlayFabLoginFailed(PlayFabError error)
        {
            Debug.Log($"<color=red>Unsuccessful Login in PlayFab</color>\n" +
                      $"Error Code: {error.Error}\n" +
                      $"{error.ErrorMessage}");

            switch (error.Error)
            {
                case PlayFabErrorCode.AccountBanned:
                    LoginFailed(LoginFailedCode.AccountBanned);
                    break;
                case PlayFabErrorCode.AccountNotFound:
                case PlayFabErrorCode.InvalidParams:
                case PlayFabErrorCode.InvalidPassword:
                case PlayFabErrorCode.InvalidUsernameOrPassword:
                case PlayFabErrorCode.InvalidUsername:
                    LoginFailed(LoginFailedCode.IncorrectAccountOrPassword);
                    break;
                default:
                    LoadingManger.Instance.Close_Loading_animator();
                    ServerEventHandler.Call_Server_Error_Event(error);
                    break;
            }
        }

        /// <summary>
        /// Handles the failure of user login and invokes the OnLoginFailed event.
        /// </summary>
        /// <param name="failedCode">The code indicating the reason for login failure.</param>
        public void LoginFailed(LoginFailedCode failedCode)
        {
            // Stop the coroutine handling successful login
            if(_handlePlayFabLoginSuccessfulCoroutine != null)
                StopCoroutine(_handlePlayFabLoginSuccessfulCoroutine);
            Debug.Log($"<color=red>Login failed! {failedCode}</color>");
            
            // Invoke the OnLoginFailed event with the specified failure code
            OnLoginFailed.Invoke(failedCode);
            
            LoadingManger.Instance.Close_Loading_animator();
        }

        #endregion
    }
}
