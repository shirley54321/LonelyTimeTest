using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Loading{
    public class Loading_animator : MonoBehaviour
    {
        public GameObject LaObject;

        private void Awake()
        {
            Application.logMessageReceived += HandleLog;
        }
        public void OpenObject(){
            LaObject.SetActive(true);
        }

        public void CloseObject(){
            LaObject.SetActive(false);
        }

        private void HandleLog(string logText, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                if (LaObject != null && LaObject.activeSelf)
                {
                    LaObject.SetActive(false);
                }
            }
        }         

    }
}