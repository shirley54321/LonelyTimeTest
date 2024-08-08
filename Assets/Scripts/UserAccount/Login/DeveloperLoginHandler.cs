using System;
using UnityEngine;

namespace UserAccount
{
    [RequireComponent(typeof(AccountLoginHandler))]
    public class DeveloperLoginHandler : MonoBehaviour
    {
        [Header("Setting")]
        public bool LoginOnStart;
        
        [Header("Parameter")]
        public string accountName;
        public string password;

        private AccountLoginHandler accountLoginHandler;

        private void Awake()
        {
            accountLoginHandler = GetComponent<AccountLoginHandler>();
        }

        void Start()
        {
            if (LoginOnStart)
            {
                Login();
            }
        }

        [ContextMenu("Login")]
        private void Login()
        {
            accountLoginHandler.StartLogin(accountName, password);
        }

    }
}